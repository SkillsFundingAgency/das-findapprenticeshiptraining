using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CoursesViewModelTests;

public class WhenCreatingCoursesViewModel
{
    private Mock<IUrlHelper> _urlHelper;
    private Mock<FindApprenticeshipTrainingWeb> _findApprenticeshipTrainingWebConfiguration;

    [SetUp]
    public void SetUp()
    {
        _findApprenticeshipTrainingWebConfiguration = new Mock<FindApprenticeshipTrainingWeb>();
        _urlHelper = new Mock<IUrlHelper>();
    }

    [Test]
    public void When_Location_Is_Not_Selected_Distance_Defaults_To_Ten_Miles_And_Is_Not_In_ClearFilters()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = string.Empty,
            Location = string.Empty,
            SelectedLevels = [],
            SelectedRoutes = []
        };

        var filters = _sut.Filters;

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Distance, Is.EqualTo(DistanceService.TEN_MILES.ToString()));
            Assert.That(filters.ClearFilterSections.Any(s => s.FilterType == FilterService.FilterType.Distance), Is.False);
            Assert.That(filters.ClearFilterSections, Is.Empty);
        });
    }

    [Test]
    public void Filters_CreateFilterSections_Has_Expected_Sections_And_Subsections()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "test",
            Location = "SW1",
            Distance = "10",
            SelectedTypes = ["Standard"],
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
    public void Then_The_Total_Message_Uses_Providers_Total_When_Unfiltered()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Total = 1,
            TotalFiltered = 1,
            Keyword = string.Empty
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("1 result"));
    }

    [Test]
    public void Then_The_Total_Message_Uses_Filtered_Total_If_There_Are_Selected_Routes()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Total = 10,
            TotalFiltered = 5,
            SelectedRoutes = ["Route"]
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void Then_The_Total_Message_Uses_Filtered_Total_If_There_Are_Selected_Levels()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Total = 10,
            TotalFiltered = 5,
            SelectedLevels = [1]
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void Then_The_Total_Message_Uses_Filtered_Total_If_Keyword_Is_Set()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Total = 10,
            TotalFiltered = 5,
            Keyword = "Keyword"
        };

        Assert.That(_sut.TotalMessage, Is.EqualTo("5 results"));
    }

    [Test]
    public void When_Keyword_Is_Set_Then_Courses_Are_Ordered_By_Score()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "Keyword"
        };

        Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Score));
    }

    [Test]
    public void When_Keyword_Is_Not_Set_Then_Courses_Are_Ordered_By_Title()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = string.Empty
        };

        Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Title));
    }

    [Test]
    public void When_Total_Is_Greater_Than_Zero_Then_CoursesSubHeader_Returns_Default_Message()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course."));
    }

    [Test]
    public void When_Location_Is_Set_And_Distance_Is_Not_All_Then_CoursesSubHeader_Returns_Location_Message()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Distance = "10",
            Location = "SW1",
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course in the apprentice's work location."));
    }

    [Test]
    public void When_Location_Is_Set_And_Distance_Is_All_Then_CoursesSubHeader_Returns_Default_Message()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Distance = "All",
            Location = "SW1",
            Total = 10
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course."));
    }

    [Test]
    public void When_Keyword_Is_Set_But_Total_Is_Zero_Then_CoursesSubHeader_Returns_Empty_String()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "Contruction",
            Total = 0
        };

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo(string.Empty));
    }

    [Test]
    public void When_Keyword_Is_Given_Then_Message_Is_Best_Match_To_Course()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "Construction"
        };

        Assert.That(_sut.SortedDisplayMessage, Is.EqualTo("Best match to course"));
    }

    [Test]
    public void When_OrderBy_Is_Title_Then_Sorted_Message_Is_Name_Of_Course()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = string.Empty
        };

        Assert.That(_sut.SortedDisplayMessage, Is.EqualTo("Name of course"));
    }

    [Test]
    public void When_Level_Exists_Then_A_Combination_Of_Level_Code_And_Name_Is_Returned()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Levels = new List<LevelViewModel>()
            {
                new LevelViewModel(
                    new Level()
                    {
                        Code = 3,
                        Name = "GCSE"
                    },
                    []
                )
            }
        };

        Assert.That(_sut.GetLevelName(3), Is.EqualTo("3 - equal to GCSE"));
    }

    [Test]
    public void When_Level_Does_Not_Exists_Then_An_Empty_String_Is_Returned()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Levels = []
        };

        Assert.That(_sut.GetLevelName(3), Is.EqualTo(string.Empty));
    }

    [Test, MoqAutoData]
    public void When_Standard_Providers_Count_Is_One_And_Location_Is_Null_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = null
        };

        standardViewModel.ProvidersCount = 1;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("View 1 training provider for this course"));
    }

    [Test, MoqAutoData]
    public void When_Standard_Providers_Count_Is_More_Than_One_And_Location_Is_Null_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = null
        };

        standardViewModel.ProvidersCount = 2;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers for this course"));
    }

    [Test, MoqAutoData]
    public void When_Standard_Providers_Count_Is_Zero_And_Location_Is_Null_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = null
        };

        standardViewModel.ProvidersCount = 0;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("Ask if training providers can run this course"));
    }

    [Test, MoqAutoData]
    public void Returns_Singular_Provider_Message_When_All_Distance_Is_Selected_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE
        };

        standardViewModel.ProvidersCount = 1;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("View 1 training provider for this course"));
    }

    [Test, MoqAutoData]
    public void Returns_Plural_Provider_Message_When_All_Distance_Is_Selected_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE
        };

        standardViewModel.ProvidersCount = 2;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers for this course"));
    }

    [Test, MoqAutoData]
    public void Returns_Singular_Distance_Message_When_Distance_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = "40"
        };

        standardViewModel.ProvidersCount = 1;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training provider within {model.Distance} miles"));
    }

    [Test, MoqAutoData]
    public void Returns_Plural_Distance_Message_When_Distance_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = "40"
        };

        standardViewModel.ProvidersCount = 2;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers within {model.Distance} miles"));
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_More_Than_Zero_Link_Returns_Providers_List_Url(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 2;
        standardViewModel.LarsCode = "123";

        var expectedUrl = "https://localhost/course/123/providers";
        var _mockUrlHelperMock = new Mock<IUrlHelper>();
        _mockUrlHelperMock
            .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(RouteNames.CourseProviders))))
            .Returns(expectedUrl);

        var _viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _mockUrlHelperMock.Object);

        var result = _viewModel.GetProvidersLink(standardViewModel);

        Assert.That(expectedUrl, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_Zero_And_Location_Is_Set_Link_Returns_Request_Apprenticeship_Training_Url_With_Location(
        StandardViewModel standardViewModel,
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={standardViewModel.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + "&location=sw1")}";

        standardViewModel.ProvidersCount = 0;
        var _sut = new CoursesViewModel(findApprenticeshipTrainingWebConfiguration, _urlHelper.Object)
        {
            Location = "sw1"
        };

        var result = _sut.GetProvidersLink(standardViewModel);
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_Zero_And_Location_Is_Not_Set_Link_Returns_Request_Apprenticeship_Training_Url_Without_Location(
        StandardViewModel standardViewModel,
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        string redirectUri = $"{findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={standardViewModel.LarsCode}&requestType={EntryPoint.CourseDetail}";
        string expectedLink = $"{findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri)}";

        standardViewModel.ProvidersCount = 0;
        var _sut = new CoursesViewModel(findApprenticeshipTrainingWebConfiguration, _urlHelper.Object);

        var result = _sut.GetProvidersLink(standardViewModel);
        Assert.That(expectedLink, Is.EqualTo(result));
    }

    [Test]
    public void Then_ToQueryString_Should_Include_Keyword_When_Set()
    {
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "test"
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "Keyword"), Is.AtLeast(0));
    }

    [Test]
    public void Then_ToQueryString_Should_Include_Location_And_Distance_When_Set()
    {
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = "10"
        };

        var _sut = viewModel.ToQueryString();

        Assert.Multiple(() =>
        {
            Assert.That(_sut.FindIndex(a => a.Item1 == "Location"), Is.AtLeast(0));
            Assert.That(_sut.FindIndex(a => a.Item1 == "Distance"), Is.AtLeast(0));
        });
    }

    [Test]
    public void Then_ToQueryString_Should_Include_Levels_When_SelectedLevels_Are_Set()
    {
        var selectedLevels = new List<int>() { 1 };
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
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
    public void Then_ToQueryString_Should_Include_Routes_When_SelectedRoutes_Are_Set()
    {
        var selectedRoutes = new List<string> { "Construction" };
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
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
    public void Then_ToQueryString_Should_Include_ApprenticeshipTypes_When_SelectedTypes_Are_Set()
    {
        var selectedTypes = new List<string> { "Standard" };
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            SelectedTypes = selectedTypes
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "ApprenticeshipTypes"), Is.AtLeast(0));
    }

    [Test]
    public void When_Location_Is_Set_Then_Location_And_Distance_Must_Be_Added_To_Standard_Routes()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = "10"
        };

        string larsCode = "123";

        var result = _sut.GenerateStandardRouteValues(larsCode);

        Assert.Multiple(() =>
        {
            Assert.That(result.ContainsKey("id"), Is.True);
            Assert.That(result["id"], Is.EqualTo(larsCode.ToString()));

            Assert.That(result.ContainsKey("location"), Is.True);
            Assert.That(result["location"], Is.EqualTo(_sut.Location));

            Assert.That(result.ContainsKey("distance"), Is.True);
            Assert.That(result["distance"], Is.EqualTo(_sut.Distance));
        });
    }

    [Test]
    public void When_Location_Is_Empty_Then_Location_And_Distance_Must_Not_Be_Added_To_Standard_Routes()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = string.Empty,
            Distance = "10"
        };

        string larsCode = "123";

        var result = _sut.GenerateStandardRouteValues(larsCode);

        Assert.Multiple(() =>
        {
            Assert.That(result.ContainsKey("id"), Is.True);
            Assert.That(result["id"], Is.EqualTo(larsCode.ToString()));

            Assert.That(result.ContainsKey("location"), Is.False);
            Assert.That(result.ContainsKey("distance"), Is.False);
        });
    }

    [Test]
    public void When_Location_Is_Null_Then_Location_And_Distance_Must_Not_Be_Added_To_Standard_Routes()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = null,
            Distance = "10"
        };

        string larsCode = "123";

        var result = _sut.GenerateStandardRouteValues(larsCode);

        Assert.Multiple(() =>
        {
            Assert.That(result.ContainsKey("id"), Is.True);
            Assert.That(result["id"], Is.EqualTo(larsCode.ToString()));

            Assert.That(result.ContainsKey("location"), Is.False);
            Assert.That(result.ContainsKey("distance"), Is.False);
        });
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_More_Than_Zero_And_Distance_Is_All_Route_Uses_Empty_Distance(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 2;
        standardViewModel.LarsCode = "123";

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName == RouteNames.CourseProviders)))
            .Returns("https://localhost/course/123/providers");

        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, mockUrlHelper.Object)
        {
            Location = "SW1",
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE
        };

        _ = _sut.GetProvidersLink(standardViewModel);

        mockUrlHelper.Verify(m => m.RouteUrl(
            It.Is<UrlRouteContext>(c =>
                c.RouteName == RouteNames.CourseProviders
                && c.Values != null
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("id")
                && Equals(new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["id"], standardViewModel.LarsCode)
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("Location")
                && Equals(new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["Location"], _sut.Location)
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("distance")
                && string.IsNullOrEmpty(new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["distance"] as string)
            )
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_More_Than_Zero_And_Distance_Is_Set_Route_Uses_Distance_Value(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 2;
        standardViewModel.LarsCode = "123";

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName == RouteNames.CourseProviders)))
            .Returns("https://localhost/course/123/providers");

        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, mockUrlHelper.Object)
        {
            Location = "SW1",
            Distance = "40"
        };

        _ = _sut.GetProvidersLink(standardViewModel);

        mockUrlHelper.Verify(m => m.RouteUrl(
            It.Is<UrlRouteContext>(c =>
                c.RouteName == RouteNames.CourseProviders
                && c.Values != null
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("id")
                && Equals(new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["id"], standardViewModel.LarsCode)
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("Location")
                && Equals(new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["Location"], _sut.Location)
                && new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values).ContainsKey("distance")
                && (new Microsoft.AspNetCore.Routing.RouteValueDictionary(c.Values)["distance"] as string) == "40"
            )
        ), Times.Once);
    }

    [Test]
    public void Then_Categories_Items_Are_Generated_From_Routes_And_Selection_Is_Applied()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
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
    public void Then_Categories_Items_Are_Empty_When_Routes_Are_Null()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Routes = null
        };

        var accordion = vm.Filters.FilterSections.First(s => s.Id == "multi-select");
        var categories = (CheckboxListFilterSectionViewModel)accordion.Children.First(c => c.Id == "categories-filter");

        Assert.That(categories.Items, Has.Count.EqualTo(0));
    }

    [Test]
    public void Then_CreateSelectedFilterSections_Only_Includes_Valid_Selected_Routes_And_Levels_Are_Coded_In_Links()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
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
    public void Then_CreateSelectedFilterSections_Returns_Empty_Clear_Sections_When_No_Filters_Selected()
    {
        var vm = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object);

        var clear = vm.Filters.ClearFilterSections;
        Assert.That(clear, Is.Empty);
    }
}
