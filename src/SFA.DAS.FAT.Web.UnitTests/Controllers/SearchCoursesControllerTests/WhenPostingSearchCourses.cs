using AutoFixture.NUnit4;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.SearchCoursesControllerTests;

public class WhenPostingSearchCourses
{
    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_No_RouteValues_Set(
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel();

        //Act
        var actual = controller.Index(viewModel);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToRouteResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.Courses);
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_CourseTerm_In_RouteValues(
        string courseTerm,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel { CourseTerm = courseTerm };
        //Act
        var actual = controller.Index(viewModel);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToRouteResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.Courses);
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().Be(courseTerm);
            result.RouteValues.Should().ContainKey("Location");
            result.RouteValues!["Location"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_Without_Location_And_Distance_In_RouteValues(
        string location,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel { Location = location };
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
                    .Returns(new LocationCookieItem { Location = viewModel.Location, Distance = DistanceService.DefaultDistance.ToString() });

        //Act
        var actual = controller.Index(viewModel);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToRouteResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.Courses);
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void And_Post_Redirects_to_Courses_With_Course_Term_In_Route_Values(
        string location,
        string courseTerm,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel { Location = location, CourseTerm = courseTerm };

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
                    .Returns(new LocationCookieItem { Location = viewModel.Location, Distance = DistanceService.DefaultDistance.ToString() });

        //Act
        var actual = controller.Index(viewModel);

        //Assert
        actual.Should().NotBeNull();
        var result = actual! as RedirectToRouteResult;
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.RouteName.Should().Be(RouteNames.Courses);
            result.RouteValues.Should().ContainKey("Keyword");
            result.RouteValues!["Keyword"].Should().Be(courseTerm);
            result.RouteValues.Should().ContainKey("Location");
        }
    }
}
