using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
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
    public void When_Keyword_Is_Set_And_Total_Is_Greater_Than_Zero_Then_CoursesSubHeader_Returns_Expected_Message()
    {
        var _sut = new CoursesViewModel(_findApprenticeshipTrainingWebConfiguration.Object, _urlHelper.Object)
        {
            Keyword = "Construction",
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
    public void When_Standard_Providers_Count_Is_One_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 1;
        var _sut = CoursesViewModel.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("View 1 training provider for this course."));
    }

    [Test, MoqAutoData]
    public void When_Standard_Providers_Count_Is_More_Than_One_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 2;
        var _sut = CoursesViewModel.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo($"View {standardViewModel.ProvidersCount} training providers for this course."));
    }

    [Test, MoqAutoData]
    public void When_Standard_Providers_Count_Is_Zero_Then_View_One_Provider_Message_Is_Returned(StandardViewModel standardViewModel)
    {
        standardViewModel.ProvidersCount = 0;
        var _sut = CoursesViewModel.GetProvidersLinkDisplayMessage(standardViewModel);
        Assert.That(_sut, Is.EqualTo("Ask if training providers can run this course."));
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
}
