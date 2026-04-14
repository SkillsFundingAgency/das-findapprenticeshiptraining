using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.Models;

public class ShortlistsViewModel
{
    public string ExpiryDateText { get; set; }
    public bool HasMaxedOutShortlists { get; set; }
    public List<ShortlistCourseViewModel> Courses { get; set; } = [];
    public bool HasShortlistItems => Courses.Count > 0;
    public string RemovedProviderName { get; set; }
    public bool ShowRemovedShortlistBanner => !string.IsNullOrEmpty(RemovedProviderName);
}

public class ShortlistCourseViewModel
{
    public string LarsCode { get; set; }
    public string CourseTitle { get; set; }
    public CourseType CourseType { get; set; }
    public LearningType LearningType { get; set; }
    public List<ShortlistLocationViewModel> Locations { get; set; } = [];
    public string LearningTypeTagClass => LearningType.GetTagClass();
    public bool IsShortCourseType => CourseType == CourseType.ShortCourse;
}

public class ShortlistLocationViewModel
{
    public string Description { get; set; }
    public string ReviewPeriod { get; set; }
    public string QarPeriod { get; set; }
    public RequestApprenticeshipTrainingViewModel RequestApprenticeshipTraining { get; set; }
    public List<ShortlistProviderViewModel> Providers { get; set; } = [];
}

public class RequestApprenticeshipTrainingViewModel
{
    public string CourseTitle { get; set; }
    public string Url { get; set; }
}

public class ShortlistProviderViewModel
{
    public string LarsCode { get; set; }
    public CourseType CourseType { get; set; }
    public LearningType LearningType { get; set; }
    public Guid ShortlistId { get; set; }
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public bool AtEmployer { get; set; }
    public bool HasBlockRelease { get; set; }
    public decimal? BlockReleaseDistance { get; set; }
    public bool HasMultipleBlockRelease { get; set; }
    public bool HasDayRelease { get; set; }
    public decimal? DayReleaseDistance { get; set; }
    public bool HasMultipleDayRelease { get; set; }
    public bool AtProviderLocation { get; set; }
    public bool HasOnlineDeliveryOption { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public ProviderRatingViewModel EmployerReviews { get; set; }
    public ProviderRatingViewModel ApprenticeReviews { get; set; }
    public string LocationDescription { get; set; }

    public int NoOfDeliveryOptions => Convert.ToInt32(AtEmployer) + Convert.ToInt32(HasDayRelease) + Convert.ToInt32(HasBlockRelease) + Convert.ToInt32(AtProviderLocation) + Convert.ToInt32(HasOnlineDeliveryOption);
    public bool HasMultipleDeliveryOptions => NoOfDeliveryOptions > 1;
    public bool HasAchievementRate => decimal.TryParse(AchievementRate, out var _);
    public bool HasLocation => !string.IsNullOrEmpty(LocationDescription);
    public bool IsShortCourseType => CourseType == CourseType.ShortCourse;

    public const string ApprenticeShortCourseRatingDescription = "Achievement rate data isn’t available for apprenticeship units";
    public const string ApprenticeNoRatingDescription = "No achievement rate - not enough data";
    public const string EmployerShortCourseRatingDescription = "Provider reviews aren’t available for apprenticeship units";

    public const string OnlineTrainingOptionLabel = "Online";
    public const string AtLearnerWorkplaceTrainingOptionLabel = "At learner's workplace";
    public const string AtTrainingProviderLocationTrainingOptionLabel = "At training provider's location";
}
