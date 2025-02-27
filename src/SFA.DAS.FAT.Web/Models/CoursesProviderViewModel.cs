using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class CoursesProviderViewModel
{
    public int Ukprn { get; set; }
    public long Ordering { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public List<ProviderLocation> Locations { get; set; }

    public bool IsEmployerLocationAvailable
    {
        get => Locations.Any(x => x.AtEmployer);
    }

    public decimal? NearestEmployerLocation
    {
        get => Locations.FirstOrDefault(x => x.AtEmployer)?.CourseDistance;
    }

    public bool IsBlockReleaseAvailable
    {
        get => Locations.Any(x => x.BlockRelease);
    }

    public bool IsDayReleaseAvailable
    {
        get => Locations.Any(l => l.DayRelease);
    }

    public bool IsBlockReleaseMultiple
    {
        get => Locations.Count(x => x.BlockRelease) > 1;
    }

    public bool IsDayReleaseMultiple
    {
        get => Locations.Count(x => x.DayRelease) > 1;
    }

    public decimal? NearestBlockRelease
    {
        get => Locations.FirstOrDefault(x => x.BlockRelease)?.CourseDistance;
    }

    public decimal? NearestDayRelease
    {
        get => Locations.FirstOrDefault(x => x.DayRelease)?.CourseDistance;
    }

    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }

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
            ApprenticeRating = source.ApprenticeRating
        };
    }
}
