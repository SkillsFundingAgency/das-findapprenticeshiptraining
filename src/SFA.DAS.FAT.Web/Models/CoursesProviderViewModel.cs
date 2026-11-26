using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Helpers;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesProviderViewModel
{
    public const string ProviderCourseClosingDescription = "This training provider will stop offering this course on {0}";

    public int Ukprn { get; set; }
    public long Ordering { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public List<ProviderLocation> Locations { get; set; }

    public bool IsOnlineAvailable
    {
        get; init;
    }

    public bool IsEmployerLocationAvailable
    {
        get; init;
    }

    public decimal? NearestEmployerLocation
    {
        get; init;
    }

    public bool IsProviderAvailable
    {
        get; init;
    }

    public decimal? NearestProviderPlace
    {
        get; init;
    }

    public bool IsBlockReleaseAvailable
    {
        get; init;
    }

    public bool IsDayReleaseAvailable
    {
        get; init;
    }

    public bool IsBlockReleaseMultiple
    {
        get; init;
    }

    public bool IsDayReleaseMultiple
    {
        get; init;
    }

    public decimal? NearestBlockRelease
    {
        get; init;
    }

    public decimal? NearestDayRelease
    {
        get; init;
    }


    public string LocationName { get; set; }
    public string Distance { get; set; } = "All";

    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }
    public DateTime ProviderCourseAddedDate { get; set; }
    public DateTime? LastDateStarts { get; set; }
    public bool ShowProviderCourseAddedDate { get; set; }

    public string ProviderCourseAddedDateDisplay => CourseFundingStatusHelper.FormatDate(ProviderCourseAddedDate);
    public bool IsLastDateStartsInFuture => CourseFundingStatusHelper.IsLastDateStartsInFuture(LastDateStarts);
    public string ProviderCourseClosingMessage =>
        IsLastDateStartsInFuture
            ? string.Format(CultureInfo.InvariantCulture, ProviderCourseClosingDescription, CourseFundingStatusHelper.FormatDate(LastDateStarts.Value))
            : string.Empty;

    public TrainingOptionsViewModel TrainingOptionsViewModel
    {
        get
        {
            return new TrainingOptionsViewModel
            {
                IsEmployerLocationAvailable = IsEmployerLocationAvailable,
                NearestEmployerLocation = NearestEmployerLocation,
                IsDayReleaseAvailable = IsDayReleaseAvailable,
                IsDayReleaseMultiple = IsDayReleaseMultiple,
                NearestDayRelease = NearestDayRelease,
                IsBlockReleaseAvailable = IsBlockReleaseAvailable,
                IsBlockReleaseMultiple = IsBlockReleaseMultiple,
                NearestBlockRelease = NearestBlockRelease,
                Distance = Distance,
                LocationName = LocationName
            };
        }
    }

    public TrainingOptionsShortCourseViewModel TrainingOptionsShortCourseViewModel
    {
        get
        {
            return new TrainingOptionsShortCourseViewModel
            {
                IsOnlineAvailable = IsOnlineAvailable,
                IsEmployerLocationAvailable = IsEmployerLocationAvailable,
                NearestEmployerLocation = NearestEmployerLocation,
                IsProviderAvailable = IsProviderAvailable,
                NearestProviderPlace = NearestProviderPlace,
                OnlineDisplayDescription = FilterService.DeliveryModesSectionOnlineDisplayDescription,
                EmployerLocationDisplayDescription = FilterService.DeliveryModesSectionWorkplaceDisplayDescription,
                ProviderLocationDisplayDescription = FilterService.DeliveryModesSectionProviderDisplayDescription,
                Distance = Distance,
                LocationName = LocationName
            };
        }
    }

    public string AchievementRateMessage
    {
        get
        {
            string message = "No achievement rate - not enough data";
            if (float.TryParse(AchievementRate, out _) && float.TryParse(Leavers, out _))
            {
                message = $"{AchievementRate}% (out of {Leavers} apprentices)";
            }

            return message;
        }
    }

    public static implicit operator CoursesProviderViewModel(ProviderData source)
    {
        if (source == null)
        {
            return null;
        }

        return new CoursesProviderViewModel
        {
            Ukprn = source.Ukprn,
            Ordering = source.Ordering,
            ProviderName = source.ProviderName,
            ShortlistId = source.ShortlistId,
            Locations = source.Locations,
            Leavers = source.Leavers,
            AchievementRate = source.AchievementRate,
            EmployerReviews = source.EmployerReviews,
            EmployerStars = source.EmployerStars,
            EmployerRating = source.EmployerRating,
            ApprenticeReviews = source.ApprenticeReviews,
            ApprenticeStars = source.ApprenticeStars,
            ApprenticeRating = source.ApprenticeRating,
            ProviderCourseAddedDate = source.ProviderCourseAddedDate,
            LastDateStarts = source.LastDateStarts,

            IsOnlineAvailable = source.Locations.Any(x => x.LocationType == LocationType.Online),

            IsEmployerLocationAvailable = source.Locations.Any(x => x.LocationType == LocationType.National || x.LocationType == LocationType.Regional),
            NearestEmployerLocation = source.Locations.Where(x => x.LocationType == LocationType.National || x.LocationType == LocationType.Regional).MinBy(x => x.CourseDistance)?.CourseDistance,

            IsProviderAvailable = source.Locations.Any(x => x.LocationType == LocationType.Provider),
            NearestProviderPlace = source.Locations.Where(x => x.LocationType == LocationType.Provider).MinBy(x => x.CourseDistance)?.CourseDistance,

            IsBlockReleaseAvailable = source.Locations.Any(x => x.BlockRelease),
            IsDayReleaseAvailable = source.Locations.Any(l => l.DayRelease),
            IsBlockReleaseMultiple = source.Locations.Count(x => x.BlockRelease) > 1,
            IsDayReleaseMultiple = source.Locations.Count(x => x.DayRelease) > 1,
            NearestBlockRelease = source.Locations.Where(x => x.BlockRelease).MinBy(x => x.CourseDistance)?.CourseDistance,
            NearestDayRelease = source.Locations.Where(x => x.DayRelease).MinBy(x => x.CourseDistance)?.CourseDistance
        };
    }
}
