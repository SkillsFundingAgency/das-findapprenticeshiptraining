using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.CourseProviders;

public class ProviderData
{
    public long Ordering { get; set; }
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public List<ProviderLocation> Locations { get; set; }
    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }
}
