using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.SearchCoursesViewModelTests;

public sealed class WhenBuildingApprenticeshipTypesFilter
{
    [Test]
    public void TrainingTypesCheckboxListItems_WithSelectedTypes_ConfiguredCorrectly()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.FoundationApprenticeship.GetDescription()
        };

        var sut = new SearchCoursesViewModel
        {
            CourseTerm = "engineering",
            SelectedTypes = selectedTypes,
            TrainingTypesFilterItems = ApprenticeshipTypesFilterHelper.BuildItems(selectedTypes)
        };

        var filter = sut.TrainingTypesCheckboxListItems;

        var unit = filter.Items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnit.GetDescription());
        var foundation = filter.Items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = filter.Items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Id, Is.EqualTo("training-types"));
            Assert.That(filter.Heading, Is.EqualTo("Training type"));
            Assert.That(filter.For, Is.EqualTo(nameof(SearchCoursesViewModel.SelectedTypes)));
            Assert.That(filter.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));
            Assert.That(filter.Link, Is.Null);

            Assert.That(filter.Items, Has.Count.EqualTo(3));
            Assert.That(filter.Items.All(i => i.IsApprenticeshipTypeEmphasised), Is.False);

            Assert.That(unit.DisplayText, Is.EqualTo(ApprenticeshipType.ApprenticeshipUnit.GetDescription()));
            Assert.That(foundation.DisplayText, Is.EqualTo(ApprenticeshipType.FoundationApprenticeship.GetDescription()));
            Assert.That(apprenticeship.DisplayText, Is.EqualTo(ApprenticeshipType.Apprenticeship.GetDescription()));

            Assert.That(unit.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION));
            Assert.That(foundation.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION));

            Assert.That(unit.IsSelected, Is.False);
            Assert.That(foundation.IsSelected, Is.True);
            Assert.That(apprenticeship.IsSelected, Is.False);
        });
    }

    [Test]
    public void TrainingTypesCheckboxListItems_WithSelectedTypes_ReflectsSelection()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.Apprenticeship.GetDescription()
        };

        var sut = new SearchCoursesViewModel
        {
            SelectedTypes = selectedTypes,
            TrainingTypesFilterItems = ApprenticeshipTypesFilterHelper.BuildItems(selectedTypes)
        };

        var filter = sut.TrainingTypesCheckboxListItems;

        var unit = filter.Items.First(i => i.DisplayText == ApprenticeshipType.ApprenticeshipUnit.GetDescription());
        var foundation = filter.Items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = filter.Items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(unit.IsSelected, Is.False);
            Assert.That(foundation.IsSelected, Is.False);
            Assert.That(apprenticeship.IsSelected, Is.True);
        });
    }

    [Test]
    public void TrainingTypesCheckboxListItems_WithEmptySelectedTypes_AreUnselected()
    {
        var sut = new SearchCoursesViewModel
        {
            TrainingTypesFilterItems = ApprenticeshipTypesFilterHelper.BuildItems([])
        };

        var filter = sut.TrainingTypesCheckboxListItems;
        Assert.Multiple(() =>
        {
            Assert.That(filter.Items, Has.Count.EqualTo(3));
            Assert.That(filter.Items.All(i => i.IsSelected), Is.False);
            Assert.That(filter.Items.All(i => i.IsApprenticeshipTypeEmphasised), Is.False);
        });
    }
}
