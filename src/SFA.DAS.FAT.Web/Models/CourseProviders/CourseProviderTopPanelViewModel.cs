using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class CourseProviderTopPanelViewModel
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public int ShortlistCount { get; set; }
    public string Location { get; set; }
    public string CourseId { get; set; }
    public string Distance { get; set; }

    public Dictionary<string, string> ProviderRouteData
    {
        get
        {
            var distance = string.Empty;
            if (!string.IsNullOrEmpty(Distance) && Distance != DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
            {
                distance = Distance;
            }

            var providerRouteData = new Dictionary<string, string>
            {
                { "location", Location },
                { "id", CourseId },
                { "providerId", Ukprn.ToString() },
                { "distance", distance }
            };

            return providerRouteData;
        }
    }

}
