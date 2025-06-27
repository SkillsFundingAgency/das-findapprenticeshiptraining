using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses;

public enum KsbType
{
    [Description("Knowledge")]
    Knowledge = 1,
    [Description("Skills")]
    Skill = 2,
    [Description("Behaviours")]
    Behaviour = 3,
    [Description("Technical knowledge")]
    TechnicalKnowledge = 4,
    [Description("Technical skills")]
    TechnicalSkill = 5,
    [Description("Employability skills and behaviours")]
    EmployabilitySkillsAndBehaviour = 6
}
