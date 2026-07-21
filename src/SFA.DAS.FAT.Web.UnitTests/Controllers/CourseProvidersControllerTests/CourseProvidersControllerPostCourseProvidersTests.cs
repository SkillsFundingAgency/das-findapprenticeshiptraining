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
    public void WhenPostingCourseProviders_ThenUpdatesLocationCookieAndRedirects(
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
            It.Is<LocationCookieItem>(c => c.LocationName == submitModel.LocationName && c.Distance == submitModel.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public void WhenApplyingFilters_AndLocationHasWhitespace_ThenUpdatesCookieWithTrimmedLocation(
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
            It.Is<LocationCookieItem>(c => c.LocationName == submitModel.LocationName.Trim() && c.Distance == submitModel.Distance)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenPostingCourseProviderDetails_ThenUpdatesLocationCookieAndRedirects(
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
            It.Is<LocationCookieItem>(c => c.LocationName == model.LocationName)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenApplyingLocation_AndCookieContainsDistance_ThenPreservesDistanceAndTrimsLocation(
        string larsCode,
        int ukprn,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller)
    {
        // Arrange
        var submitModel = new ProviderLocationSubmitModel { LocationName = "  Manchester  " };
        locationCookieService
            .Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(new LocationCookieItem { LocationName = "Old", Distance = "40" });

        // Act
        var result = await controller.ApplyLocation(submitModel, larsCode, ukprn) as RedirectToRouteResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviderDetails));

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.LocationName == "Manchester" && c.Distance == "40")
        ), Times.Once);
    }
}
