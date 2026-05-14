using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class CourseViewModel : PageLinksViewModelBase
{
    public IOptions<FindApprenticeshipTrainingWeb> ConfigOptions { get; set; }

    public const string KnowledgeSkillsHeaderText = "Knowledge, skills and behaviours";
    public const string KnowledgeSkillsHeaderTextApprenticeshipUnit = "Knowledge and skills learners will gain";
    public const string KnowledgeSkillsLinkText = "View knowledge, skills and behaviours";
    public const string KnowledgeSkillsLinkTextApprenticeshipUnit = "View knowledge and skills";
    public const string MaximumFundingText = "apprenticeship training and assessment costs.";
    public const string MaximumFundingTextApprenticeshipUnit = "apprenticeship unit training and assessment costs.";

    public const string ZeroProvidersWithinDistanceMessage = "There are no training providers who offer this course. Remove location or select View providers for this course to increase how far the learner can travel.";
    public const string SingleProviderWithinDistanceMessage = "There is 1 training provider who offers this course.";
    public const string MultipleProvidersWithinDistanceMessage = "There are {{ProvidersCountWithinDistance}} training providers who offer this course.";
    public const string SingleProviderOutsideDistanceMessage = "There is 1 training provider who offers this course. Check if a training provider can deliver this training in the learner's work location.";
    public const string MultipleProviderOutsideDistanceMessage = "There are {{TotalProvidersCount}} training providers who offer this course. Check if a training provider can deliver this training in the learner's work location.";

    public string StandardUId { get; set; }
    public string IFateReferenceNumber { get; set; }
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
    public string MaxFundingDisplayValue => MaxFunding.ToString("C0", new CultureInfo("en-GB"));
    public int TypicalDuration { get; set; }
    public string TypicalJobTitles { get; set; }
    public string StandardPageUrl { get; set; }
    public LearningType LearningType { get; set; }
    public bool IsActiveAvailable { get; set; }
    public string LearningTypeTagClass => LearningType.GetTagClass();
    public bool IsApprenticeship { get; set; }
    public bool IsFoundationApprenticeship { get; set; }
    public bool IsApprenticeshipUnit { get; set; }
    public bool IsShortCourseType { get; set; }
    public int IncentivePayment { get; set; }
    public string IncentivePaymentDisplayValue => IncentivePayment.ToString("C0", new CultureInfo("en-GB"));
    public List<Level> Levels { get; set; } = [];
    public IEnumerable<KsbGroup> KsbDetails { get; set; } = [];
    public List<RelatedOccupation> RelatedOccupations { get; set; } = [];

    public static implicit operator CourseViewModel(GetCourseQueryResult source)
    {
        var learningType = source.ApprenticeshipType;

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
            ShowShortListLink = true,
            ShowApprenticeTrainingCoursesCrumb = true,
            IsApprenticeship = learningType == LearningType.Apprenticeship,
            IsFoundationApprenticeship = learningType == LearningType.FoundationApprenticeship,
            IsApprenticeshipUnit = learningType == LearningType.ApprenticeshipUnit,
            LearningType = learningType,

            IsActiveAvailable = source.IsActiveAvailable,
            IncentivePayment = source.IncentivePayment,
            RelatedOccupations = source.RelatedOccupations,
            IsShortCourseType = source.CourseType == CourseType.ShortCourse
        };
    }
    public string LevelEquivalentToDisplayText => GetLevelEquivalentToDisplayText();
    public string KnowledgeSkillsHeaderTextToDisplay => GetKnowledgeSkillsHeaderTextToDisplay();
    public string KnowledgeSkillsLinkTextToDisplay => GetKnowledgeSkillsLinkTextToDisplay();
    public string MaximumFundingTextToDisplay => GetMaximumFundingTextToDisplay();
    public string[] TypicalJobTitlesArray => GetTypicalJobTitles();
    public string HelpFindingCourseUrl => GetHelpFindingCourseUrl(ConfigOptions.Value);
    public string ProviderCountDisplayMessage => GetProviderCountDisplayMessage();
    public string ApprenticeCanTravelDisplayMessage => GetApprenticeCanTravelDisplayMessage();
    public bool HasLocation => !string.IsNullOrWhiteSpace(Location);
    private string GetLevelEquivalentToDisplayText()
    {
        if (Levels.Count == 0)
        {
            return string.Empty;
        }

        var equivalentLevel = Levels.Find(a => a.Code == Level);

        return equivalentLevel is null ? string.Empty : $"Equal to {equivalentLevel.Name}";
    }
    private string[] GetTypicalJobTitles()
    {
        if (string.IsNullOrWhiteSpace(TypicalJobTitles))
        {
            return Array.Empty<string>();
        }

        return TypicalJobTitles.Split('|');
    }
    private string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config)
    {
        var redirectUri = $"{config.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={LarsCode}&requestType={EntryPoint.CourseDetail}";
        var locationQueryParam = HasLocation ? $"&location={Location}" : string.Empty;

        return $"{config.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    private string GetKnowledgeSkillsHeaderTextToDisplay() =>
        IsApprenticeshipUnit ? KnowledgeSkillsHeaderTextApprenticeshipUnit : KnowledgeSkillsHeaderText;

    private string GetKnowledgeSkillsLinkTextToDisplay() =>
        IsApprenticeshipUnit ? KnowledgeSkillsLinkTextApprenticeshipUnit : KnowledgeSkillsLinkText;

    private string GetMaximumFundingTextToDisplay() =>
        IsApprenticeshipUnit ? MaximumFundingTextApprenticeshipUnit : MaximumFundingText;

    private string GetProviderCountDisplayMessage()
    {
        if (!HasLocation)
        {
            return TotalProvidersCount == 1
                ? SingleProviderOutsideDistanceMessage
                : MultipleProviderOutsideDistanceMessage.Replace("{{TotalProvidersCount}}", TotalProvidersCount.ToString());
        }

        return ProvidersCountWithinDistance switch
        {
            0 => ZeroProvidersWithinDistanceMessage,
            1 => SingleProviderWithinDistanceMessage,
            _ => MultipleProvidersWithinDistanceMessage.Replace("{{ProvidersCountWithinDistance}}", ProvidersCountWithinDistance.ToString())
        };
    }
    private string GetApprenticeCanTravelDisplayMessage() =>
        Distance == DistanceService.AcrossEnglandFilterValue
            ? DistanceService.AcrossEnglandDisplayText
            : $"{Distance} miles";

    private static List<KsbGroup> KsbsGroupsOrdered(List<Ksb> ksbs)
    {
        var ksbsOrdered = new List<KsbGroup>();

        var ksbGroups = ksbs.GroupBy(x => x.Type)
            .Select(c => new KsbGroup { Type = c.Key, Details = c.Select(x => x.Detail).ToList() }).ToList();

        var orderedTypes = new[]
        {
            KsbType.Knowledge,
            KsbType.TechnicalKnowledge,
            KsbType.Skill,
            KsbType.TechnicalSkill,
            KsbType.Behaviour,
            KsbType.EmployabilitySkillsAndBehaviour
        };

        foreach (var ksbType in orderedTypes)
        {
            ksbsOrdered.AddRange(ksbGroups.Where(x => x.Type == ksbType));
        }

        return ksbsOrdered;
    }
}
