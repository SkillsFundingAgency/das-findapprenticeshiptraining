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

public class SearchCoursesControllerPostTests
{
    [Test, MoqAutoData]
    public void WhenPosting_ThenRedirectsToCoursesWithNoRouteValuesSet(
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
            result.RouteValues.Should().ContainKey("LocationName");
            result.RouteValues!["LocationName"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void WhenPosting_ThenRedirectsToCoursesWithCourseTermInRouteValues(
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
            result.RouteValues.Should().ContainKey("LocationName");
            result.RouteValues!["LocationName"].Should().BeNull();
            result.RouteValues.Should().ContainKey("Distance");
            result.RouteValues!["Distance"].Should().BeNull();
        }
    }

    [Test, MoqAutoData]
    public void WhenPosting_ThenRedirectsToCoursesWithoutLocationAndDistanceInRouteValues(
        string location,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel { LocationName = location };
        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
                    .Returns(new LocationCookieItem { LocationName = viewModel.LocationName, Distance = DistanceService.DefaultDistance.ToString() });

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
    public void WhenPosting_AndLocationCookieHasDefaultDistance_ThenRedirectsToCoursesWithCourseTermInRouteValues(
        string location,
        string courseTerm,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] SearchCoursesController controller)
    {
        //Arrange
        SearchCoursesSubmitModel viewModel = new SearchCoursesSubmitModel { LocationName = location, CourseTerm = courseTerm };

        locationCookieService.Setup(x => x.Get(Constants.LocationCookieName))
                    .Returns(new LocationCookieItem { LocationName = viewModel.LocationName, Distance = DistanceService.DefaultDistance.ToString() });

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
            result.RouteValues.Should().ContainKey("LocationName");
        }
    }
}
