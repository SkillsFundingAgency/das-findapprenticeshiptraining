using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesProviderViewModel
{
    public int Ukprn { get; set; }
    public long Ordering { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public List<ProviderLocation> Locations { get; set; }

    public bool IsOnlineAvailable
    {
        get; init;
    }

    public bool IsLearnerWorkPlaceAvailable
    {
        get; init;
    }

    public decimal? NearestLearnerWorkPlace
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


    public string Location { get; set; }
    public string Distance { get; set; } = "All";

    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }

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
                Location = Location
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
                IsLearnerWorkPlaceAvailable = IsLearnerWorkPlaceAvailable,
                NearestLearnerWorkPlace = NearestLearnerWorkPlace,
                IsEmployerLocationAvailable = IsEmployerLocationAvailable,
                NearestEmployerLocation = NearestEmployerLocation,
                OnlineDisplayDescription = FilterService.DELIVERYMODES_SECTION_ONLINE_DISPLAYDESCRIPTION,
                LearnerWorkPlaceDisplayDescription = FilterService.DELIVERYMODES_SECTION_WORKPLACE_DISPLAYDESCRIPTION,
                EmployerLocationDisplayDescription = FilterService.DELIVERYMODES_SECTION_PROVIDER_DISPLAYDESCRIPTION,
                Distance = Distance,
                Location = Location
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
            IsOnlineAvailable = source.Locations.Any(x => x.LocationType == LocationType.Online),
            IsLearnerWorkPlaceAvailable = source.Locations.Any(x => x.LocationType == LocationType.National || x.LocationType == LocationType.Regional),
            NearestLearnerWorkPlace = source.Locations.Where(x => x.LocationType == LocationType.National || x.LocationType == LocationType.Regional).Min(x => x.CourseDistance),
            IsEmployerLocationAvailable = source.Locations.Any(x => x.AtEmployer),
            NearestEmployerLocation = source.Locations.FirstOrDefault(x => x.AtEmployer)?.CourseDistance,
            IsBlockReleaseAvailable = source.Locations.Any(x => x.BlockRelease),
            IsDayReleaseAvailable = source.Locations.Any(l => l.DayRelease),
            IsBlockReleaseMultiple = source.Locations.Count(x => x.BlockRelease) > 1,
            IsDayReleaseMultiple = source.Locations.Count(x => x.DayRelease) > 1,
            NearestBlockRelease = source.Locations.FirstOrDefault(x => x.BlockRelease)?.CourseDistance,
            NearestDayRelease = source.Locations.FirstOrDefault(x => x.DayRelease)?.CourseDistance
        };
    }
}
