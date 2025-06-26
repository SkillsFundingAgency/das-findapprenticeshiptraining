using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.Courses;

public class Ksb
{
    public KsbType Type { get; set; }
    public string Detail { get; set; }

    public static implicit operator Ksb(KsbResponse standard)
    {
        KsbType ksbType = KsbType.Skill;

        if (standard.Type == KsbType.TechnicalSkill.ToString()) ksbType = KsbType.TechnicalSkill;
        if (standard.Type == KsbType.Knowledge.ToString()) ksbType = KsbType.Knowledge;
        if (standard.Type == KsbType.TechnicalKnowledge.ToString()) ksbType = KsbType.TechnicalKnowledge;
        if (standard.Type == KsbType.Behaviour.ToString()) ksbType = KsbType.Behaviour;
        if (standard.Type == KsbType.EmployabilitySkillsAndBehaviour.ToString()) ksbType = KsbType.EmployabilitySkillsAndBehaviour;

        return new Ksb
        {
            Type = ksbType,
            Detail = standard.Detail
        };
    }
}
