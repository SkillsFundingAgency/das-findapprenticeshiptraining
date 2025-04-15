using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderDetailsViewModel : PageLinksViewModelBase
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }

    public string ProviderAddress { get; set; }
    public ContactDetailsModel Contact { get; set; }
    public ProviderCoursesModel ProviderCoursesDetails { get; set; }

    public QarModel Qar { get; set; }

    public EndpointAssessmentModel EndpointAssessments { get; set; }

    public ReviewsModel Reviews { get; set; }

    public bool ShowCourses => ProviderCoursesDetails.CourseCount > 0;

    public bool EmployerReviewed => Reviews.EmployerRating != ProviderRating.NotYetReviewed.ToString();
    public bool ApprenticeReviewed => Reviews.ApprenticeRating != ProviderRating.NotYetReviewed.ToString();


    public static implicit operator ProviderDetailsViewModel(GetProviderQueryResponse source)
    {
        var vm = new ProviderDetailsViewModel
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = ((GetProviderAddress)source.ProviderAddress).GetComposedAddress(source.ProviderName),
            Contact = source.Contact,
            ProviderCoursesDetails = source.Courses,
            Qar = source.Qar,
            Reviews = source.Reviews,
            EndpointAssessments = source.EndpointAssessments,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        if (vm.ProviderCoursesDetails is { Courses: { } })
        {
            foreach (var course in vm.ProviderCoursesDetails.Courses)
            {
                var routeData = new Dictionary<string, string>
                {
                    { "id", course.LarsCode.ToString() },
                    { "providerId", vm.Ukprn.ToString() }
                };

                course.RouteData = routeData;
            }
        }

        return vm;
    }

}

public class ReviewsModel
{
    public string StartYear { get; set; } = string.Empty;

    public string EndYear { get; set; } = string.Empty;

    public string ApprenticeRating { get; set; } = string.Empty;
    public string EmployerRating { get; set; } = string.Empty;

    public int EmployerStarsValue { get; set; }

    public int ApprenticeStarsValue { get; set; }

    public int EmployerReviewCount { get; set; }

    public int ApprenticeReviewCount { get; set; }

    public string EmployerStarsMessage { get; set; } = string.Empty;
    public string ApprenticeStarsMessage { get; set; } = string.Empty;

    public static implicit operator ReviewsModel(GetProviderReviewsModel source)
    {
        if (source == null) return new ReviewsModel();


        var reviewsStartYear = $"20{source.ReviewPeriod.AsSpan(0, 2)}";
        var reviewsEndYear = $"20{source.ReviewPeriod.AsSpan(2, 2)}";
        var reviewEmployerStarsValue = int.TryParse(source.EmployerStars, out var employerStars) ? employerStars : 0;
        var reviewApprenticeStarsValue =
            int.TryParse(source.ApprenticeStars, out var apprenticeStars) ? apprenticeStars : 0;
        var employerReviewCount = int.TryParse(source.EmployerReviews, out var reviewCount) ? reviewCount : 0;
        var apprenticeReviewCount = int.TryParse(source.ApprenticeReviews, out var appReviewCount) ? appReviewCount : 0;


        var employerStarsMessage = employerReviewCount == 1
            ? $"average review from {employerReviewCount} employer when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'."
            : $"average review from {employerReviewCount} employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";

        var apprenticeStarsMessage = apprenticeReviewCount == 1
            ? $"average review from {apprenticeReviewCount} apprentice when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'."
            : $"average review from {apprenticeReviewCount} apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";

        return new ReviewsModel
        {
            StartYear = reviewsStartYear,
            EndYear = reviewsEndYear,
            ApprenticeRating = source.ApprenticeRating,
            EmployerRating = source.EmployerRating,
            EmployerStarsValue = reviewEmployerStarsValue,
            ApprenticeStarsValue = reviewApprenticeStarsValue,
            EmployerStarsMessage = employerStarsMessage,
            EmployerReviewCount = employerReviewCount,
            ApprenticeStarsMessage = apprenticeStarsMessage,
            ApprenticeReviewCount = apprenticeReviewCount
        };
    }
}

public class EndpointAssessmentModel
{

    public string CountFormatted { get; set; } = string.Empty;
    public string DetailsMessage { get; set; } = string.Empty;

    public static implicit operator EndpointAssessmentModel(GetProviderEndpointAssessmentsDetails source)
    {
        if (source == null) return new EndpointAssessmentModel();

        var endpointAssessmentCount = source.EndpointAssessmentCount;

        var countFormatted = endpointAssessmentCount.ToString("N0");

        string detailsMessage;

        if (endpointAssessmentCount <= 0 || source.EarliestAssessment == null)
        {
            detailsMessage =
                "apprentices have completed a course and taken their end-point assessment with this provider.";
        }
        else
        {
            var earliestYear = source.EarliestAssessment.Value.Year.ToString();

            detailsMessage = endpointAssessmentCount == 1
                ? $"apprentice has completed a course and taken their end-point assessment with this provider since {earliestYear}."
                : $"apprentices have completed a course and taken their end-point assessment with this provider since {earliestYear}.";
        }

        return new EndpointAssessmentModel
        {
            CountFormatted = countFormatted,
            DetailsMessage = detailsMessage
        };
    }
}

public class QarModel
{
    public bool AchievementRatePresent { get; set; }

    public string PeriodStartYear { get; set; }

    public string PeriodEndYear { get; set; }

    public int Achievers { get; set; }

    public int TotalParticipantCount { get; set; }
    public string DidNotPassPercentage { get; set; }

    public string AchievementRate { get; set; }
    public string NationalAchievementRate { get; set; }

    public static implicit operator QarModel(GetProviderQarModel source)
    {
        var periodStartYear = string.Empty;
        var periodEndYear = string.Empty;
        var achievers = 0;
        var totalParticipantCount = 0;
        var didNotPassPercentage = string.Empty;

        if (source == null) return new QarModel();
        var isAchievementRateNumeric = double.TryParse(source.AchievementRate, out double achievementRateValue);
        var isLeaversCountNumeric = int.TryParse(source.Leavers, out int leaversValue);

        var achievementRatePresent = isAchievementRateNumeric && isLeaversCountNumeric && achievementRateValue != 0;

        if (source.Period is { Length: >= 4 })
        {
            periodStartYear = $"20{source.Period.AsSpan(0, 2)}";
            periodEndYear = $"20{source.Period.AsSpan(2, 2)}";
        }

        if (isAchievementRateNumeric && isLeaversCountNumeric && achievementRateValue != 0)
        {
            achievers = (int)Math.Round(leaversValue * (achievementRateValue / 100));
        }

        if (isLeaversCountNumeric)
        {
            totalParticipantCount = leaversValue;
        }

        if (isAchievementRateNumeric)
        {
            didNotPassPercentage = (100 - achievementRateValue).ToString("0.0");
        }

        return new QarModel
        {
            AchievementRatePresent = achievementRatePresent,
            PeriodStartYear = periodStartYear,
            PeriodEndYear = periodEndYear,
            Achievers = achievers,
            TotalParticipantCount = totalParticipantCount,
            DidNotPassPercentage = didNotPassPercentage,
            AchievementRate = source.AchievementRate,
            NationalAchievementRate = source.NationalAchievementRate
        };
    }
}

public class GetProviderAddress
{
    public string AddressLine1 { get; init; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }

    public static implicit operator GetProviderAddress(GetProviderAddressModel source)
    {
        if (source == null) return new GetProviderAddress();

        return new GetProviderAddress
        {
            AddressLine1 = source.AddressLine1,
            AddressLine2 = source.AddressLine2,
            AddressLine3 = source.AddressLine3,
            AddressLine4 = source.AddressLine4,
            Town = source.Town,
            Postcode = source.Postcode
        };
    }

    public string GetComposedAddress(string providerName)
    {

        var addressItems = new List<string>
                {
                    providerName,
                    AddressLine1,
                    AddressLine2,
                    AddressLine3,
                    AddressLine4,
                    Town,
                    Postcode
                };

        return string.Join(", ", addressItems.Where(s => !string.IsNullOrEmpty(s)).ToArray());

    }
}

public class ContactDetailsModel
{
    public string MarketingInfo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public static implicit operator ContactDetailsModel(GetProviderContactDetails source)
    {
        if (source == null) return new ContactDetailsModel();

        var website = source.Website;

        if (website != null && !website.StartsWith("http") && website.Trim() != string.Empty)
        {
            website = $"http://{website}";
        }

        return new ContactDetailsModel
        {
            MarketingInfo = source.MarketingInfo,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Website = website ?? string.Empty
        };
    }
}

public class ProviderCoursesModel
{
    public List<ProviderCourseDetails> Courses { get; set; }
    public int CourseCount { get; set; }

    public string CoursesDropdownText { get; set; } = string.Empty;

    public static implicit operator ProviderCoursesModel(List<GetProviderCourseDetails> source)
    {
        if (source == null) return new ProviderCoursesModel();

        var details = new ProviderCoursesModel();
        var courses = new List<ProviderCourseDetails>();
        foreach (var course in source)
        {
            courses.Add(course);
        }

        details.Courses = courses;
        var coursesCount = courses.Count;
        details.CourseCount = coursesCount;

        details.CoursesDropdownText = coursesCount == 1
        ? $"View 1 course delivered by this training provider"
        : $"View {coursesCount} courses delivered by this training provider";

        return details;
    }
}


public class ProviderCourseDetails
{
    public string CourseName { get; init; }
    public int Level { get; init; }

    public int LarsCode { get; init; }

    public Dictionary<string, string> RouteData { get; set; } = new();

    public static implicit operator ProviderCourseDetails(GetProviderCourseDetails source)
    {
        return new ProviderCourseDetails
        {
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = source.LarsCode
        };
    }
}
