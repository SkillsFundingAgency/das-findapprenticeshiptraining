using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests;

public class WhenCreatingCoursesViewModel
{
    [Test]
    public void CreateFilters_LocationNotSelected_DistanceDefaultsToTenMilesAndNotInClearFilters()
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = string.Empty,
            Location = string.Empty,
            SelectedLevels = [],
            SelectedRoutes = []
        };

        var filters = _sut.Filters;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut.Distance, Is.EqualTo(DistanceService.TenMiles.ToString()));
            Assert.That(filters.ClearFilterSections.Any(s => s.FilterType == FilterService.FilterType.Distance), Is.False);
            Assert.That(filters.ClearFilterSections, Is.Empty);
        }
    }

    [Test]
    public void CreateFilterSections_WithInputs_HasExpectedSectionsAndSubsections()
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = "test",
            Location = "SW1",
            Distance = "10",
            SelectedTrainingTypes = [LearningType.Apprenticeship],
            Levels = [new LevelViewModel(new Level { Code = 2, Name = "GCSE" }, [])],
            Routes = [new RouteViewModel(new Route { Id = 1, Name = "Construction" }, [])]
        };

        var filters = _sut.CreateFilterSections();

        Assert.That(filters.FilterSections, Has.Count.EqualTo(4));

        var accordion = filters.FilterSections[3] as AccordionFilterSectionViewModel;
        Assert.That(accordion, Is.Not.Null);
        Assert.That(accordion!.Children, Has.Count.EqualTo(3));
    }
    [Test]
    public void TotalMessage_Unfiltered_UsesProvidersTotal()
    {
        var _sut = new CoursesViewModel()
        {
            Total = 1,
            TotalFiltered = 1,
            Keyword = string.Empty
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("1 result"));
    }

    [Test]
    public void TotalMessage_WithSelectedRoutes_UsesFilteredTotal()
    {
        var _sut = new CoursesViewModel()
        {
            Total = 10,
            TotalFiltered = 5,
            SelectedRoutes = ["Route"]
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void TotalMessage_WithSelectedLevels_UsesFilteredTotal()
    {
        var _sut = new CoursesViewModel()
        {
            Total = 10,
            TotalFiltered = 5,
            SelectedLevels = [1]
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void TotalMessage_WithKeyword_UsesFilteredTotal()
    {
        var _sut = new CoursesViewModel()
        {
            Total = 10,
            TotalFiltered = 5,
            Keyword = "Keyword"
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void OrderBy_WithKeywordSet_IsScore()
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = "Keyword"
        };

        Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Score));
    }

    [Test]
    public void OrderBy_WithKeywordEmpty_IsTitle()
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = string.Empty
        };

        Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Title));
    }

    [Test]
    public void CoursesSubHeader_WithTotalGreaterThanZero_ReturnsDefaultMessage()
    {
        var _sut = new CoursesViewModel()
        {
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course."));
    }

    [Test]
    public void CoursesSubHeader_WithLocationAndDistanceNotAll_ReturnsLocationMessage()
    {
        var _sut = new CoursesViewModel()
        {
            Distance = "10",
            Location = "SW1",
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course in the learner's work location."));
    }

    [Test]
    public void CoursesSubHeader_WithLocationAndDistanceAll_ReturnsDefaultMessage()
    {
        var _sut = new CoursesViewModel()
        {
            Distance = "All",
            Location = "SW1",
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course."));
    }

    [Test]
    public void CoursesSubHeader_WithKeywordAndZeroTotal_ReturnsEmptyString()
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = "Contruction",
            Total = 0
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo(string.Empty));
    }

    [TestCase("Construction", "Best match to course")]
    [TestCase("", "Name of course")]
    [TestCase(null, "Name of course")]
    public void SortedDisplayMessage_WithKeyword_ReturnsCorrectMessage(string keyword, string expectedMessage)
    {
        var _sut = new CoursesViewModel()
        {
            Keyword = keyword
        };

        Assert.That(_sut.SortedDisplayMessage, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void ToQueryString_WithKeyword_IncludesKeyword()
    {
        var viewModel = new CoursesViewModel()
        {
            Keyword = "test"
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "Keyword"), Is.AtLeast(0));
    }

    [Test]
    public void ToQueryString_WithLocationAndDistance_BothNotIncluded()
    {
        var viewModel = new CoursesViewModel()
        {
            Location = "SW1",
            Distance = "10"
        };

        var _sut = viewModel.ToQueryString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut.Count(a => a.Item1 == "Location"), Is.Zero);
            Assert.That(_sut.Count(a => a.Item1 == "Distance"), Is.Zero);
        }
    }

    [Test]
    public void ToQueryString_WithSelectedLevels_IncludesLevels()
    {
        var selectedLevels = new List<int>() { 1 };
        var viewModel = new CoursesViewModel()
        {
            Levels = new List<LevelViewModel>()
        {
            new(
                new Level()
                {
                    Code = 1,
                    Name = "Level"
                },
                selectedLevels
            )
        },
            SelectedLevels = selectedLevels
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "Levels"), Is.AtLeast(0));
    }

    [Test]
    public void ToQueryString_WithSelectedRoutes_IncludesCategories()
    {
        var selectedRoutes = new List<string> { "Construction" };
        var viewModel = new CoursesViewModel()
        {
            Routes = new List<RouteViewModel>()
        {
            new(
                new Route()
                {
                    Id = 1,
                    Name = "Construction"
                },
                selectedRoutes
            )
        },
            SelectedRoutes = selectedRoutes
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "Categories"), Is.AtLeast(0));
    }


    [Test]
    public void ToQueryString_WithSelectedTypes_IncludesLearningTypes()
    {
        var selectedTypes = new List<LearningType> { LearningType.Apprenticeship };
        var viewModel = new CoursesViewModel()
        {
            SelectedTrainingTypes = selectedTypes
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "LearningTypes"), Is.AtLeast(0));
    }

    [Test]
    public void CategoriesItems_WithRoutes_GeneratesItemsAndAppliesSelection()
    {
        var vm = new CoursesViewModel()
        {
            Routes =
            [
                new RouteViewModel(new Route { Name = "Construction" }, ["Construction"]),
                new RouteViewModel(new Route { Name = "Digital" }, ["Digital"])
            ],
            SelectedRoutes = ["Digital"]
        };

        var accordion = vm.Filters.FilterSections.First(s => s.Id == "multi-select");
        var categories = (CheckboxListFilterSectionViewModel)accordion.Children.First(c => c.Id == "categories-filter");

        Assert.That(categories.Items, Has.Count.EqualTo(2));
        categories.Items.First(i => i.Value == "Construction").IsSelected.Should().BeFalse();
        categories.Items.First(i => i.Value == "Digital").IsSelected.Should().BeTrue();
    }

    [Test]
    public void CreateFilterSections_WithEmptyRoutes_ReturnsEmptyCategoriesItems()
    {
        var vm = new CoursesViewModel()
        {
            Routes = []
        };

        var accordion = vm.Filters.FilterSections.First(s => s.Id == "multi-select");
        var categories = (CheckboxListFilterSectionViewModel)accordion.Children.First(c => c.Id == "categories-filter");

        Assert.That(categories.Items, Has.Count.EqualTo(0));
    }

    [Test]
    public void CreateSelectedFilterSections_WithMixedRoutes_IncludesValidRoutesAndCodesLevelsInLinks()
    {
        var vm = new CoursesViewModel()
        {
            Keyword = "k",
            SelectedRoutes = ["Construction", "Invalid"],
            Routes = [new RouteViewModel(new Route { Name = "Construction" }, ["Construction"])],
            Levels = [new LevelViewModel { Code = 3, Name = "Level 3" }],
            SelectedLevels = [3]
        };

        var clear = vm.Filters.ClearFilterSections;
        var categories = clear.First(s => s.FilterType == FilterService.FilterType.Categories);

        Assert.That(categories.Items, Has.Count.EqualTo(1));
        Assert.That(categories.Items[0].DisplayText, Is.EqualTo("Construction"));

        var keyword = clear.First(s => s.FilterType == FilterService.FilterType.KeyWord);
        keyword.Items[0].ClearLink.Should().Contain("levels=3").And.NotContain("levels=Level 3");
    }

    [Test]
    public void CreateSelectedFilterSections_WithNoFiltersSelected_ReturnsEmptyClearFilterSections()
    {
        var vm = new CoursesViewModel();

        var clear = vm.Filters.ClearFilterSections;
        Assert.That(clear, Is.Empty);
    }

    [Test]
    public void ToQueryString_LocationSelectedAndDistanceIsWhitespace_DoesNotIncludeBoth()
    {
        var viewModel = new CoursesViewModel()
        {
            Location = "SW1A 1AA",
            Distance = " "
        };

        var result = viewModel.ToQueryString();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count(x => x.Item1 == nameof(CoursesViewModel.Location)), Is.Zero);
            Assert.That(result.Count(x => x.Item1 == nameof(CoursesViewModel.Distance)), Is.Zero);
        }
    }

    [Test]
    public void Filters_SelectedTrainingTypesContainUndefinedValue_ClearFiltersExcludeTrainingType()
    {
        var viewModel = new CoursesViewModel()
        {
            SelectedTrainingTypes = [(LearningType)999]
        };

        var clearFilters = viewModel.Filters.ClearFilterSections;

        Assert.That(clearFilters.Any(x => x.FilterType == FilterService.FilterType.LearningTypes), Is.False);
    }

    [Test]
    public void GenerateLevelFilterItems_WithLevels_AppliesSelection()
    {
        var level = new Level { Code = 4, Name = "NVQ" };
        var vm = new CoursesViewModel()
        {
            Levels = new List<LevelViewModel>() { new LevelViewModel(level, new List<int>()) },
            SelectedLevels = new List<int> { 4 }
        };

        var filters = vm.CreateFilterSections();

        var accordion = (AccordionFilterSectionViewModel)Enumerable.First(filters.FilterSections, s => s.Id == "multi-select");
        var levels = (CheckboxListFilterSectionViewModel)Enumerable.First(accordion.Children, c => c.Id == "levels-filter");

        levels.Items.Should().HaveCount(1);
        var item = levels.Items[0];
        item.Value.Should().Be("4");
        item.DisplayText.Should().Be("Level 4");
        item.DisplayDescription.Should().Be($"Equal to {level.Name}");
        item.IsSelected.Should().BeTrue();
    }

    [Test]
    public void GenerateLevelFilterItems_WithNoLevels_ReturnsEmpty()
    {
        var vm = new CoursesViewModel()
        {
            Levels = new List<LevelViewModel>(),
            SelectedLevels = new List<int>()
        };

        var filters = vm.CreateFilterSections();

        var accordion = (AccordionFilterSectionViewModel)Enumerable.First(filters.FilterSections, s => s.Id == "multi-select");
        var levels = (CheckboxListFilterSectionViewModel)Enumerable.First(accordion.Children, c => c.Id == "levels-filter");

        levels.Items.Should().BeEmpty();
    }

    [Test]
    public void TotalMessage_Zero_Unfiltered_ReturnsZeroResults()
    {
        var vm = new CoursesViewModel()
        {
            Total = 0,
            TotalFiltered = 0,
            Keyword = string.Empty,
            SelectedRoutes = new List<string>(),
            SelectedLevels = new List<int>()
        };

        vm.TotalMessage.Should().Be("0 results");
    }

    [Test]
    public void ToQueryString_NoSelectedFilters_ReturnsEmpty()
    {
        var vm = new CoursesViewModel();

        var result = vm.ToQueryString();

        result.Should().BeEmpty();
    }

    [Test]
    public void SelectedFilterOptionsAreNotEmpty_DoesHaveFilters()
    {
        var sut = new CoursesViewModel()
        {
            SelectedRoutes = ["1"],
            SelectedTrainingTypes = [LearningType.Apprenticeship],
            SelectedLevels = [10],
        };

        using (new AssertionScope())
        {
            sut.Categories.Contains("1");
            sut.LearningTypes.Contains(LearningType.Apprenticeship);
            sut.LevelCodes.Contains(10);
        }
    }
    [Test]
    public void SelectedFilterOptionsNotEmpty_DoesNotHaveFilters()
    {
        var sut = new CoursesViewModel()
        {
            SelectedRoutes = [],
            SelectedTrainingTypes = [],
            SelectedLevels = [],
        };

        using (new AssertionScope())
        {
            sut.Categories.Should().BeEmpty();
            sut.LearningTypes.Should().BeEmpty();
            sut.LevelCodes.Should().BeEmpty();
        }
    }
}
