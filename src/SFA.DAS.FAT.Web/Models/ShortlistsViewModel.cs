using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.Models;

public class ShortlistsViewModel
{
    public string ExpiryDateText { get; set; }

    public List<ShortlistCourseViewModel> Courses { get; set; } = [];
    public bool HasShortlistItems => Courses.Count > 0;

    public string RemovedProviderName { get; set; }
    public bool ShowRemovedShortlistBanner => !string.IsNullOrEmpty(RemovedProviderName);
}

public class ShortlistCourseViewModel
{
    public int LarsCode { get; set; }
    public string CourseTitle { get; set; }
    public List<ShortlistLocationViewModel> Locations { get; set; } = [];
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
    public int LarsCode { get; set; }
    public Guid ShortlistId { get; set; }
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public bool AtEmployer { get; set; }
    public bool HasBlockRelease { get; set; }
    public decimal? BlockReleaseDistance { get; set; }
    public int BlockReleaseCount { get; set; }
    public bool HasDayRelease { get; set; }
    public decimal? DayReleaseDistance { get; set; }
    public int DayReleaseCount { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public ProviderRatingViewModel EmployerReviews { get; set; }
    public ProviderRatingViewModel ApprenticeReviews { get; set; }
    public string LocationDescription { get; set; }

    public string BlockReleaseText => "Block release" + (BlockReleaseCount == 1 ? string.Empty : " at multiple locations");
    public string DayReleaseText => "Day release" + (DayReleaseCount == 1 ? string.Empty : " at multiple locations");
    public int NoOfDeliveryOptions => Convert.ToInt32(AtEmployer) + Convert.ToInt32(HasDayRelease) + Convert.ToInt32(HasBlockRelease);
    public bool HasMultipleDeliveryOptions => NoOfDeliveryOptions > 1;
    public bool HasAchievementRate => decimal.TryParse(AchievementRate, out var _);
    public bool HasLocation => !string.IsNullOrEmpty(LocationDescription);
}
