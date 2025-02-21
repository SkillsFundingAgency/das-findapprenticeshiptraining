using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Filters;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.Testing.AutoFixture;
using static SFA.DAS.FAT.Web.Models.Filters.FilterFactory;

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

        Assert.That(_sut.CoursesSubHeader, Is.EqualTo("Select the course name to view details about it, or select view training providers to see the training providers who run that course in the apprentice’s work location."));
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
    public void When_Order_By_Is_Score_Then_Message_Is_Best_Match_To_Course()
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

        Assert.That(_sut.GetLevelName(3), Is.EqualTo("3 (equal to GCSE)"));
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
        Assert.That(_sut, Is.EqualTo("View 1 training provider for this course."));
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
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers for this course."));
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
        Assert.That(_sut, Is.EqualTo("Ask if training providers can run this course."));
    }

    [Test, MoqAutoData]
    public void Returns_Singular_Provider_Message_When_All_Distance_Is_Selected_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Location = "SW1",
            Distance = ValidDistances.ACROSS_ENGLAND_FILTER_VALUE
        };

        standardViewModel.ProvidersCount = 1;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("View 1 training provider for this course."));
    }

    [Test, MoqAutoData]
    public void Returns_Plural_Provider_Message_When_All_Distance_Is_Selected_And_Location_Is_Set(StandardViewModel standardViewModel)
    {
        CoursesViewModel model = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            
            Location = "SW1",
            Distance = ValidDistances.ACROSS_ENGLAND_FILTER_VALUE
        };

        standardViewModel.ProvidersCount = 2;
        var _sut = model.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers for this course."));
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
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training provider within {model.Distance} miles."));
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
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers within {model.Distance} miles."));
    }

    [Test, MoqAutoData]
    public void When_Providers_Count_Is_More_Than_Zero_Link_Returns_Providers_List_Url(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 2;
        standardViewModel.LarsCode = 123;

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
    public void When_Providers_Count_Is_Zero_Link_Returns_Request_Apprenticeship_Training_Url(
        StandardViewModel standardViewModel,
        FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration
    )
    {
        standardViewModel.ProvidersCount = 0;
        var _sut = new CoursesViewModel(findApprenticeshipTrainingWebConfiguration, _urlHelper.Object);
        var result = _sut.GetProvidersLink(standardViewModel);
        Assert.That(findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl, Is.EqualTo(result));
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

        Assert.That(_sut.FindIndex(a => a.Item1 == "Location"), Is.AtLeast(0));
        Assert.That(_sut.FindIndex(a => a.Item1 == "Distance"), Is.AtLeast(0));
    }

    [Test]
    public void Then_ToQueryString_Should_Include_Levels_When_SelectedLevels_Are_Set()
    {
        var selectedLevels = new List<int>() { 1 };
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Levels = new List<LevelViewModel>()
            {
                new LevelViewModel(
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
                new RouteViewModel(
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
    public void Then_ToQueryString_Should_Include_PageNumber()
    {
        var viewModel = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            PageNumber = 3
        };

        var _sut = viewModel.ToQueryString();

        Assert.That(_sut.FindIndex(a => a.Item1 == "PageNumber"), Is.AtLeast(0));
    }
}
