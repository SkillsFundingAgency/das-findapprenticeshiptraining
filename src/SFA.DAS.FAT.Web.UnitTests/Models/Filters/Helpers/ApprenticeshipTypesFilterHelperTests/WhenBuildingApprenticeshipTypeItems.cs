using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Filters.Helpers.ApprenticeshipTypesFilterHelperTests;

public sealed class WhenBuildingApprenticeshipTypeItems
{
    [Test]
    public void Then_Returns_All_Types_When_SelectedTypes_Is_Null()
    {
        var items = ApprenticeshipTypesFilterHelper.BuildItems(null);

        items.Should().NotBeNull();
        items.Should().HaveCount(3);
        items.All(i => i.IsSelected == false).Should().BeTrue();
    }

    [Test]
    public void Then_Sets_Selected_Correctly_When_SelectedTypes_Is_Null()
    {
        var items = ApprenticeshipTypesFilterHelper.BuildItems(null);

        items.Should().HaveCount(3);
        items.All(i => i.IsSelected == false).Should().BeTrue();
    }

    [Test]
    public void Then_Maps_Descriptions_For_All_Apprenticeship_Types()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.FoundationApprenticeship.GetDescription()
        };

        var items = ApprenticeshipTypesFilterHelper.BuildItems(selectedTypes);

        items.Should().HaveCount(3);

        var units = items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnit.GetDescription());
        var foundation = items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(units.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION));
            Assert.That(foundation.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION));
        });
    }

    [Test]
    public void Then_Sets_Selected_Based_On_SelectedTypes_Match()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.ApprenticeshipUnit.GetDescription(),
            ApprenticeshipType.Apprenticeship.GetDescription()
        };

        var items = ApprenticeshipTypesFilterHelper.BuildItems(selectedTypes);

        var units = items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnit.GetDescription());
        var foundation = items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(units.IsSelected, Is.True);
            Assert.That(foundation.IsSelected, Is.False);
            Assert.That(apprenticeship.IsSelected, Is.True);
        });
    }
}
