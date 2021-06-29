using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
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

namespace SFA.DAS.FAT.Web.UnitTests.Filters
{
    public class WhenAddingGoogleAnalyticsInformation
    {
        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Has_A_Location_Adds_The_Location_To_The_ViewBag(
            string location,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Greedy] CoursesController controller,
            GoogleAnalyticsFilter filter)
        {
            //Arrange
            var context =
                SetupContextAndCookieLocations(controller, location, null, cookieStorageService);

            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            Assert.AreEqual(location, viewBag.Location);
        }

        [Test, MoqAutoData]
        public async Task
            Then_If_No_Location_In_Context_And_Location_In_Cookie_Adds_The_Location_From_The_Cookie_To_The_ViewBag(
                LocationCookieItem location,
                [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
                [Greedy] CoursesController controller,
                GoogleAnalyticsFilter filter)
        {
            //Arrange
            var context = SetupContextAndCookieLocations(controller, null, location, cookieStorageService);

            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            Assert.AreEqual(location.Name, viewBag.Location);
        }
        [Test, MoqAutoData]
        public async Task
            Then_If_Location_In_Context_And_Location_In_Cookie_Adds_The_Location_From_The_Context_To_The_ViewBag(
                string location,
                LocationCookieItem cookieLocation,
                [Greedy] CoursesController controller,
                [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
                GoogleAnalyticsFilter filter)
        {
            //Arrange
            var context = SetupContextAndCookieLocations(controller, location, cookieLocation, cookieStorageService);

            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            Assert.AreEqual(location, viewBag.Location);
        }

        [Test, MoqAutoData]
        public async Task Then_If_No_Location_In_Context_Or_Cookie_Location_Is_Not_Added_To_The_ViewBag(
            [Greedy] CoursesController controller,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            GoogleAnalyticsFilter filter)
        {
            // Arrange
            var context = SetupContextAndCookieLocations(controller, null, null, cookieStorageService);

            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            viewBag.Should().BeEquivalentTo(new GaData());
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_A_ProviderId_Then_The_Data_Query_Param_Is_Checked_And_Decoded(
            int providerPosition,
            int providerCount,
            uint providerId,
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
            var context = SetupContextAndCookieLocations(controller, null, null, cookieStorageService, providerId.ToString(), Convert.ToBase64String(encodedData));
            
            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            viewBag.ProviderTotal.Should().Be(providerCount);
            viewBag.ProviderPlacement.Should().Be(providerPosition);
            viewBag.ProviderId.Should().Be(providerId);
        }

        [Test, MoqAutoData]
        public async Task Then_If_It_Is_Unable_To_Unprotect_The_Value_Then_Nothing_Is_Added(
            int providerPosition,
            int providerCount,
            uint providerId,
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
            var context = SetupContextAndCookieLocations(controller, null, null, cookieStorageService, providerId.ToString(), Convert.ToBase64String(encodedData));
            
            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            viewBag.ProviderTotal.Should().Be(0);
            viewBag.ProviderPlacement.Should().Be(0);
            viewBag.ProviderId.Should().Be(0);
        }

        [Test, MoqAutoData]
        public async Task Then_If_It_Is_An_Invalid_Base64_String_Then_Nothing_Is_Added(
            int providerPosition,
            int providerCount,
            uint providerId,
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
            var context = SetupContextAndCookieLocations(controller, null, null, cookieStorageService, providerId.ToString(), data);
            
            //Act
            await filter.OnActionExecutionAsync(context, Mock.Of<ActionExecutionDelegate>());

            //Assert
            var viewBag = controller.ViewBag.GaData as GaData;
            Assert.IsNotNull(viewBag);
            viewBag.ProviderTotal.Should().Be(0);
            viewBag.ProviderPlacement.Should().Be(0);
            viewBag.ProviderId.Should().Be(0);
        }

        private ActionExecutingContext SetupContextAndCookieLocations(CoursesController controller, string location,
            LocationCookieItem cookieLocation, Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService, string providerId = "", string data = "")
        {
            cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName))
                .Returns(cookieLocation);

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var queryString = "";
            if (!string.IsNullOrEmpty(location))
            {
                queryString += $"?location={location}";
            }

            if (!string.IsNullOrEmpty(providerId))
            {
                routeData.Values.Add("providerId",providerId);
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
}
