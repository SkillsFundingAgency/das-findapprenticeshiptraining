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

public class WhenPostingCourseProviders
{
    [Test, MoqAutoData]
    public void CourseProvidersPost_UpdatesLocationCookie_AndRedirects(
        CourseProvidersSubmitModel submitModel,
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
    public async Task CourseProviderDetailsPost_UpdatesLocationCookie_AndRedirects(
        CourseProviderViewModel model,
        string larsCode,
        int providerId,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        [Greedy] CourseProvidersController controller)
    {
        // Act
        var result = await controller.ApplyLocation(model, larsCode, providerId) as RedirectToRouteResult;

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.RouteName, Is.EqualTo(RouteNames.CourseProviderDetails));
            Assert.That(result.RouteValues, Is.Not.Null);
            Assert.That(result.RouteValues!["providerId"].ToString(), Is.EqualTo(providerId.ToString()));
            Assert.That(result.RouteValues!["larsCode"].ToString(), Is.EqualTo(larsCode));
        }

        locationCookieService.Verify(x => x.Update(
            Constants.LocationCookieName,
            It.Is<LocationCookieItem>(c => c.Location == model.Location && c.Distance == model.Distance)
        ), Times.Once);
    }
}
