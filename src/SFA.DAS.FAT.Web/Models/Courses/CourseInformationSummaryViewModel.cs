using System.Collections.Generic;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.Courses;

public class CourseInformationSummaryViewModel
{
    public string LarsCode { get; set; }
    public string TitleAndLevel { get; set; }
    public string OverviewOfRole { get; set; }
    public bool IsFoundationApprenticeship { get; set; }
    public bool IsApprenticeshipUnit { get; set; }
    public string IncentivePaymentDisplayValue { get; set; }
    public string KnowledgeSkillsHeaderTextToDisplay { get; set; }
    public string KnowledgeSkillsLinkTextToDisplay { get; set; }
    public IEnumerable<KsbGroup> KsbDetails { get; set; } = [];
    public string Route { get; set; }
    public int Level { get; set; }
    public string LevelEquivalentToDisplayText { get; set; }
    public LearningType LearningType { get; set; }
    public int TypicalDuration { get; set; }
    public string MaxFundingDisplayValue { get; set; }
    public string MaximumFundingTextToDisplay { get; set; }
    public bool IsApprenticeship { get; set; }
    public string[] TypicalJobTitlesArray { get; set; } = [];
    public List<RelatedOccupation> RelatedOccupations { get; set; } = [];
    public string StandardPageUrl { get; set; }
}
