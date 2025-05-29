using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProviderViewModel : PageLinksViewModelBase
{
    public int ShortlistCount { get; set; }
    public long Ukprn { get; set; }
    public string ProviderName { get; set; }
    public ShortProviderAddressModel ProviderAddress { get; set; }
    public ContactModel Contact { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public int LarsCode { get; set; }
    public string IFateReferenceNumber { get; set; }
    public QarModel Qar { get; set; }
    public ReviewsModel Reviews { get; set; }
    public EndpointAssessmentModel EndpointAssessments { get; set; }
    public int TotalProvidersCount { get; set; }
    public Guid? ShortlistId { get; set; }
    public IReadOnlyCollection<LocationModel> Locations { get; set; }
    public IReadOnlyCollection<ProviderCourseModel> Courses { get; set; } = [];
    public string CourseNameAndLevel => $"{CourseName} (level {Level})";
    public string AchievementRateInformation => GetAchievementRateInformation();
    public bool ShowApprenticesWorkplaceOption => Locations.Any(a => a.LocationType == LocationType.National || a.LocationType == LocationType.Regional);
    public bool ShowBlockReleaseOption => Locations.Any(a => a.BlockRelease);
    public bool ShowDayReleaseOption => Locations.Any(a => a.DayRelease);
    public List<LocationModel> BlockReleaseLocations => GetBlockReleaseLocations();
    public List<LocationModel> DayReleaseLocations => GetDayReleaseLocations();
    public string AtApprenticesWorkplaceWithNoLocationDisplayMessage => GetAtApprenticeWorkplaceWithNoLocationDisplayMessage();
    public bool HasMultipleBlockReleaseLocations => BlockReleaseLocations.Count(a => a.BlockRelease) > 1;
    public bool HasMultipleDayReleaseLocations => DayReleaseLocations.Count(a => a.DayRelease) > 1;
    public LocationModel ClosestBlockReleaseLocation => GetClosestBlockReleaseLocation();
    public LocationModel ClosestDayReleaseLocation => GetClosestDayReleaseLocation();
    public string EmployerReviewsDisplayMessage => GetEmployerReviewsDisplayMessage();
    public string ApprenticeReviewsDisplayMessage => GetApprenticeReviewsDisplayMessage();
    public string EndpointAssessmentDisplayMessage => GetEndpointAssessmentDisplayMessage();
    public string EndpointAssessmentsCountDisplay =>
        EndpointAssessments is null || !EndpointAssessments.EarliestAssessment.HasValue ?
        "No data" :
        EndpointAssessments.EndpointAssessmentCount.ToString("N0");
    public LocationModel NationalLocation => Locations.FirstOrDefault(a => a.AtEmployer && a.LocationType == LocationType.National);
    public string ContactAddress => FormatContactAddress();
    public string CoursesDeliveredCountDisplay => CoursesDeliveredDisplayText();
    public string ShortlistClass => GetShortlistClass();
    public bool HasMatchingRegionalLocation => Locations.Any(l => (l.LocationType == LocationType.National) || (l.LocationType == LocationType.Regional && l.AtEmployer));

    public FeedbackSurveyViewModel FeedbackSurvey { get; set; }

    public static implicit operator CourseProviderViewModel(GetCourseProviderQueryResult source)
    {

        var orderedCourses = new List<ProviderCourseModel>();
        if (source.Courses != null)
        {
            orderedCourses.AddRange(source.Courses.OrderBy(c => c.CourseName).ThenBy(c => c.Level));
        }

        return new CourseProviderViewModel
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = source.ProviderAddress,
            Contact = source.Contact,
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = source.LarsCode,
            IFateReferenceNumber = source.IFateReferenceNumber,
            Qar = source.Qar,
            EndpointAssessments = source.EndpointAssessments,
            Reviews = source.Reviews,
            TotalProvidersCount = source.TotalProvidersCount,
            ShortlistId = source.ShortlistId,
            Locations = source.Locations?.ToList() ?? [],
            Courses = orderedCourses
        };
    }

    private string GetShortlistClass()
    {
        if (ShortlistId is null)
        {
            return ShortlistCount < ShortlistConstants.MaximumShortlistCount ? string.Empty : "app-provider-shortlist-full";
        }
        else
        {
            return "app-provider-shortlist-added";
        }
    }

    private List<LocationModel> GetBlockReleaseLocations()
    {
        return Locations.Where(a => a.BlockRelease).ToList();
    }

    private List<LocationModel> GetDayReleaseLocations()
    {
        return Locations.Where(a => a.DayRelease).ToList();
    }

    private string GetEndpointAssessmentDisplayMessage()
    {
        if (EndpointAssessments is not null && EndpointAssessments.EarliestAssessment.HasValue)
        {
            int assessmentYear = EndpointAssessments.EarliestAssessment.Value.Year;

            if (EndpointAssessments.EndpointAssessmentCount == 0)
            {
                return "apprentices have completed a course and taken their end-point assessment with this provider.";
            }
            else
            {
                var plural = EndpointAssessments.EndpointAssessmentCount == 1 ? string.Empty : "s";
                return $"apprentice{plural} have completed this course and taken their end-point assessment with this provider since {assessmentYear}";
            }
        }
        else
        {
            return "Not enough apprentices have completed a course with this provider to show participation";
        }
    }

    private LocationModel GetClosestBlockReleaseLocation()
    {
        return Locations.Where(a => a.BlockRelease).OrderBy(a => a.CourseDistance).FirstOrDefault();
    }

    private LocationModel GetClosestDayReleaseLocation()
    {
        return Locations.Where(a => a.DayRelease).OrderBy(a => a.CourseDistance).FirstOrDefault();
    }

    private string FormatContactAddress()
    {
        var builder = new StringBuilder();

        void AppendIfNotEmpty(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(value.Trim());
            }
        }

        AppendIfNotEmpty(ProviderAddress.AddressLine1);
        AppendIfNotEmpty(ProviderAddress.AddressLine2);
        AppendIfNotEmpty(ProviderAddress.AddressLine3);
        AppendIfNotEmpty(ProviderAddress.AddressLine4);
        AppendIfNotEmpty(ProviderAddress.Town);
        AppendIfNotEmpty(ProviderAddress.Postcode);

        return builder.ToString();
    }

    private string GetAtApprenticeWorkplaceWithNoLocationDisplayMessage()
    {
        if (Locations.Any(l => l.LocationType == LocationType.National))
        {
            return "Training is provided at apprentice's workplaces across England.";
        }
        else
        {
            return "Training is provided at apprentice's workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the apprentice's workplace in your location.";
        }
    }

    private string GetAchievementRateInformation()
    {
        string leaversText = Qar.Leavers;
        if (int.TryParse(Qar.Leavers, out var leaversCount))
        {
            leaversText = leaversCount.ToString("N0");
        }

        if (this.Qar.AchievementRate is not null)
        {
            return $"of apprentices ({Qar.TotalNumberOfCompletedParticipants.ToString("N0")} of {leaversText}) completed this course and passed their end-point assessment with this " +
                   $"provider in academic year {Qar.PeriodDisplay}. {Qar.FailureRate}% did not pass or left the course before taking the " +
                    "assessment.";
        }

        return string.Empty;
    }

    private string CoursesDeliveredDisplayText()
    {
        return $"View {Courses.Count} course" + (Courses.Count == 1 ? string.Empty : "s") + " delivered by this training provider";
    }

    private string GetEmployerReviewsDisplayMessage()
    {
        if (int.TryParse(Reviews.EmployerReviews, out int employerReviewsCount))
        {
            string plural = employerReviewsCount > 1 ? "s" : string.Empty;
            return $"average review from {Reviews.EmployerReviews} employer{plural} when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";
        }
        else
        {
            return string.Empty;
        }
    }

    private string GetApprenticeReviewsDisplayMessage()
    {
        if (int.TryParse(Reviews.ApprenticeReviews, out int apprenticeReviewsCount))
        {
            string plural = apprenticeReviewsCount > 1 ? "s" : string.Empty;
            return $"average review from {Reviews.ApprenticeReviews} apprentice{plural} when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";
        }
        else
        {
            return string.Empty;
        }
    }
}
