using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using static SFA.DAS.FAT.Web.Models.CoursesViewModel;

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
        var result = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(result, Is.EqualTo("London (within 50 miles)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Exceeds_Maximum()
    {
        _coursesViewModel.Distance = 150;
        var result = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(result, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Is_Null()
    {
        _coursesViewModel.Distance = null;
        var result = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(result, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Work_Location_Distance_Display_Message_Should_Return_Across_England_When_Distance_Is_Less_Than_One()
    {
        _coursesViewModel.Distance = -1;
        var result = _coursesViewModel.WorkLocationDisplayMessage;
        Assert.That(result, Is.EqualTo("London (Across England)"));
    }

    [Test]
    public void Then_Selected_Filters_Should_Contain_Correct_Filters()
    {
        var selectedFilters = _coursesViewModel.GetSelectedFilters();

        Assert.Multiple(() =>
        {
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.KeyWord), Is.True);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Location), Is.True);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Levels), Is.True);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Categories), Is.True);
        });
    }

    [Test]
    public void Then_Selected_Filters_Should_Be_Empty()
    {
        _coursesViewModel.Distance = null;
        _coursesViewModel.Keyword = null;
        _coursesViewModel.SelectedLevels = [];
        _coursesViewModel.SelectedRoutes = [];
        _coursesViewModel.Location = null;

        var selectedFilters = _coursesViewModel.GetSelectedFilters();

        Assert.Multiple(() =>
        {
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.KeyWord), Is.False);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Location), Is.False);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Levels), Is.False);
            Assert.That(selectedFilters.ContainsKey(CoursesFilterType.Categories), Is.False);
        });
    }

    [Test]
    public void Then_Selected_Filters_Should_Not_Contain_Invalid_Level_Filter()
    {
        _coursesViewModel.SelectedLevels = [-1];
        var selectedFilters = _coursesViewModel.GetSelectedFilters();
        Assert.That(selectedFilters[CoursesFilterType.Levels].Any(), Is.False);
    }

    [Test]
    public void Then_Selected_Filters_Should_Not_Contain_Invalid_Category_Filter()
    {
        _coursesViewModel.SelectedRoutes = ["Invalid Route"];
        var selectedFilters = _coursesViewModel.GetSelectedFilters();
        Assert.That(selectedFilters[CoursesFilterType.Categories].Any(), Is.False);
    }

    [Test]
    public void Then_Clear_Links_Must_Have_Correct_Permutations()
    {
        CoursesViewModel sut = new CoursesViewModel
        {
            Location = "M60 RNA",
            Distance = null,
            Keyword = "Construction",
            SelectedLevels = [],
            SelectedRoutes = []
        };

        var result = sut.GenerateFilterClearLinks();

        Assert.Multiple(() =>
        {
            Assert.That(result.ContainsKey(CoursesFilterType.KeyWord), Is.True);
            Assert.That(result[CoursesFilterType.KeyWord]["Construction"], Is.EqualTo("?location=M60 RNA"));

            Assert.That(result.ContainsKey(CoursesFilterType.Location), Is.True);
            Assert.That(result[CoursesFilterType.Location]["M60 RNA"], Is.EqualTo("?keyword=Construction"));
        });
    }
}
