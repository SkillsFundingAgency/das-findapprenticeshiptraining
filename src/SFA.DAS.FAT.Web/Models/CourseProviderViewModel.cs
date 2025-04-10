using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProviderViewModel : PageLinksViewModelBase
{
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
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
    public IEnumerable<LocationModel> Locations { get; set; }
    public IEnumerable<ProviderCourseModel> Courses { get; set; } = [];
    public IEnumerable<AnnualEmployerFeedbackDetailsModel> AnnualEmployerFeedbackDetails { get; set; } = [];
    public IEnumerable<AnnualApprenticeFeedbackDetailsModel> AnnualApprenticeFeedbackDetails { get; set; } = [];
    public string CourseNameAndLevel => $"{CourseName} (level {Level})";
    public string AchievementRateInformation => GetAchievementRateInformation();
    public bool IsNational => Locations.Any(a => a.AtEmployer);
    public bool IsBlockRelease => Locations.Any(a => a.BlockRelease);
    public bool IsDayRelease => Locations.Any(a => a.DayRelease);
    public List<string> BlockReleaseLocations => FormatLocations(Locations.Where(a => a.BlockRelease));
    public List<string> DayReleaseLocations => FormatLocations(Locations.Where(a => a.DayRelease));
    public string ApprenticeWorkplaceDisplayMessage => GetApprenticeWorkplaceDisplayMessage();
    public bool HasMultipleBlockReleaseLocations => Locations.Count(a => a.BlockRelease) > 1;
    public bool HasMultipleDayReleaseLocations => Locations.Count(a => a.BlockRelease) > 1;
    public string EmployerReviewsDisplayMessage => GetEmployerReviewsDisplayMessage();
    public string ApprenticeReviewsDisplayMessage => GetApprenticeReviewsDisplayMessage();
    public string EndpointAssessmentDisplayMessage => GetEndpointAssessmentDisplayMessage();
    public string EndpointAssessmentsCountDisplay => EndpointAssessments is null || 
                                                     !EndpointAssessments.EarliestAssessment.HasValue ? 
                                                        "No data" : 
                                                        EndpointAssessments.EndpointAssessmentCount.ToString();

    public string ContactAddress => FormatContactAddress();
    public string CoursesDeliveredCountDisplay => CoursesDeliveredDisplayText();
    private string GetEndpointAssessmentDisplayMessage()
    {
        if(EndpointAssessments is not null && EndpointAssessments.EarliestAssessment.HasValue)
        {
            int assessmentYear = EndpointAssessments.EarliestAssessment.Value.Year;

            if(EndpointAssessments.EndpointAssessmentCount == 0)
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

    public static implicit operator CourseProviderViewModel(GetCourseProviderQueryResult source)
    {
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
            Locations = source.Locations,
            Courses = source.Courses,
            AnnualEmployerFeedbackDetails = source.AnnualEmployerFeedbackDetails,
            AnnualApprenticeFeedbackDetails = source.AnnualApprenticeFeedbackDetails
        };
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

    private List<string> FormatLocations(IEnumerable<LocationModel> locations)
    {
        if (!locations.Any())
        {
            return [];
        }

        List<string> locationAddresses = [];

        foreach(LocationModel location in locations.OrderBy(a => a.Postcode))
        {
            locationAddresses.Add(BuildLocationAddress(location));
        }

        return locationAddresses;
    }

    private string BuildLocationAddress(LocationModel location)
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

        AppendIfNotEmpty(location.AddressLine1);
        AppendIfNotEmpty(location.AddressLine2);
        AppendIfNotEmpty(location.Town);
        AppendIfNotEmpty(location.County);
        AppendIfNotEmpty(location.Postcode);

        return builder.ToString();
    }

    private string GetApprenticeWorkplaceDisplayMessage()
    {
        LocationModel trainingLocation = Locations.FirstOrDefault(a => a.AtEmployer);

        if(trainingLocation is not null && trainingLocation.LocationType == LocationType.National)
        {
            return "Training is provided at apprentice’s workplaces across England.";
        }
        else
        {
            return "Training is provided at apprentice’s workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the apprentice’s workplace in your location.";
        }
    }

    private string GetAchievementRateInformation()
    {
        if(this.Qar.AchievementRate is not null)
        {
            return @$"of apprentices ({Qar.ConvertedLeavers} of {Qar.TotalParticipants}) completed this course and passed their end-point assessment with this
                    provider in academic year {Qar.PeriodDisplay}. {Qar.FailureRate}% did not pass or left the course before taking the
                    assessment.";
        }

        return string.Empty;
    }

    private string CoursesDeliveredDisplayText()
    {
        int courseCount = Courses.Count();

        return $"View {courseCount} course" + (courseCount == 1 ? string.Empty : "s") + " delivered by this training provider";
    }

    private string GetEmployerReviewsDisplayMessage()
    {
        if (int.TryParse(Reviews.EmployerReviews, out int employerReviewsCount))
        {
            string plural = employerReviewsCount > 1 ? "s" : string.Empty;
            return @$"average review from {Reviews.EmployerReviews} employer{plural} when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";
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
            return @$"average review from {Reviews.ApprenticeReviews} apprentice{plural} when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";
        }
        else
        {
            return string.Empty;
        }
    }
}
