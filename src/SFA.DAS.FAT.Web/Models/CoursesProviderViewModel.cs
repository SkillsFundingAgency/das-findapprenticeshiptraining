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

    public bool IsBlockReleaseAvailable
    {
        get => Locations.Any(x => x.BlockRelease);
    }

    public bool IsDayReleaseAvailable
    {
        get => Locations.Any(l => l.DayRelease);
    }

    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public EmployerProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ApprenticeProviderRating ApprenticeRating { get; set; }

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
