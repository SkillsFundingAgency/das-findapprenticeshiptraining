﻿using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class CourseProvidersViewModel
    {
        public IEnumerable<ProviderViewModel> Providers { get; set; }
        public CourseViewModel Course { get; set; }
        public int Total { get; set; }
        public string TotalMessage => Total == 1 ? $"{Total} result" : $"{Total} results";
        public string Location { get; set; }
        public ProviderSortBy SortOrder { get; set; }
        public bool HasLocations => !string.IsNullOrWhiteSpace(Location);
        public string BuildSortLink()
        {
            var newOrder = SortOrder == ProviderSortBy.Distance ? 
                ProviderSortBy.Name : ProviderSortBy.Distance;

            return $"?location={Location}&sortorder={newOrder}";
        }

        public IEnumerable<DeliveryModeOptionViewModel> DeliveryModes { get; set; }
    }
}
