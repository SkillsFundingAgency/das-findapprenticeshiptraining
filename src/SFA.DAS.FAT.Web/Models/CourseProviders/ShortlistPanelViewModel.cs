using System;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class ShortlistPanelViewModel
{
    public string ShortlistClass { get; set; }
    public string LarsCode { get; set; }
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public string Location { get; set; }
    public Guid? ShortlistId { get; set; }
    public bool ShowMultipleProvidersForCourse { get; set; }
    public int TotalProvidersCount { get; set; }
    public string CourseNameAndLevel { get; set; }
}
