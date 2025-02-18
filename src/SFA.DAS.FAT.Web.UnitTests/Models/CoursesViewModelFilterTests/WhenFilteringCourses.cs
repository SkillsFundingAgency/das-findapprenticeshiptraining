﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelFilterTests;

public sealed class WhenFilteringCourses
{
    private CoursesViewModel _coursesViewModel;

    [SetUp]
    public void Setup()
    {
        _coursesViewModel = new CoursesViewModel
        {
            Keyword = "Construction",
            Location = "M60 7RA",
            Distance = "20",
            SelectedLevels = new List<int> { 3, 4 },
            SelectedRoutes = new List<string> { "Construction" },
            Levels = new List<LevelViewModel> 
            { 
                new LevelViewModel { Code = 3, Name = "Level 3" },
                new LevelViewModel { Code = 4, Name = "Level 4" },
                new LevelViewModel { Code = 5, Name = "Level 5" }
            },
            Routes = new List<RouteViewModel> { new RouteViewModel(new Route() { Name = "Construction" }, ["Construction"]) }
        };
    }

    [Test]
    public void Then_Filters_Must_Contain_Keyword_Filter_Section()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Keyword)), Is.True);

            var keyWordFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Keyword));
            Assert.That(keyWordFilterSection, Is.TypeOf<TextBoxFilterSectionViewModel>());
            Assert.That(keyWordFilterSection.Id, Is.EqualTo("keyword-input"));
            Assert.That(keyWordFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Keyword)));
            Assert.That(keyWordFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.TextBox));
            Assert.That(((TextBoxFilterSectionViewModel)keyWordFilterSection).InputValue, Is.EqualTo(_coursesViewModel.Keyword));
            Assert.That(keyWordFilterSection.Heading, Is.EqualTo(FilterFactory.KEYWORD_SECTION_HEADING));
            Assert.That(keyWordFilterSection.SubHeading, Is.EqualTo(FilterFactory.KEYWORD_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Location_Filter_Section()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Location)), Is.True);

            var locationFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Location));
            Assert.That(locationFilterSection, Is.TypeOf<SearchFilterSectionViewModel>());
            Assert.That(locationFilterSection.Id, Is.EqualTo("search-location"));
            Assert.That(locationFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Location)));
            Assert.That(locationFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.Search));
            Assert.That(((SearchFilterSectionViewModel)locationFilterSection).InputValue, Is.EqualTo(_coursesViewModel.Location));
            Assert.That(locationFilterSection.Heading, Is.EqualTo(FilterFactory.LOCATION_SECTION_HEADING));
            Assert.That(locationFilterSection.SubHeading, Is.EqualTo(FilterFactory.LOCATION_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Distance_Filter_Section()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Distance)), Is.True);

            var distanceFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection, Is.TypeOf<DropdownFilterSectionViewModel>());
            Assert.That(distanceFilterSection.Id, Is.EqualTo("distance-filter"));
            Assert.That(distanceFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Distance)));
            Assert.That(distanceFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.Dropdown));

            var dropdownFilter = ((DropdownFilterSectionViewModel)distanceFilterSection);

            dropdownFilter.Items.Should().BeEquivalentTo(
                FilterFactory.GetDistanceFilterValues(_coursesViewModel.Distance),
                options => options.WithStrictOrdering()
            );

            var selectedDistanceValue = dropdownFilter.Items.First(a => a.Selected);
            Assert.That(selectedDistanceValue.Value, Is.EqualTo(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection.Heading, Is.EqualTo(FilterFactory.DISTANCE_SECTION_HEADING));
            Assert.That(distanceFilterSection.SubHeading, Is.EqualTo(FilterFactory.DISTANCE_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Accordion_Filter_Section_With_Levels()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = _sut.First(a => a.Id == "multi-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(string.Empty));
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.Accordion));

            var levelsFilterSection = accordionFilterSection.Children.First(a => a.For == nameof(_coursesViewModel.Levels));
            Assert.That(levelsFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(levelsFilterSection.Id, Is.EqualTo("levels-filter"));
            Assert.That(levelsFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.CheckboxList));

            var levelsCheckBoxList = ((CheckboxListFilterSectionViewModel)levelsFilterSection);
            Assert.That(levelsCheckBoxList.Items, Has.Count.EqualTo(_coursesViewModel.Levels.Count));
            Assert.That(levelsCheckBoxList.Items.Where(a => a.Selected).ToList(), Has.Count.EqualTo(_coursesViewModel.SelectedLevels.Count));
            Assert.That(levelsCheckBoxList.Heading, Is.EqualTo(FilterFactory.LEVELS_SECTION_HEADING));
            Assert.That(levelsCheckBoxList.Link, Is.Not.Null);
            Assert.That(levelsCheckBoxList.Link.DisplayText, Is.EqualTo(FilterFactory.LEVEL_INFORMATION_DISPLAY_TEXT));
            Assert.That(levelsCheckBoxList.Link.Url, Is.EqualTo(FilterFactory.LEVEL_INFORMATION_URL));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Accordion_Filter_Section_With_Categories()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = _sut.First(a => a.Id == "multi-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(string.Empty));
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.Accordion));

            var categoryFilterSection = accordionFilterSection.Children.First(a => a.For == nameof(FilterFactory.FilterType.Categories));
            Assert.That(categoryFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(categoryFilterSection.Id, Is.EqualTo("categories-filter"));
            Assert.That(categoryFilterSection.FilterComponentType, Is.EqualTo(FilterFactory.FilterComponentType.CheckboxList));

            var categoriesCheckBoxList = ((CheckboxListFilterSectionViewModel)categoryFilterSection);
            Assert.That(categoriesCheckBoxList.Items, Has.Count.EqualTo(_coursesViewModel.Routes.Count));
            Assert.That(categoriesCheckBoxList.Items.Where(a => a.Selected).ToList(), Has.Count.EqualTo(_coursesViewModel.SelectedRoutes.Count));
            Assert.That(categoriesCheckBoxList.Heading, Is.EqualTo(FilterFactory.CATEGORIES_SECTION_HEADING));
            Assert.That(categoriesCheckBoxList.Link, Is.Null);
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_Location_Clear_Link()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var locationClearLink = _sut.First(a => a.FilterType == FilterFactory.FilterType.Location);
            Assert.That(locationClearLink, Is.Not.Null);
            Assert.That(locationClearLink.Title, Is.EqualTo("Apprentice's work location"));
            Assert.That(locationClearLink.Items[0].DisplayText, Is.EqualTo($"{_coursesViewModel.Location} (within {_coursesViewModel.Distance} miles)"));
            Assert.That(locationClearLink.Items[0].ClearLink, Is.Not.Contain($"location={_coursesViewModel.Location}"));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_Levels_Clear_Links()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var levelsClearLinks = _sut.First(a => a.FilterType == FilterFactory.FilterType.Levels);
            Assert.That(levelsClearLinks, Is.Not.Null);
            Assert.That(levelsClearLinks.Items, Has.Count.EqualTo(2));

            var levelThreeLink = levelsClearLinks.Items.First(a => a.DisplayText == "Level 3");
            Assert.That(levelThreeLink, Is.Not.Null);
            Assert.That(levelThreeLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=4&categories=Construction"));

            var levelFourLink = levelsClearLinks.Items.First(a => a.DisplayText == "Level 4");
            Assert.That(levelFourLink, Is.Not.Null);
            Assert.That(levelFourLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=3&categories=Construction"));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_Keyword_Clear_Link()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var keywordClearLink = _sut.First(a => a.FilterType == FilterFactory.FilterType.KeyWord);
            Assert.That(keywordClearLink, Is.Not.Null);
            Assert.That(keywordClearLink.Items, Has.Count.EqualTo(1));
            Assert.That(keywordClearLink.Items[0].DisplayText, Is.EqualTo("Construction"));
            Assert.That(keywordClearLink.Items[0].ClearLink, Is.EqualTo("?location=M60 7RA&distance=20&levels=3&levels=4&categories=Construction"));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_Category_Clear_Links()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var categoryClearLinks = _sut.First(a => a.FilterType == FilterFactory.FilterType.Categories);
            Assert.That(categoryClearLinks, Is.Not.Null);
            Assert.That(categoryClearLinks.Items, Has.Count.EqualTo(1));

            var constructionCategoryClearLink = categoryClearLinks.Items.First(a => a.DisplayText == "Construction");
            Assert.That(constructionCategoryClearLink, Is.Not.Null);
            Assert.That(constructionCategoryClearLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=3&levels=4"));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Not_Contain_Distance_Clear_Link()
    {
        var clearFilterSections = _coursesViewModel.Filters.ClearFilterSections;

        var locationClearLink = clearFilterSections.FirstOrDefault(a => a.FilterType == FilterFactory.FilterType.Distance);
        Assert.That(locationClearLink, Is.Null);
    }

    [Test]
    public void When_Location_Is_Not_Set_Then_Distance_Filter_Must_Be_All()
    {
        CoursesViewModel _sut = new CoursesViewModel()
        {
            Location = null,
            Distance = "10"
        };

        Assert.Multiple(() =>
        {
            var distanceFilterSection = _sut.Filters.FilterSections.First(a => a.For == nameof(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection, Is.Not.Null);

            var selectedItem = ((DropdownFilterSectionViewModel)distanceFilterSection).Items.First(a => a.Selected);
            Assert.That(selectedItem.Value, Is.EqualTo(ValidDistances.ACROSS_ENGLAND_FILTER_VALUE));
        });
    }
}
