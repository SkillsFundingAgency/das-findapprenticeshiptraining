using System.Security.Cryptography;
using System.Text;
using AutoFixture.NUnit4;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Filters;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Filters;

public class GoogleAnalyticsFilterTests
{
    [Test, MoqAutoData]
    public async Task WhenExecutingGoogleAnalyticsFilter_AndHasLocationCookie_ThenAddsLocationToViewBag(
        LocationCookieItem locationCookie,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
        [Greedy] CoursesController controller,
        GoogleAnalyticsFilter filter)
    {
        //Arrange
        var context =
            SetupContextAndCookieLocations(controller, locationCookie, cookieStorageService);

        //Act
        await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

        //Assert
        var viewBag = controller.ViewBag.GaData as GaData;
        viewBag.Should().NotBeNull();
        viewBag!.Location.Should().Be(locationCookie.Location);
    }

    [Test, MoqAutoData]
    public async Task
        WhenExecutingGoogleAnalyticsFilter_AndHasNoLocationCookie_ThenNoLocationAddedToViewBag(
            LocationCookieItem locationCookie,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Greedy] CoursesController controller,
            GoogleAnalyticsFilter filter)
    {
        //Arrange
        locationCookie = null;
        var context = SetupContextAndCookieLocations(controller, locationCookie, cookieStorageService);

        //Act
        await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

        //Assert
        var viewBag = controller.ViewBag.GaData as GaData;
        viewBag.Should().NotBeNull();
        viewBag!.Location.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task WhenExecutingGoogleAnalyticsFilter_AndUkprnPresent_ThenDataQueryParamIsCheckedAndDecoded(
        int providerPosition,
        int providerCount,
        uint ukprn,
        [Greedy] CoursesController controller,
        [Frozen] Mock<IDataProtector> protector,
        [Frozen] Mock<IDataProtectionProvider> provider,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
        GoogleAnalyticsFilter filter)
    {
        // Arrange
        var encodedData = Encoding.UTF8.GetBytes($"{providerPosition}|{providerCount}");
        protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
        provider.Setup(x => x.CreateProtector(Constants.GaDataProtectorName)).Returns(protector.Object);
        var context = SetupContextAndCookieLocations(controller, null, cookieStorageService, ukprn.ToString(), Convert.ToBase64String(encodedData));

        //Act
        await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

        //Assert
        var viewBag = controller.ViewBag.GaData as GaData;

        viewBag.Should().NotBeNull();
        viewBag!.ProviderTotal.Should().Be(providerCount);
        viewBag.ProviderPlacement.Should().Be(providerPosition);
        viewBag.Ukprn.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task WhenExecutingGoogleAnalyticsFilter_AndUnprotectThrows_ThenNoGaDataAdded(
        int providerPosition,
        int providerCount,
        uint ukprn,
        [Greedy] CoursesController controller,
        [Frozen] Mock<IDataProtector> protector,
        [Frozen] Mock<IDataProtectionProvider> provider,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
        GoogleAnalyticsFilter filter)
    {
        // Arrange
        var encodedData = Encoding.UTF8.GetBytes($"{providerPosition}|{providerCount}");
        protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Throws<CryptographicException>();
        provider.Setup(x => x.CreateProtector(Constants.GaDataProtectorName)).Returns(protector.Object);
        var context = SetupContextAndCookieLocations(controller, null, cookieStorageService, ukprn.ToString(), Convert.ToBase64String(encodedData));

        //Act
        await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

        //Assert
        var viewBag = controller.ViewBag.GaData as GaData;
        viewBag.Should().NotBeNull();
        viewBag!.ProviderTotal.Should().Be(0);
        viewBag.ProviderPlacement.Should().Be(0);
        viewBag.Ukprn.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task WhenExecutingGoogleAnalyticsFilter_AndInvalidBase64Data_ThenNoGaDataAdded(
        int providerPosition,
        int providerCount,
        uint ukprn,
        [Greedy] CoursesController controller,
        [Frozen] Mock<IDataProtector> protector,
        [Frozen] Mock<IDataProtectionProvider> provider,
        [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
        GoogleAnalyticsFilter filter)
    {
        // Arrange
        var data = $"{providerPosition}|{providerCount}";
        var encodedData = Encoding.UTF8.GetBytes(data);
        protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
        provider.Setup(x => x.CreateProtector(Constants.GaDataProtectorName)).Returns(protector.Object);
        var context = SetupContextAndCookieLocations(controller, null, cookieStorageService, ukprn.ToString(), data);

        //Act
        await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

        //Assert
        var viewBag = controller.ViewBag.GaData as GaData;
        viewBag.Should().NotBeNull();
        viewBag!.ProviderTotal.Should().Be(0);
        viewBag.ProviderPlacement.Should().Be(0);
        viewBag.Ukprn.Should().Be(0);
    }

    private static ActionExecutingContext SetupContextAndCookieLocations(CoursesController controller, LocationCookieItem cookieLocation, Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService, string ukprn = "", string data = "")
    {
        cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName))
            .Returns(cookieLocation);

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var queryString = "";

        if (!string.IsNullOrEmpty(ukprn))
        {
            routeData.Values.Add("ukprn", ukprn);
            if (!string.IsNullOrEmpty(data))
            {
                if (string.IsNullOrEmpty(queryString))
                {
                    queryString += $"?data={data}";
                }
                else
                {
                    queryString += $"&data={data}";
                }
            }
        }

        if (!string.IsNullOrEmpty(queryString))
        {
            httpContext.Request.QueryString = new QueryString(queryString);
        }


        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };


        var actionContext = new ActionContext(
            httpContext,
            routeData,
            Mock.Of<ActionDescriptor>(),
            new ModelStateDictionary()
        );

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            controller
        );
    }
}
