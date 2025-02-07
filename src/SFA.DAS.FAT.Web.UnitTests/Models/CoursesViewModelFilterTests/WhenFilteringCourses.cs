using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Filters;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelFilterTests;

public sealed class WhenFilteringCourses
{
    private CoursesViewModel _coursesViewModel;

    [SetUp]
    public void Setup()
    {
        _coursesViewModel = new CoursesViewModel
        {
            Location = "London",
            Distance = 50,
            Keyword = "IT",
            SelectedLevels = new List<int> { 3 },
            SelectedRoutes = new List<string> { "Technology" },
            Levels = new List<LevelViewModel> { new LevelViewModel { Code = 3, Name = "Level 3" } },
            Routes = new List<RouteViewModel> { new RouteViewModel(new Route() { Name = "Technology" }, ["Technology"]) }
        };
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Within_Miles()
    {
        var _sut = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(_sut, Is.EqualTo("London (within 50 miles)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Exceeds_Maximum()
    {
        _coursesViewModel.Distance = 150;
        var _sut = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(_sut, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Is_Null()
    {
        _coursesViewModel.Distance = null;
        var _sut = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(_sut, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Is_Less_Than_One()
    {
        _coursesViewModel.Distance = -1;
        var _sut = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(_sut, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Selected_Filters_Should_Contain_Correct_Selected_Filter_Sections()
    {
        var _sut = _coursesViewModel.SelectedFilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.KEYWORD_SECTION_HEADING), Is.Not.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LOCATION_SECTION_HEADING), Is.Not.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LEVELS_SECTION_HEADING), Is.Not.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.CATEGORIES_SECTION_HEADING), Is.Not.Null);
        });
    }

    [Test]
    public void Then_Selected_Filter_Sections_Should_Be_Empty()
    {
        _coursesViewModel.Distance = null;
        _coursesViewModel.Keyword = null;
        _coursesViewModel.SelectedLevels = [];
        _coursesViewModel.SelectedRoutes = [];
        _coursesViewModel.Location = null;

        var _sut = _coursesViewModel.SelectedFilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.KEYWORD_SECTION_HEADING), Is.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LOCATION_SECTION_HEADING), Is.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LEVELS_SECTION_HEADING), Is.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.CATEGORIES_SECTION_HEADING), Is.Null);
        });
    }

    [Test]
    public void Then_Selected_Filter_Sections_Should_Not_Contain_Invalid_Levels()
    {
        _coursesViewModel.SelectedLevels = [-1];
        var _sut = _coursesViewModel.SelectedFilterSections;
        Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LEVELS_SECTION_HEADING), Is.Null);
    }

    [Test]
    public void Then_Selected_Filter_Sections_Should_Not_Contain_Invalid_Categories()
    {
        _coursesViewModel.SelectedRoutes = ["Invalid Route"];
        var selectedFilterSections = _coursesViewModel.SelectedFilterSections;
        Assert.That(selectedFilterSections.FirstOrDefault(a => a.Title == CourseFilters.CATEGORIES_SECTION_HEADING), Is.Null);
    }

    [Test]
    public void Then_Clear_Links_Must_Have_Correct_Permutations()
    {
        CoursesViewModel _viewModel = new CoursesViewModel
        {
            Location = "M60 RNA",
            Distance = null,
            Keyword = "Construction",
            SelectedLevels = [],
            SelectedRoutes = []
        };

        var sut = _viewModel.SelectedFilterSections;

        Assert.Multiple(() =>
        {
            var keywordFilterSection = sut.FirstOrDefault(a => a.Title == CourseFilters.KEYWORD_SECTION_HEADING);
            Assert.That(keywordFilterSection, Is.Not.Null);
            Assert.That(keywordFilterSection.Items[0].ClearLink, Is.EqualTo("?location=M60 RNA"));

            var locationFilterSection = sut.FirstOrDefault(a => a.Title == CourseFilters.LOCATION_SECTION_HEADING);
            Assert.That(locationFilterSection, Is.Not.Null);
            Assert.That(locationFilterSection.Items[0].ClearLink, Is.EqualTo("?keyword=Construction"));
        });
    }

    [Test]
    public void When_Distance_Is_Set_Then_Clear_Links_Must_Contain_Both_Distance_And_Location_Permutation()
    {
        var routeName = "Agriculture, environmental and animal care";

        CoursesViewModel _viewModel = new CoursesViewModel
        {
            Location = "M60 RNA",
            Distance = 10,
            Keyword = string.Empty,
            Routes = new List<RouteViewModel> { new RouteViewModel(new Route() { Name = routeName }, [routeName]) },
            SelectedLevels = [],
            SelectedRoutes = [routeName]
        };

        var _sut = _viewModel.SelectedFilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LOCATION_SECTION_HEADING), Is.Not.Null);

            var categoriesSelectedFilterSections = _sut.FirstOrDefault(a => a.Title == CourseFilters.CATEGORIES_SECTION_HEADING);
            Assert.That(categoriesSelectedFilterSections, Is.Not.Null);
            Assert.That(categoriesSelectedFilterSections.Items[0].ClearLink, Is.EqualTo("?location=M60 RNA&distance=10"));
        });
    }

    [Test]
    public void When_Distance_Is_Invalid_Then_Location_Clear_Link_Must_Not_Contain_Distance_Permutation()
    {
        CoursesViewModel _viewModel = new CoursesViewModel
        {
            Location = "M60 RNA",
            Distance = -10,
            Keyword = string.Empty,
            SelectedLevels = [],
            SelectedRoutes = []
        };

        var _sut = _viewModel.SelectedFilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.LOCATION_SECTION_HEADING), Is.Not.Null);
            Assert.That(_sut.FirstOrDefault(a => a.Title == CourseFilters.KEYWORD_SECTION_HEADING), Is.Null);
        });
    }
}
