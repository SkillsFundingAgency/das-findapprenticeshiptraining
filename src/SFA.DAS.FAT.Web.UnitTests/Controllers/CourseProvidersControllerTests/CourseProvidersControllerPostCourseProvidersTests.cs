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

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CourseProvidersControllerTests;

public class CourseProvidersControllerPostCourseProvidersTests
{
    [Test, MoqAutoData]
    public void CourseProvidersPost_UpdatesLocationCookie_AndRedirects(
        CourseProvidersFiltersSubmitModel submitModel,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller)
    {
        //Act
        var result = controller.ApplyFilters(submitModel) as RedirectToRouteResult;

        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviders));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == submitModel.Location && c.Distance == submitModel.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public void ApplyFilters_WhenLocationHasWhitespace_UpdatesCookieWithTrimmedLocation(
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller,
        CourseProvidersFiltersSubmitModel submitModel)
    {
        // Act
        var result = controller.ApplyFilters(submitModel) as RedirectToRouteResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviders));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == submitModel.Location.Trim() && c.Distance == submitModel.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task CourseProviderDetailsPost_UpdatesLocationCookie_AndRedirects(
        ProviderLocationSubmitModel model,
        string larsCode,
        int ukprn,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller)
    {
        // Act
        var result = await controller.ApplyLocation(model, larsCode, ukprn) as RedirectToRouteResult;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviderDetails));
            Assert.That(result.RouteValues, Is.Not.Null);
            Assert.That(result.RouteValues!["ukprn"].ToString(), Is.EqualTo(ukprn.ToString()));
            Assert.That(result.RouteValues!["larsCode"].ToString(), Is.EqualTo(larsCode));
        }

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == model.Location)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ApplyLocation_WhenCookieContainsDistance_PreservesDistanceAndTrimsLocation(
        string larsCode,
        int ukprn,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller)
    {
        // Arrange
        var submitModel = new ProviderLocationSubmitModel { Location = "  Manchester  " };
        locationCookieService
            .Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { Location = "Old", Distance = "40" });

        // Act
        var result = await controller.ApplyLocation(submitModel, larsCode, ukprn) as RedirectToRouteResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviderDetails));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == "Manchester" && c.Distance == "40")
        ), Times.Once);
    }
}
