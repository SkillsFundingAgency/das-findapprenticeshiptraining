using AutoFixture.NUnit4;
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
    public void CourseDetailsDelete_DeletesLocationCookie_AndRedirects(
        string larsCode,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CoursesController sut)
    {
        //Act
        var result = sut.CourseDetailsDelete(larsCode) as RedirectToRouteResult;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseDetails));

        locationCookieService.Verify(x => x.Delete(Constants.LocationCookieName), Times.Once);
    }
}
