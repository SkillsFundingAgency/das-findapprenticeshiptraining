using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Filters.Helpers.LearningTypesFilterHelperTests;

public sealed class WhenBuildingLearningTypeItems
{
    [Test]
    public void BuildItems_WithEmptySelectedTypes_ReturnsAllUnselectedTypes()
    {
        var items = LearningTypesFilterHelper.BuildItems(new List<LearningType>());

        items.Should().NotBeNull();
        items.Should().HaveCount(3);
        items.All(i => i.IsSelected).Should().BeFalse();
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public void BuildItems_WithBoldDisplayText_FlagsAllItems(bool isLearningTypeEmphasised, bool expectedBold)
    {
        var items = LearningTypesFilterHelper.BuildItems(new List<LearningType>(), isLearningTypeEmphasised);

        items.Should().HaveCount(3);
        items.All(i => i.IsLearningTypeEmphasised == expectedBold).Should().BeTrue();
    }

    [Test]
    public void BuildItems_WithSelectedTypes_MapsDescriptionsForAllTypes()
    {
        var selectedTypes = new List<LearningType>
        {
            LearningType.FoundationApprenticeship
        };

        var items = LearningTypesFilterHelper.BuildItems(selectedTypes);

        items.Should().HaveCount(3);

        var units = items.First(i => i.DisplayText == LearningType.ApprenticeshipUnit.GetDescription());
        var foundation = items.First(i => i.DisplayText == LearningType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == LearningType.Apprenticeship.GetDescription());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(units.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.ApprenticeshipUnitDescription));
            Assert.That(foundation.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.FoundationApprenticeshipDescription));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.ApprenticeshipDescription));
        }
    }

    [Test]
    public void BuildItems_WithSelectedTypes_SetsSelectionBasedOnMatch()
    {
        var selectedTypes = new List<LearningType>
        {
            LearningType.ApprenticeshipUnit,
            LearningType.Apprenticeship
        };

        var items = LearningTypesFilterHelper.BuildItems(selectedTypes);

        var apprenticeshipUnit = items.First(i => i.DisplayText == LearningType.ApprenticeshipUnit.GetDescription());
        var foundationApprenticeship = items.First(i => i.DisplayText == LearningType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == LearningType.Apprenticeship.GetDescription());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(apprenticeshipUnit.IsSelected, Is.True);
            Assert.That(foundationApprenticeship.IsSelected, Is.False);
            Assert.That(apprenticeship.IsSelected, Is.True);
        }
    }
}
