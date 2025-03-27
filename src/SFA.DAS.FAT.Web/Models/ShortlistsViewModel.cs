using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Shortlist;

namespace SFA.DAS.FAT.Web.Models;

public class ShortlistsViewModel
{
    public List<ShortlistItemViewModel> ShortlistedItems { get; set; } = [];
    public string Removed { get; set; }
    public string ExpiryDateText { get; set; }


    public bool IsOneTable => OneTable();
    private bool OneTable()
    {
        var distinctCourseTitles = ShortlistedItems
            .GroupBy(model => model.Course.Title)
            .Select(models => models.Key)
            .ToList();

        if (distinctCourseTitles.Count > 1)
            return false;

        var distinctCourseLevels = ShortlistedItems
            .GroupBy(model => model.Course.Level)
            .Select(models => models.Key)
            .ToList();

        if (distinctCourseLevels.Count > 1)
            return false;

        var distinctLocations = ShortlistedItems
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

    public string TitleAndLevel { get => Course.TitleAndLevel; }

    public bool CanGetHelpFindingCourse(FindApprenticeshipTrainingWeb config)
    {
        return Course.CanGetHelpFindingCourse(config);
    }

    public string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config)
    {
        return Course.GetHelpFindingCourseUrl(config, EntryPoint.Shortlist, LocationDescription);
    }

    public static implicit operator ShortlistItemViewModel(ShortlistItem source)
    {
        return new ShortlistItemViewModel
        {
            Id = source.Id,
            LocationDescription = source.LocationDescription,
            CreatedDate = source.CreatedDate,
            Course = source.Course,
            Provider = source.Provider,
        };
    }
}
