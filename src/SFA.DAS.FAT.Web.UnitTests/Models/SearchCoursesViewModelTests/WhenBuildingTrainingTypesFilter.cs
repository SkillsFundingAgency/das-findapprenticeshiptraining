using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.SearchCoursesViewModelTests;

public sealed class WhenBuildingTrainingTypesFilter
{
    [Test]
    public void TrainingTypesCheckboxListItems_WithSelectedTypes_ConfiguredCorrectly()
    {
        var selectedTypes = new List<LearningType>
        {
            LearningType.FoundationApprenticeship
        };

        var sut = new SearchCoursesViewModel
        {
            CourseTerm = "engineering",
            SelectedTypes = selectedTypes,
            TrainingTypesFilterItems = LearningTypesFilterHelper.BuildItems(selectedTypes)
        };

        var filter = sut.TrainingTypesCheckboxListItems;

        var apprenticeshipUnit = filter.Items.First(i => i.DisplayText == LearningType.ApprenticeshipUnit.GetDescription());
        var foundationApprenticeship = filter.Items.First(i => i.DisplayText == LearningType.FoundationApprenticeship.GetDescription());
        var apprenticeship = filter.Items.First(i => i.DisplayText == LearningType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Id, Is.EqualTo("training-types"));
            Assert.That(filter.Heading, Is.EqualTo("Training type"));
            Assert.That(filter.For, Is.EqualTo(nameof(SearchCoursesViewModel.SelectedTypes)));
            Assert.That(filter.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));
            Assert.That(filter.Link, Is.Null);

            Assert.That(filter.Items, Has.Count.EqualTo(3));
            Assert.That(filter.Items.All(i => i.IsLearningTypeEmphasised), Is.False);

            Assert.That(apprenticeshipUnit.DisplayText, Is.EqualTo(LearningType.ApprenticeshipUnit.GetDescription()));
            Assert.That(foundationApprenticeship.DisplayText, Is.EqualTo(LearningType.FoundationApprenticeship.GetDescription()));
            Assert.That(apprenticeship.DisplayText, Is.EqualTo(LearningType.Apprenticeship.GetDescription()));

            Assert.That(apprenticeshipUnit.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.LearningTypeApprenticeshipUnitDescription));
            Assert.That(foundationApprenticeship.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.LearningTypeFoundationApprenticeshipDescription));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(LearningTypesFilterHelper.LearningTypeApprenticeshipDescription));

            Assert.That(apprenticeshipUnit.IsSelected, Is.False);
            Assert.That(foundationApprenticeship.IsSelected, Is.True);
            Assert.That(apprenticeship.IsSelected, Is.False);
        });
    }

    [Test]
    public void TrainingTypesCheckboxListItems_WithSelectedTypes_ReflectsSelection()
    {
        var selectedTypes = new List<LearningType>
        {
            LearningType.Apprenticeship
        };

        var sut = new SearchCoursesViewModel
        {
            SelectedTypes = selectedTypes,
            TrainingTypesFilterItems = LearningTypesFilterHelper.BuildItems(selectedTypes)
        };

        var filter = sut.TrainingTypesCheckboxListItems;

        var apprenticeshipUnit = filter.Items.First(i => i.DisplayText == LearningType.ApprenticeshipUnit.GetDescription());
        var foundationApprenticeship = filter.Items.First(i => i.DisplayText == LearningType.FoundationApprenticeship.GetDescription());
        var apprenticeship = filter.Items.First(i => i.DisplayText == LearningType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(apprenticeshipUnit.IsSelected, Is.False);
            Assert.That(foundationApprenticeship.IsSelected, Is.False);
            Assert.That(apprenticeship.IsSelected, Is.True);
        });
    }

    [Test]
    public void TrainingTypesCheckboxListItems_WithEmptySelectedTypes_AreUnselected()
    {
        var sut = new SearchCoursesViewModel
        {
            TrainingTypesFilterItems = LearningTypesFilterHelper.BuildItems([])
        };

        var filter = sut.TrainingTypesCheckboxListItems;
        Assert.Multiple(() =>
        {
            Assert.That(filter.Items, Has.Count.EqualTo(3));
            Assert.That(filter.Items.All(i => i.IsSelected), Is.False);
            Assert.That(filter.Items.All(i => i.IsLearningTypeEmphasised), Is.False);
        });
    }
}
