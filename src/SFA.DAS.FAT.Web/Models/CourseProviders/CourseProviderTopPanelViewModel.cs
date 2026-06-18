using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.Models.CourseProviders;

public class CourseProviderTopPanelViewModel
{
    public int Ukprn { get; set; }
    public string ProviderName { get; set; }
    public Guid? ShortlistId { get; set; }
    public int ShortlistCount { get; set; }
    public string LarsCode { get; set; }

    public Dictionary<string, string> ProviderRouteData
    {
        get
        {

            var providerRouteData = new Dictionary<string, string>
            {
                { "larsCode", LarsCode },
                { "ukprn", Ukprn.ToString() },
            };

            return providerRouteData;
        }
    }

}
