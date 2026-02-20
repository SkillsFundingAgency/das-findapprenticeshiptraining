using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseViewModelTests;

public class WhenBuildingCourseViewModelFromQueryResult
{
    [Test, MoqAutoData]
    public void Then_The_Model_Is_Converted_From_Result_Correctly(GetCourseQueryResult source, ApprenticeshipType apprenticeshipType)
    {
        source.ApprenticeshipType = apprenticeshipType;
        var isFoundationApprenticeship = apprenticeshipType == ApprenticeshipType.FoundationApprenticeship;

        var sut = (CourseViewModel)source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.StandardUId, Is.EqualTo(source.StandardUId));
            Assert.That(sut.IFateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
            Assert.That(sut.LarsCode, Is.EqualTo(source.LarsCode));
            Assert.That(sut.ProvidersCountWithinDistance, Is.EqualTo(source.ProvidersCountWithinDistance));
            Assert.That(sut.TotalProvidersCount, Is.EqualTo(source.TotalProvidersCount));
            Assert.That(sut.Title, Is.EqualTo(source.Title));
            Assert.That(sut.Level, Is.EqualTo(source.Level));
            Assert.That(sut.TitleAndLevel, Is.EqualTo($"{source.Title} (level {source.Level})"));
            Assert.That(sut.Version, Is.EqualTo(source.Version));
            Assert.That(sut.OverviewOfRole, Is.EqualTo(source.OverviewOfRole));
            Assert.That(sut.Route, Is.EqualTo(source.Route));
            Assert.That(sut.RouteCode, Is.EqualTo(source.RouteCode));
            Assert.That(sut.MaxFunding, Is.EqualTo(source.MaxFunding));
            Assert.That(sut.TypicalDuration, Is.EqualTo(source.TypicalDuration));
            Assert.That(sut.TypicalJobTitles, Is.EqualTo(source.TypicalJobTitles));
            Assert.That(sut.StandardPageUrl, Is.EqualTo(source.StandardPageUrl));
            Assert.That(sut.IsFoundationApprenticeship, Is.EqualTo(isFoundationApprenticeship));
            Assert.That(sut.Levels, Is.EqualTo(source.Levels));
            Assert.That(sut.LarsCode, Is.EqualTo(source.LarsCode));
            Assert.That(sut.ShowShortListLink, Is.True);
            Assert.That(sut.ShowApprenticeTrainingCoursesCrumb, Is.True);
            Assert.That(sut.ApprenticeshipType, Is.EqualTo(source.ApprenticeshipType));
        });
    }

    [Test, MoqAutoData]
    public void Then_The_Model_Has_No_KsbDetails_When_Query_Result_Has_None(GetCourseQueryResult source)
    {
        source.Ksbs = new List<Ksb>();

        var sut = (CourseViewModel)source;
        sut.KsbDetails.Should().BeEquivalentTo(new List<KsbGroup>());

    }

    [Test, AutoData]
    public void Then_The_Model_Has_Ksbs_Grouped_By_Type()
    {
        GetCourseQueryResult source = new GetCourseQueryResult();

        source.Ksbs = [
            new() { Type = KsbType.Knowledge, Detail = "Knowledge 1"},
            new() { Type = KsbType.Skill, Detail = "Skill 1"},
            new() { Type = KsbType.Skill, Detail = "Skill 2"},
            new() { Type = KsbType.EmployabilitySkillsAndBehaviour, Detail = "EmployabilitySkillsAndBehaviour 1"},
            new() { Type = KsbType.EmployabilitySkillsAndBehaviour, Detail = "EmployabilitySkillsAndBehaviour 2"},
            new() { Type = KsbType.EmployabilitySkillsAndBehaviour, Detail = "EmployabilitySkillsAndBehaviour 3"}
        ];

        var sut = (CourseViewModel)source;

        using (new AssertionScope())
        {
            sut.KsbDetails.Should().HaveCount(3);
            sut.KsbDetails.Should().ContainSingle(x => x.Type == KsbType.Knowledge);
            sut.KsbDetails.Should().ContainSingle(x => x.Type == KsbType.Skill);
            sut.KsbDetails.Should().ContainSingle(x => x.Type == KsbType.EmployabilitySkillsAndBehaviour);
            sut.KsbDetails.First(x => x.Type == KsbType.Knowledge).Details.Should().BeEquivalentTo("Knowledge 1");
            sut.KsbDetails.First(x => x.Type == KsbType.Skill).Details.Should().BeEquivalentTo("Skill 1", "Skill 2");
            sut.KsbDetails.First(x => x.Type == KsbType.EmployabilitySkillsAndBehaviour).Details.Should().BeEquivalentTo("EmployabilitySkillsAndBehaviour 1", "EmployabilitySkillsAndBehaviour 2", "EmployabilitySkillsAndBehaviour 3");
        }
    }

    [Test, AutoData]
    public void Then_The_Model_Has_Empty_Ksbs()
    {
        GetCourseQueryResult source = new GetCourseQueryResult();

        source.Ksbs = [];

        var sut = (CourseViewModel)source;

        sut.KsbDetails.Should().BeEmpty();
    }

    [Test]
    public void Then_The_Model_Has_KsbDetails_Ordered_As_Expected()
    {
        GetCourseQueryResult source = new GetCourseQueryResult();

        var detailSkill = Guid.NewGuid().ToString();
        var detailSkill2 = Guid.NewGuid().ToString();
        var detailBehaviour = Guid.NewGuid().ToString();
        var detailEmployability = Guid.NewGuid().ToString();
        var detailKnowledge = Guid.NewGuid().ToString();
        var detailTechnicalKnowledge = Guid.NewGuid().ToString();
        var detailTechnicalSkill = Guid.NewGuid().ToString();

        source.Ksbs = new List<Ksb> {
         new() { Type = KsbType.Skill, Detail =detailSkill},
         new() { Type = KsbType.Behaviour, Detail = detailBehaviour},
         new() { Type = KsbType.EmployabilitySkillsAndBehaviour, Detail = detailEmployability},
         new() { Type = KsbType.Knowledge, Detail = detailKnowledge},
         new() { Type = KsbType.TechnicalKnowledge, Detail = detailTechnicalKnowledge},
         new() { Type = KsbType.TechnicalSkill, Detail = detailTechnicalSkill},
         new() { Type = KsbType.Skill, Detail =detailSkill2},
     };

        var expectedKsbs = new List<KsbGroup>
     {
         new() { Type = KsbType.Knowledge, Details =  [detailKnowledge] },
         new() { Type = KsbType.TechnicalKnowledge, Details = [ detailTechnicalKnowledge ] },
         new() { Type = KsbType.Skill, Details =[ detailSkill, detailSkill2 ] },
         new() { Type = KsbType.TechnicalSkill, Details = [detailTechnicalSkill ]},
         new() { Type = KsbType.Behaviour, Details = [detailBehaviour ] },
         new() { Type = KsbType.EmployabilitySkillsAndBehaviour, Details = [ detailEmployability ]},
     };

        var sut = (CourseViewModel)source;
        sut.KsbDetails.Should().BeEquivalentTo(expectedKsbs);
    }
}
