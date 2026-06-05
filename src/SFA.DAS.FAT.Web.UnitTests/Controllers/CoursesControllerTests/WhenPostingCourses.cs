using AutoFixture.NUnit4;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests;

public class WhenPostingCourses
{
    [Test, MoqAutoData]
    public void CoursesPost_UpdatesLocationCookie_AndRedirects(
        GetCoursesViewModel submitModel,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut)
    {
        //Act
        var result = sut.CoursesPost(submitModel) as RedirectToRouteResult;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.Courses));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == submitModel.Location && c.Distance == submitModel.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public void CourseDetailsPost_UpdatesLocationCookie_AndRedirects(
        string larsCode,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut)
    {
        CoursesViewModel model = new CoursesViewModel();
        model.Location = "Test Location";
        model.Distance = "10 miles";

        //Act
        var result = sut.CourseDetailsPost(model, larsCode) as RedirectToRouteResult;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseDetails));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == model.Location && c.Distance == model.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task CourseDetailsDelete_DeletesLocationCookie_AndRedirects(
        string larsCode,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut)
    {
        //Act
        var isRemoveLocation = true;
        var result = await sut.CourseDetails(larsCode, isRemoveLocation) as ViewResult;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ViewName, Is.EqualTo("CourseDetails"));

        locationCookieService.Verify(x => x.Delete(Constants.LocationCookieName), Times.Once);
    }
    [Test, MoqAutoData]
    public void CoursesPost_WithSubmitModel_UpdatesLocationCookieAndRedirectsToCourses(
       GetCoursesViewModel submitModel,
       [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
       [Greedy] CoursesController sut
   )
    {
        // Act
        var result = sut.CoursesPost(submitModel) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.Courses);

        result.RouteValues.Should().ContainKey(nameof(GetCoursesViewModel.Keyword));
        result.RouteValues[nameof(GetCoursesViewModel.Keyword)].Should().Be(submitModel.Keyword);
        result.RouteValues.Should().ContainKey(nameof(GetCoursesViewModel.Categories));
        result.RouteValues[nameof(GetCoursesViewModel.PageNumber)].Should().Be(1);

        locationCookieService.Verify(
            x => x.Update(
                Constants.LocationCookieName,
                It.Is<LocationCookieItem>(c => c.Location == submitModel.Location && c.Distance == submitModel.Distance)
            ),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public void CourseDetailsPost_WithModel_UpdatesLocationCookieAndRedirectsToCourseDetails(
        string larsCode,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut
    )
    {
        CoursesViewModel model = new CoursesViewModel();

        // Act
        var result = sut.CourseDetailsPost(model, larsCode) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.CourseDetails);
        result.RouteValues.Should().ContainKey("larsCode");
        result.RouteValues["larsCode"].Should().Be(larsCode);

        locationCookieService.Verify(
            x => x.Update(
                Constants.LocationCookieName,
                It.Is<LocationCookieItem>(c => c.Location == model.Location && c.Distance == model.Distance)
            ),
            Times.Once
        );
    }

    [Test, MoqAutoData]
    public async Task CourseDetailsDelete_WithLarsCode_DeletesLocationCookieAndRedirectsToCourseDetails(
        string larsCode,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut
    )
    {
        // Act
        var isRemoveLocation = true;
        var result = await sut.CourseDetails(larsCode, isRemoveLocation) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.ViewName.Should().Be("CourseDetails");

        locationCookieService.Verify(x => x.Delete(Constants.LocationCookieName), Times.Once);
    }
}
