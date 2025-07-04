﻿using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class CourseViewModel : PageLinksViewModelBase
{
    public string StandardUId { get; set; }
    public string IFateReferenceNumber { get; set; }
    public int LarsCode { get; set; }
    public int ProvidersCountWithinDistance { get; set; }
    public int TotalProvidersCount { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public string TitleAndLevel { get; set; }
    public string Version { get; set; }
    public string OverviewOfRole { get; set; }
    public string Route { get; set; }
    public int RouteCode { get; set; }
    public int MaxFunding { get; set; }
    public int TypicalDuration { get; set; }
    public string TypicalJobTitles { get; set; }
    public string StandardPageUrl { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public bool IsFoundationApprenticeship { get; set; }

    public int IncentivePayment { get; set; }

    public List<Level> Levels { get; set; } = [];

    public IEnumerable<KsbGroup> KsbDetails { get; set; }
    public List<RelatedOccupation> RelatedOccupations { get; set; }

    public static implicit operator CourseViewModel(GetCourseQueryResult source)
    {
        return new CourseViewModel
        {
            StandardUId = source.StandardUId,
            IFateReferenceNumber = source.IFateReferenceNumber,
            LarsCode = source.LarsCode,
            ProvidersCountWithinDistance = source.ProvidersCountWithinDistance,
            TotalProvidersCount = source.TotalProvidersCount,
            Title = source.Title,
            Level = source.Level,
            TitleAndLevel = $"{source.Title} (level {source.Level})",
            Version = source.Version,
            OverviewOfRole = source.OverviewOfRole,
            Route = source.Route,
            RouteCode = source.RouteCode,
            MaxFunding = source.MaxFunding,
            TypicalDuration = source.TypicalDuration,
            TypicalJobTitles = source.TypicalJobTitles,
            StandardPageUrl = source.StandardPageUrl,
            KsbDetails = KsbsGroupsOrdered(source.Ksbs),
            Levels = source.Levels,
            CourseId = source.LarsCode,
            ShowShortListLink = true,
            ShowApprenticeTrainingCoursesCrumb = true,
            IsFoundationApprenticeship = source.ApprenticeshipType == ApprenticeshipType.FoundationApprenticeship,
            ApprenticeshipType = source.ApprenticeshipType == ApprenticeshipType.FoundationApprenticeship ? ApprenticeshipType.FoundationApprenticeship : ApprenticeshipType.Apprenticeship,
            IncentivePayment = source.IncentivePayment,
            RelatedOccupations = source.RelatedOccupations
        };
    }

    public string GetLevelEquivalentToDisplayText()
    {
        if (Levels.Count < 1)
        {
            return string.Empty;
        }

        Level EquivalentLevel = Levels.Find(a => a.Code == Level);

        return EquivalentLevel is null ? string.Empty : $"Equal to {EquivalentLevel.Name}";
    }

    public string[] GetTypicalJobTitles()
    {
        if (string.IsNullOrWhiteSpace(TypicalJobTitles))
        {
            return [];
        }

        return TypicalJobTitles.Split('|');
    }

    public string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config)
    {
        string redirectUri = $"{config.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={LarsCode}&requestType={EntryPoint.CourseDetail}";

        var locationQueryParam = !string.IsNullOrEmpty(Location) ? $"&location={Location}" : string.Empty;

        return $"{config.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    public const string ZERO_PROVIDERS_WITHIN_DISTANCE_MESSAGE = "There are no training providers who offer this course. Remove location or select View providers for this course to increase how far the apprentice can travel.";

    public const string SINGLE_PROVIDER_WITHIN_DISTANCE_MESSAGE = "There is 1 training provider who offers this course.";

    public const string MULTPLE_PROVIDERS_WITHIN_DISTANCE_MESSAGE = "There are {{ProvidersCountWithinDistance}} training providers who offer this course.";

    public const string SINGLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE = "There is 1 training provider who offers this course. Check if a training provider can deliver this training in the apprentice's work location.";

    public const string MULTIPLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE = "There are {{TotalProvidersCount}} training providers who offer this course. Check if a training provider can deliver this training in the apprentice's work location.";

    public string GetProviderCountDisplayMessage()
    {
        if (HasLocation)
        {
            return ProvidersCountWithinDistance switch
            {
                0 => ZERO_PROVIDERS_WITHIN_DISTANCE_MESSAGE,
                1 => SINGLE_PROVIDER_WITHIN_DISTANCE_MESSAGE,
                _ => MULTPLE_PROVIDERS_WITHIN_DISTANCE_MESSAGE.Replace("{{ProvidersCountWithinDistance}}", ProvidersCountWithinDistance.ToString())
            };
        }

        return TotalProvidersCount == 1
            ? SINGLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE
            : MULTIPLE_PROVIDER_OUTSIDE_DISTANCE_MESSAGE.Replace("{{TotalProvidersCount}}", TotalProvidersCount.ToString());
    }

    public string GetApprenticeCanTravelDisplayMessage()
    {
        if (Distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE)
        {
            return DistanceService.ACROSS_ENGLAND_DISPLAY_TEXT;
        }

        return $"{Distance} miles";
    }

    public bool HasLocation => !string.IsNullOrWhiteSpace(Location);

    private static List<KsbGroup> KsbsGroupsOrdered(List<Ksb> ksbs)
    {
        List<KsbGroup> ksbsOrdered = new List<KsbGroup>();

        var ksbGroups = ksbs.GroupBy(x => x.Type)
            .Select(c => new KsbGroup { Type = c.Key, Details = c.Select(x => x.Detail).ToList() }).ToList();

        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.Knowledge));
        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.TechnicalKnowledge));
        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.Skill));
        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.TechnicalSkill));
        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.Behaviour));
        ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == KsbType.EmployabilitySkillsAndBehaviour));
        return ksbsOrdered;
    }
}
