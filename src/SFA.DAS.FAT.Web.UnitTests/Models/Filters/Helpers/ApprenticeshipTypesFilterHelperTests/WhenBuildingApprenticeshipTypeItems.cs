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
    public void Then_Returns_Empty_List_When_Types_Is_Null()
    {
        var items = ApprenticeshipTypesFilterHelper.BuildItems(null, null);

        items.Should().NotBeNull();
        items.Should().BeEmpty();
    }

    [Test]
    public void Then_Returns_Empty_List_When_Types_Is_Empty()
    {
        var items = ApprenticeshipTypesFilterHelper.BuildItems([], []);

        items.Should().NotBeNull();
        items.Should().BeEmpty();
    }

    [Test]
    public void Then_Sets_Selected_Correctly_When_SelectedTypes_Is_Null()
    {
        var types = new List<TypeViewModel>
        {
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.ApprenticeshipUnits.ToString(),
                Name = ApprenticeshipType.ApprenticeshipUnits.GetDescription()
            }, null)
        };

        var items = ApprenticeshipTypesFilterHelper.BuildItems(types, null);

        items.Should().HaveCount(1);
        items[0].Selected.Should().BeFalse();
    }

    [Test]
    public void Then_Maps_Descriptions_For_All_Apprenticeship_Types()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.FoundationApprenticeship.GetDescription()
        };

        var types = new List<TypeViewModel>
        {
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.ApprenticeshipUnits.ToString(),
                Name = ApprenticeshipType.ApprenticeshipUnits.GetDescription()
            }, selectedTypes),
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
            }, selectedTypes),
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.Apprenticeship.ToString(),
                Name = ApprenticeshipType.Apprenticeship.GetDescription()
            }, selectedTypes)
        };

        var items = ApprenticeshipTypesFilterHelper.BuildItems(types, selectedTypes);

        items.Should().HaveCount(3);

        var units = items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnits.GetDescription());
        var foundation = items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(units.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_UNITS_DESCRIPTION));
            Assert.That(foundation.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_FOUNDATION_DESCRIPTION));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION));
        });
    }

    [Test]
    public void Then_Sets_Selected_Based_On_SelectedTypes_Match()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.ApprenticeshipUnits.GetDescription(),
            ApprenticeshipType.Apprenticeship.GetDescription()
        };

        var types = new List<TypeViewModel>
        {
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.ApprenticeshipUnits.ToString(),
                Name = ApprenticeshipType.ApprenticeshipUnits.GetDescription()
            }, selectedTypes),
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
            }, selectedTypes),
            new TypeViewModel(new ApprenticeType
            {
                Code = ApprenticeshipType.Apprenticeship.ToString(),
                Name = ApprenticeshipType.Apprenticeship.GetDescription()
            }, selectedTypes)
        };

        var items = ApprenticeshipTypesFilterHelper.BuildItems(types, selectedTypes);

        var units = items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnits.GetDescription());
        var foundation = items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(units.Selected, Is.True);
            Assert.That(foundation.Selected, Is.False);
            Assert.That(apprenticeship.Selected, Is.True);
        });
    }
}
