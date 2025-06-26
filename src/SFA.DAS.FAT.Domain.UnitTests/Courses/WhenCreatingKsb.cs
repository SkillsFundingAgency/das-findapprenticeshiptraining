using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses;
public class WhenCreatingKsbFromResponse
{
    [TestCase(KsbType.Skill)]
    [TestCase(KsbType.TechnicalSkill)]
    [TestCase(KsbType.Knowledge)]
    [TestCase(KsbType.TechnicalKnowledge)]
    [TestCase(KsbType.Behaviour)]
    [TestCase(KsbType.EmployabilitySkillsAndBehaviour)]
    public void Then_Type_Is_Correctly_Mapped(KsbType ksbType)
    {
        KsbResponse standard = new KsbResponse { Type = ksbType.ToString() };
        var sut = (Ksb)standard;
        sut.Type.Should().Be(ksbType);
    }
}
