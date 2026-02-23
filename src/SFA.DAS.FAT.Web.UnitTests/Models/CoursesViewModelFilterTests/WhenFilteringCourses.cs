using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Models.Filters.Helpers;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelFilterTests;

public sealed class WhenFilteringCourses
{
    private CoursesViewModel _coursesViewModel;

    private FindApprenticeshipTrainingWeb _findApprenticeshipTrainingWebConfiguration;

    private Mock<IUrlHelper> _urlHelperMock;

    [SetUp]
    public void Setup()
    {
        _urlHelperMock = new Mock<IUrlHelper>();
        _findApprenticeshipTrainingWebConfiguration = new FindApprenticeshipTrainingWeb()
        {
            RequestApprenticeshipTrainingUrl = "https://localhost"
        };

        var selectedTypes = new List<string>
        {
            ApprenticeshipType.FoundationApprenticeship.GetDescription()
        };

        _coursesViewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Keyword = "Construction",
            Location = "M60 7RA",
            Distance = "20",
            SelectedLevels = new List<int> { 3, 4 },
            SelectedRoutes = new List<string> { "Construction" },
            SelectedTypes = selectedTypes,
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
    public void Filters_KeywordFilterSection_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Keyword)), Is.True);

            var keyWordFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Keyword));
            Assert.That(keyWordFilterSection, Is.TypeOf<TextBoxFilterSectionViewModel>());
            Assert.That(keyWordFilterSection.Id, Is.EqualTo("keyword-input"));
            Assert.That(keyWordFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Keyword)));
            Assert.That(keyWordFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.TextBox));
            Assert.That(((TextBoxFilterSectionViewModel)keyWordFilterSection).InputValue, Is.EqualTo(_coursesViewModel.Keyword));
            Assert.That(keyWordFilterSection.Heading, Is.EqualTo(FilterService.KEYWORD_SECTION_HEADING));
            Assert.That(keyWordFilterSection.SubHeading, Is.EqualTo(FilterService.KEYWORD_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Filters_LocationFilterSection_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Location)), Is.True);

            var locationFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Location));
            Assert.That(locationFilterSection, Is.TypeOf<SearchFilterSectionViewModel>());
            Assert.That(locationFilterSection.Id, Is.EqualTo("search-location"));
            Assert.That(locationFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Location)));
            Assert.That(locationFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Search));
            Assert.That(((SearchFilterSectionViewModel)locationFilterSection).InputValue, Is.EqualTo(_coursesViewModel.Location));
            Assert.That(locationFilterSection.Heading, Is.EqualTo(FilterService.LOCATION_SECTION_HEADING));
            Assert.That(locationFilterSection.SubHeading, Is.EqualTo(FilterService.LOCATION_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Filters_DistanceFilterSection_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Any(a => a.For == nameof(_coursesViewModel.Distance)), Is.True);

            var distanceFilterSection = _sut.First(a => a.For == nameof(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection, Is.TypeOf<DropdownFilterSectionViewModel>());
            Assert.That(distanceFilterSection.Id, Is.EqualTo("distance-filter"));
            Assert.That(distanceFilterSection.For, Is.EqualTo(nameof(_coursesViewModel.Distance)));
            Assert.That(distanceFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Dropdown));

            var dropdownFilter = ((DropdownFilterSectionViewModel)distanceFilterSection);

            dropdownFilter.Items.Should().BeEquivalentTo(
                FilterService.GetDistanceFilterValues(_coursesViewModel.Distance),
                options => options.WithStrictOrdering()
            );

            var selectedDistanceValue = dropdownFilter.Items.First(a => a.IsSelected);
            Assert.That(selectedDistanceValue.Value, Is.EqualTo(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection.Heading, Is.EqualTo(FilterService.DISTANCE_SECTION_HEADING));
            Assert.That(distanceFilterSection.SubHeading, Is.EqualTo(FilterService.DISTANCE_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Filters_AccordionWithLevels_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = _sut.First(a => a.Id == "multi-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(string.Empty));
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Accordion));

            var levelsFilterSection = accordionFilterSection.Children.First(a => a.For == nameof(_coursesViewModel.Levels));
            Assert.That(levelsFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(levelsFilterSection.Id, Is.EqualTo("levels-filter"));
            Assert.That(levelsFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var levelsCheckBoxList = ((CheckboxListFilterSectionViewModel)levelsFilterSection);
            Assert.That(levelsCheckBoxList.Items, Has.Count.EqualTo(_coursesViewModel.Levels.Count));
            Assert.That(levelsCheckBoxList.Items.Where(a => a.IsSelected).ToList(), Has.Count.EqualTo(_coursesViewModel.SelectedLevels.Count));
            Assert.That(levelsCheckBoxList.Heading, Is.EqualTo(FilterService.LEVELS_SECTION_HEADING));
            Assert.That(levelsCheckBoxList.Link, Is.Not.Null);
            Assert.That(levelsCheckBoxList.Link.DisplayText, Is.EqualTo(FilterService.LEVEL_INFORMATION_DISPLAY_TEXT));
            Assert.That(levelsCheckBoxList.Link.Url, Is.EqualTo(FilterService.LEVEL_INFORMATION_URL));
        });
    }

    [Test]
    public void Filters_AccordionWithCategories_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = _sut.First(a => a.Id == "multi-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(string.Empty));
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Accordion));

            var categoryFilterSection = accordionFilterSection.Children.First(a => a.For == nameof(FilterService.FilterType.Categories));
            Assert.That(categoryFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(categoryFilterSection.Id, Is.EqualTo("categories-filter"));
            Assert.That(categoryFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var categoriesCheckBoxList = ((CheckboxListFilterSectionViewModel)categoryFilterSection);
            Assert.That(categoriesCheckBoxList.Items, Has.Count.EqualTo(_coursesViewModel.Routes.Count));
            Assert.That(categoriesCheckBoxList.Items.Where(a => a.IsSelected).ToList(), Has.Count.EqualTo(_coursesViewModel.SelectedRoutes.Count));
            Assert.That(categoriesCheckBoxList.Heading, Is.EqualTo(FilterService.CATEGORIES_SECTION_HEADING));
            Assert.That(categoriesCheckBoxList.Link, Is.Null);
        });
    }

    [Test]
    public void Filters_AccordionWithApprenticeshipTypes_IsPresent()
    {
        var _sut = _coursesViewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = _sut.First(a => a.Id == "multi-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(string.Empty));
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Accordion));

            var typeFilterSection = accordionFilterSection.Children.First(a => a.For == nameof(FilterService.FilterType.ApprenticeshipTypes));
            Assert.That(typeFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(typeFilterSection.Id, Is.EqualTo("types-filter"));
            Assert.That(typeFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var typesCheckBoxList = ((CheckboxListFilterSectionViewModel)typeFilterSection);
            Assert.That(typesCheckBoxList.Items, Has.Count.EqualTo(3));
            Assert.That(typesCheckBoxList.Items.Where(a => a.IsSelected).ToList(), Has.Count.EqualTo(_coursesViewModel.SelectedTypes.Count));
            Assert.That(typesCheckBoxList.Heading, Is.EqualTo(FilterService.APPRENTICESHIP_TYPES_SECTION_HEADING));
            Assert.That(typesCheckBoxList.Link, Is.Not.Null);
            Assert.That(typesCheckBoxList.Link.DisplayText, Is.EqualTo(CoursesViewModel.APPRENTICESHIP_TYPE_FIND_OUT_MORE_TEXT));
            Assert.That(typesCheckBoxList.Link.Url, Is.EqualTo(CoursesViewModel.APPRENTICESHIP_TYPE_FIND_OUT_MORE_LINK));
            Assert.That(typesCheckBoxList.Items[0].DisplayText, Is.EqualTo(ApprenticeshipType.ApprenticeshipUnit.GetDescription()));
            Assert.That(typesCheckBoxList.Items[1].DisplayText, Is.EqualTo(ApprenticeshipType.FoundationApprenticeship.GetDescription()));
            Assert.That(typesCheckBoxList.Items[2].DisplayText, Is.EqualTo(ApprenticeshipType.Apprenticeship.GetDescription()));
            Assert.That(typesCheckBoxList.Items[0].DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION));
            Assert.That(typesCheckBoxList.Items[1].DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION));
            Assert.That(typesCheckBoxList.Items[2].DisplayDescription, Is.EqualTo(ApprenticeshipTypesFilterHelper.APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION));
        });
    }

    [Test]
    public void ClearFilters_Location_IncludesClearLink()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var locationClearLink = _sut.First(a => a.FilterType == FilterService.FilterType.Location);
            Assert.That(locationClearLink, Is.Not.Null);
            Assert.That(locationClearLink.Title, Is.EqualTo("learner's work location"));
            Assert.That(locationClearLink.Items[0].DisplayText, Is.EqualTo($"{_coursesViewModel.Location} (within {_coursesViewModel.Distance} miles)"));
            Assert.That(locationClearLink.Items[0].ClearLink, Is.Not.Contain($"location={_coursesViewModel.Location}"));
        });
    }

    [Test]
    public void ClearFilters_Levels_IncludesClearLinks()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var levelsClearLinks = _sut.First(a => a.FilterType == FilterService.FilterType.Levels);
            Assert.That(levelsClearLinks, Is.Not.Null);
            Assert.That(levelsClearLinks.Items, Has.Count.EqualTo(2));

            var levelThreeLink = levelsClearLinks.Items.First(a => a.DisplayText == "Level 3");
            Assert.That(levelThreeLink, Is.Not.Null);
            Assert.That(levelThreeLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=4&categories=Construction&apprenticeshiptypes=Foundation apprenticeships"));

            var levelFourLink = levelsClearLinks.Items.First(a => a.DisplayText == "Level 4");
            Assert.That(levelFourLink, Is.Not.Null);
            Assert.That(levelFourLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=3&categories=Construction&apprenticeshiptypes=Foundation apprenticeships"));
        });
    }

    [Test]
    public void ClearFilters_Keyword_IncludesClearLink()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var keywordClearLink = _sut.First(a => a.FilterType == FilterService.FilterType.KeyWord);
            Assert.That(keywordClearLink, Is.Not.Null);
            Assert.That(keywordClearLink.Items, Has.Count.EqualTo(1));
            Assert.That(keywordClearLink.Items[0].DisplayText, Is.EqualTo("Construction"));
            Assert.That(keywordClearLink.Items[0].ClearLink, Is.EqualTo("?location=M60 7RA&distance=20&levels=3&levels=4&categories=Construction&apprenticeshiptypes=Foundation apprenticeships"));
        });
    }

    [Test]
    public void ClearFilters_Categories_IncludesClearLinks()
    {
        var _sut = _coursesViewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var categoryClearLinks = _sut.First(a => a.FilterType == FilterService.FilterType.Categories);
            Assert.That(categoryClearLinks, Is.Not.Null);
            Assert.That(categoryClearLinks.Items, Has.Count.EqualTo(1));

            var constructionCategoryClearLink = categoryClearLinks.Items.First(a => a.DisplayText == "Construction");
            Assert.That(constructionCategoryClearLink, Is.Not.Null);
            Assert.That(constructionCategoryClearLink.ClearLink, Is.EqualTo("?keyword=Construction&location=M60 7RA&distance=20&levels=3&levels=4&apprenticeshiptypes=Foundation apprenticeships"));
        });
    }

    [Test]
    public void ClearFilters_Distance_DoesNotIncludeClearLink()
    {
        var clearFilterSections = _coursesViewModel.Filters.ClearFilterSections;

        var locationClearLink = clearFilterSections.FirstOrDefault(a => a.FilterType == FilterService.FilterType.Distance);
        Assert.That(locationClearLink, Is.Null);
    }

    [Test]
    public void Distance_WhenLocationNotSet_DefaultsToTenMiles()
    {
        CoursesViewModel _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Location = null,
            Distance = "10"
        };

        Assert.Multiple(() =>
        {
            var distanceFilterSection = _sut.Filters.FilterSections.First(a => a.For == nameof(_coursesViewModel.Distance));
            Assert.That(distanceFilterSection, Is.Not.Null);

            var selectedItem = ((DropdownFilterSectionViewModel)distanceFilterSection).Items.First(a => a.IsSelected);
            Assert.That(selectedItem.Value, Is.EqualTo(DistanceService.TEN_MILES.ToString()));
        });
    }

    [Test]
    public void GetLevelName_WithExistingLevel_ReturnsFormattedName()
    {
        var result = _coursesViewModel.GetLevelName(3);
        Assert.That(result, Is.EqualTo("3 - equal to Level 3"));
    }

    [Test]
    public void GetLevelName_WithMissingLevel_ReturnsEmptyString()
    {

        var result = _coursesViewModel.GetLevelName(9);
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void OrderBy_WithKeywordSet_IsScore()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Keyword = "Construction"
        };

        Assert.That(vm.OrderBy, Is.EqualTo(OrderBy.Score));
    }

    [Test]
    public void OrderBy_WithEmptyKeyword_IsTitle()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Keyword = string.Empty
        };

        Assert.That(vm.OrderBy, Is.EqualTo(OrderBy.Title));
    }

    [Test]
    public void SortedDisplayMessage_BasedOnOrderBy_IsCorrect()
    {
        Assert.That(_coursesViewModel.SortedDisplayMessage, Is.EqualTo(CoursesViewModel.BEST_MATCH_TO_COURSE));

        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Keyword = string.Empty
        };

        Assert.That(vm.SortedDisplayMessage, Is.EqualTo(CoursesViewModel.NAME_OF_COURSE));
    }

    [Test, MoqAutoData]
    public void GetProvidersLinkDisplayMessage_WithZeroProviders_ReturnsAskTrainingProvider(StandardViewModel standard)
    {
        standard.ProvidersCount = 0;

        var message = _coursesViewModel.GetProvidersLinkDisplayMessage(standard);
        Assert.That(message, Is.EqualTo(CoursesViewModel.ASK_TRAINING_PROVIDER));
    }

    [Test, MoqAutoData]
    public void GetProvidersLinkDisplayMessage_WithNationalSearchWithoutLocation_ReturnsPluralProviders(StandardViewModel standard)
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Location = string.Empty,
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE
        };

        standard.LarsCode = "1";
        standard.ProvidersCount = 2;

        var message = vm.GetProvidersLinkDisplayMessage(standard);
        Assert.That(message, Is.EqualTo("View 2 training providers for this course"));
    }

    [Test, MoqAutoData]
    public void GetProvidersLinkDisplayMessage_WithLocalSearchAndDistance_ReturnsSingularProvider(StandardViewModel standard)
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Location = "SW1A 1AA",
            Distance = "30"
        };

        standard.LarsCode = "1";
        standard.ProvidersCount = 1;

        var message = vm.GetProvidersLinkDisplayMessage(standard);
        Assert.That(message, Is.EqualTo("View 1 training provider within 30 miles"));
    }

    [Test, MoqAutoData]
    public void GetProvidersLink_WithProvidersAvailable_UsesRouteUrl(StandardViewModel standard)
    {
        _urlHelperMock
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns("/courses/1/providers?location=M60%207RA&distance=20");

        standard.LarsCode = "1";
        standard.ProvidersCount = 3;

        var url = _coursesViewModel.GetProvidersLink(standard);
        Assert.That(url, Is.EqualTo("/courses/1/providers?location=M60%207RA&distance=20"));
    }

    [Test, MoqAutoData]
    public void GetProvidersLink_WithZeroProvidersAndLocation_ReturnsHelpUrlWithLocation(StandardViewModel standard)
    {
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://localhost",
            EmployerAccountsUrl = "https://accounts"
        };
        var vm = new CoursesViewModel(config, _urlHelperMock.Object)
        {
            Location = "M60 7RA"
        };

        standard.LarsCode = "123";
        standard.ProvidersCount = 0;

        var url = vm.GetProvidersLink(standard);
        Assert.Multiple(() =>
        {
            Assert.That(url, Does.StartWith("https://accounts/service/?redirectUri="));
            Assert.That(Uri.UnescapeDataString(url), Does.Contain("standardId=123"));
            Assert.That(Uri.UnescapeDataString(url), Does.Contain("location=M60 7RA"));
        });
    }

    [TestCase(1, 2, "", 0, 0, "1 result")]
    [TestCase(2, 3, "x", 1, 2, "3 results")]
    public void TotalMessage_BasedOnFilters_UsesCorrectCount(int total, int totalFiltered, string keyword, int routesCount, int levelsCount, string expectedMessage)
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Total = total,
            TotalFiltered = totalFiltered,
            Keyword = keyword,
            SelectedRoutes = routesCount == 0 ? [] : Enumerable.Repeat("Construction", routesCount).ToList(),
            SelectedLevels = levelsCount == 0 ? [] : Enumerable.Range(1, levelsCount).ToList()
        };

        Assert.That(vm.TotalMessage, Is.EqualTo(expectedMessage));
    }

    [TestCase(5, "M60 7RA", "20", "Select the course name to view details about it, or select view training providers to see the training providers who run that course in the learner's work location.")]
    [TestCase(5, "", DistanceService.ACROSS_ENGLAND_FILTER_VALUE, "Select the course name to view details about it, or select view training providers to see the training providers who run that course.")]
    [TestCase(0, "M60 7RA", "20", "")]
    public void CoursesSubHeader_BasedOnTotalLocationAndDistance_DisplaysCorrectMessage(int total, string location, string distance, string expected)
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Total = total,
            Location = location,
            Distance = distance
        };

        Assert.That(vm.CoursesSubHeader, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateStandardRouteValues_WithLocationAndDistance_IncludesBoth()
    {
        var values = _coursesViewModel.GenerateStandardRouteValues("55");
        Assert.Multiple(() =>
        {
            Assert.That(values["larsCode"], Is.EqualTo("55"));
            Assert.That(values.ContainsKey("location"), Is.True);
            Assert.That(values.ContainsKey("distance"), Is.True);
            Assert.That(values["location"], Is.EqualTo("M60 7RA"));
            Assert.That(values["distance"], Is.EqualTo("20"));
        });
    }

    [Test]
    public void ToQueryString_WithSelectedFilters_ReturnsKeyValuePairs()
    {
        var qs = _coursesViewModel.ToQueryString();
        Assert.Multiple(() =>
        {
            Assert.That(qs.Any(kv => kv.Item1 == nameof(CoursesViewModel.Keyword) && kv.Item2 == "Construction"), Is.True);

            Assert.That(qs.Any(kv => kv.Item1 == nameof(CoursesViewModel.Location) && kv.Item2 == "M60 7RA"), Is.True);
            Assert.That(qs.Any(kv => kv.Item1 == nameof(CoursesViewModel.Distance) && kv.Item2 == "20"), Is.True);

            Assert.That(qs.Count(kv => kv.Item1 == nameof(FilterService.FilterType.Levels)), Is.EqualTo(2));
            Assert.That(qs.Any(kv => kv.Item1 == nameof(FilterService.FilterType.Levels) && kv.Item2 == "3"), Is.True);
            Assert.That(qs.Any(kv => kv.Item1 == nameof(FilterService.FilterType.Levels) && kv.Item2 == "4"), Is.True);

            Assert.That(qs.Count(kv => kv.Item1 == nameof(FilterService.FilterType.Categories)), Is.EqualTo(1));
            Assert.That(qs.Any(kv => kv.Item1 == nameof(FilterService.FilterType.Categories) && kv.Item2 == "Construction"), Is.True);

            Assert.That(qs.Count(kv => kv.Item1 == nameof(FilterService.FilterType.ApprenticeshipTypes)), Is.EqualTo(1));
            Assert.That(qs.Any(kv => kv.Item1 == nameof(FilterService.FilterType.ApprenticeshipTypes) && kv.Item2 == ApprenticeshipType.FoundationApprenticeship.GetDescription()), Is.True);
        });
    }
}
