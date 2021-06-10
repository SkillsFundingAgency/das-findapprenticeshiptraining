using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Shortlist;

namespace SFA.DAS.FAT.Web.Models
{
    public class ShortlistViewModel
    {
        private const int ShortlistExpiryInDays = 30;
        public List<ShortlistItemViewModel> Shortlist { get; set; } = new List<ShortlistItemViewModel>();

        public bool IsOneTable => OneTable();
        public string Removed { get; set; }

        public string ExpiryDateText => GetExpiryDateText();
        public string HelpBaseUrl { get ; set ; }

        private string GetExpiryDateText()
        {
            if (!Shortlist.Any())
            {
                return "";
            }

            var dateToUse = Shortlist.Select(c => c.CreatedDate).Max();

            return $"We will save your shortlist until {dateToUse.AddDays(ShortlistExpiryInDays):dd MMMM yyyy}.";
        }

        private bool OneTable()
        {
            var distinctCourseTitles = Shortlist
                .GroupBy(model => model.Course.Title)
                .Select(models => models.Key)
                .ToList();

            if (distinctCourseTitles.Count > 1)
                return false;

            var distinctCourseLevels = Shortlist
                .GroupBy(model => model.Course.Level)
                .Select(models => models.Key)
                .ToList();

            if (distinctCourseLevels.Count > 1)
                return false;

            var distinctLocations = Shortlist
                .GroupBy(model => model.LocationDescription)
                .Select(models => models.Key)
                .ToList();

            if (distinctLocations.Count > 1)
                return false;

            return true;
        }
    }

    public class ShortlistItemViewModel
    {
        public Guid Id { get; set; }
        public ProviderViewModel Provider { get; set; }
        public CourseViewModel Course { get; set; }
        public string LocationDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string HelpFindingCourseUrl { get ; set ; }
        public static implicit operator ShortlistItemViewModel(ShortlistItem source)
        {
            return new ShortlistItemViewModel
            {
                Id = source.Id,
                LocationDescription = source.LocationDescription,
                CreatedDate = source.CreatedDate,
                Course = source.Course,
                Provider = source.Provider,
                HelpFindingCourseUrl = $"/registerdemand/course/{source.Course.Id}/share-interest"
            };
        }
    }
}
