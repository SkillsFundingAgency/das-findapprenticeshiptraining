using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;
using SFA.DAS.FAT.Web.Models.Shared;
using CourseQarModel = SFA.DAS.FAT.Domain.Courses.QarModel;
using CourseReviewsModel = SFA.DAS.FAT.Domain.Courses.ReviewsModel;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderDetailsViewModel : PageLinksViewModelBase, ICourseGroupModel
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }

    public string ProviderAddress { get; set; }
    public ContactDetailsViewModel ContactDetails { get; set; } = new();

    public ProviderCoursesModel ProviderCoursesDetails { get; set; } = new();

    public ProviderQarModel Qar { get; set; }

    public EndpointAssessmentModel EndpointAssessments { get; set; }

    public ReviewsModel Reviews { get; set; }

    public bool ShowCourses => ProviderCoursesDetails.CourseCount > 0;

    public FeedbackSurveyViewModel FeedbackSurvey { get; set; }

    public AchievementsAndParticipationViewModel AchievementsAndParticipation => BuildAchievementsAndParticipation();

    public ProviderReviewsViewModel ProviderReviews => BuildProviderReviews();

    public static implicit operator ProviderDetailsViewModel(GetProviderQueryResponse source)
    {
        List<GetProviderCourseDetails> orderedCourses = null;
        if (source.Courses != null)
        {
            orderedCourses = new List<GetProviderCourseDetails>();
            orderedCourses.AddRange(source.Courses.OrderBy(c => c.CourseName).ThenBy(c => c.Level));
        }

        var model = new ProviderDetailsViewModel
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = ((GetProviderAddress)source.ProviderAddress).GetComposedAddress(source.ProviderName),
            ContactDetails = source.Contact,
            ProviderCoursesDetails = new ProviderCoursesModel
            {
                Courses = orderedCourses?.Select(c => (ProviderCourseDetails)c).ToList(),
                Ukprn = source.Ukprn
            },
            Qar = source.Qar,
            Reviews = source.Reviews,
            EndpointAssessments = source.EndpointAssessments,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        model.ContactDetails.RegisteredAddress = model.ProviderAddress ?? string.Empty;

        return model;
    }

    private AchievementsAndParticipationViewModel BuildAchievementsAndParticipation()
    {
        var qar = Qar ?? new ProviderQarModel();
        var endpointAssessments = EndpointAssessments ?? new EndpointAssessmentModel();

        return new AchievementsAndParticipationViewModel
        {
            Qar = new CourseQarModel
            {
                AchievementRate = qar.AchievementRate,
                NationalAchievementRate = qar.NationalAchievementRate,
                Leavers = qar.TotalParticipantCount.ToString(),
                Period = string.Empty
            },
            AchievementRateInformation = $"of apprentices ({qar.Achievers:N0} of {qar.TotalParticipantCount:N0}) completed a course and passed the end-point assessment with this training provider in academic year {qar.PeriodStartYear} to {qar.PeriodEndYear}. {qar.DidNotPassPercentage}% did not pass or left the course before taking their assessment.",
            EndpointAssessmentsCountDisplay = endpointAssessments.CountFormatted,
            EndpointAssessmentDisplayMessage = endpointAssessments.DetailsMessage
        };
    }

    private ProviderReviewsViewModel BuildProviderReviews()
    {
        var reviews = Reviews ?? new ReviewsModel();

        return new ProviderReviewsViewModel
        {
            Reviews = new CourseReviewsModel
            {
                EmployerStars = reviews.EmployerStarsValue.ToString(),
                ApprenticeStars = reviews.ApprenticeStarsValue.ToString(),
                EmployerRating = ParseProviderRating(reviews.EmployerRating),
                ApprenticeRating = ParseProviderRating(reviews.ApprenticeRating)
            },
            EmployerReviewsDisplayMessage = reviews.EmployerStarsMessage,
            ApprenticeReviewsDisplayMessage = reviews.ApprenticeStarsMessage
        };
    }

    private static ProviderRating ParseProviderRating(string rating)
    {
        return Enum.TryParse<ProviderRating>(rating, out var parsedRating)
            ? parsedRating
            : ProviderRating.NotYetReviewed;
    }
}
