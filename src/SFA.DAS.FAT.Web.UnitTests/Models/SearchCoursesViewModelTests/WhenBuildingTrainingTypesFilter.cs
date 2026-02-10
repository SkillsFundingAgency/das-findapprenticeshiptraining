using FluentAssertions;
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
    public void Then_TrainingTypesFilter_Must_Be_Configured_Correctly()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.FoundationApprenticeship.GetDescription()
        };

        var types = new List<TypeViewModel>
        {
            new TypeViewModel(
                new ApprenticeType
                {
                    Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                    Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
                },
                selectedTypes),
            new TypeViewModel(
                new ApprenticeType
                {
                    Code = ApprenticeshipType.Apprenticeship.ToString(),
                    Name = ApprenticeshipType.Apprenticeship.GetDescription()
                },
                selectedTypes)
        };

        var sut = new SearchCoursesViewModel
        {
            CourseTerm = "engineering",
            Types = types,
            SelectedTypes = selectedTypes
        };

        var filter = sut.TrainingTypesFilter;

        Assert.Multiple(() =>
        {
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Id, Is.EqualTo("training-types"));
            Assert.That(filter.Heading, Is.EqualTo("Training type"));
            Assert.That(filter.For, Is.EqualTo(nameof(SearchCoursesViewModel.SelectedTypes)));
            Assert.That(filter.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));
            Assert.That(filter.Link, Is.Null);
        });

        filter.Items.Should().HaveCount(2);

        var foundation = filter.Items[0];
        var apprenticeship = filter.Items[1];

        Assert.Multiple(() =>
        {
            Assert.That(foundation.DisplayText, Is.EqualTo(ApprenticeshipType.FoundationApprenticeship.GetDescription()));
            Assert.That(apprenticeship.DisplayText, Is.EqualTo(ApprenticeshipType.Apprenticeship.GetDescription()));

            Assert.That(foundation.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION));
            Assert.That(apprenticeship.DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION));

            Assert.That(foundation.Selected, Is.True);
            Assert.That(apprenticeship.Selected, Is.False);
        });
    }

    [Test]
    public void Then_TrainingTypesFilter_Items_Must_Reflect_SelectedTypes()
    {
        var selectedTypes = new List<string>
        {
            ApprenticeshipType.Apprenticeship.GetDescription()
        };

        var types = new List<TypeViewModel>
        {
            new TypeViewModel(
                new ApprenticeType
                {
                    Code = ApprenticeshipType.FoundationApprenticeship.ToString(),
                    Name = ApprenticeshipType.FoundationApprenticeship.GetDescription()
                },
                selectedTypes),
            new TypeViewModel(
                new ApprenticeType
                {
                    Code = ApprenticeshipType.Apprenticeship.ToString(),
                    Name = ApprenticeshipType.Apprenticeship.GetDescription()
                },
                selectedTypes)
        };

        var sut = new SearchCoursesViewModel
        {
            Types = types,
            SelectedTypes = selectedTypes
        };

        var filter = sut.TrainingTypesFilter;

        var foundation = filter.Items.First(i => i.DisplayText == ApprenticeshipType.FoundationApprenticeship.GetDescription());
        var apprenticeship = filter.Items.First(i => i.DisplayText == ApprenticeshipType.Apprenticeship.GetDescription());

        Assert.Multiple(() =>
        {
            Assert.That(foundation.Selected, Is.False);
            Assert.That(apprenticeship.Selected, Is.True);
        });
    }

    [Test]
    public void Then_TrainingTypesFilter_Items_Are_Empty_When_Types_Are_Empty()
    {
        var sut = new SearchCoursesViewModel
        {
            Types = [],
            SelectedTypes = []
        };

        var filter = sut.TrainingTypesFilter;

        filter.Items.Should().BeEmpty();
    }
}
