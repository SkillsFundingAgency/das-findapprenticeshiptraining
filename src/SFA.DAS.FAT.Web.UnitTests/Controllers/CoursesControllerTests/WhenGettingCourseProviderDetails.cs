using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.FAT.Web.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourseProviderDetails
    {

        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Provider_Detail_Retrieved_And_Shown_With_Shortlist(
            int providerId,
            int courseId,
            string location,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) 
                && c.CourseId.Equals(courseId) 
                && c.Location.Equals(location)
                && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, "", "");

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            Assert.IsNotNull(actualModel.AdditionalCourses);
            Assert.IsNotNull(actualModel.AdditionalCourses.Courses);
            actualModel.Location.Should().BeNullOrEmpty();
            cookieStorageService.Verify(x=>x.Update(Constants.LocationCookieName,It.IsAny<LocationCookieItem>(), It.IsAny<int>()), Times.Never);
            
        }

        [Test, MoqAutoData]
        public async Task Then_The_Location_Is_Added_To_The_Cookie_If_Set(
            int providerId,
            int courseId,
            string location,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName)).Returns((LocationCookieItem)null);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                    c.ProviderId.Equals(providerId) && c.CourseId.Equals(courseId) && c.Location.Equals(location)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns((ShortlistCookieItem) null);
            
            //Act
            var result = await controller.CourseProviderDetail(courseId, providerId, location, "", "") as ViewResult;
            
            //Assert
            var model = result!.Model as CourseProviderViewModel;
            model!.GetCourseProvidersRequest[nameof(GetCourseProvidersRequest.Location)].Should().Be(response.Location);
            cookieStorageService.Verify(x=>x.Update(Constants.LocationCookieName,It.Is<LocationCookieItem>(c=>
                c.Name.Equals(response.Location)
                && c.Lat.Equals(response.LocationGeoPoint.FirstOrDefault())
                && c.Lon.Equals(response.LocationGeoPoint.LastOrDefault())
                ),2));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Location_Is_Removed_From_The_Cookie_If_Set_To_Minus_One(
            int providerId,
            int courseId,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProviderQuery>(c => 
                        c.ProviderId.Equals(providerId) && 
                        c.CourseId.Equals(courseId) && 
                        c.Location.Equals("")), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "-1", "", "") as ViewResult;
            
            //Assert
            cookieStorageService.Verify(x=>x.Delete(Constants.LocationCookieName));
            var model = actual!.Model as CourseProviderViewModel;
            model!.GetCourseProvidersRequest[nameof(GetCourseProvidersRequest.Location)].Should().Be(response.Location);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_Location_Stored_In_Cookie_And_No_Location_In_Query_It_Is_Used_For_Results_And_Cookie_Updated(
            int providerId,
            int courseId,
            LocationCookieItem location,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(location);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                    c.ProviderId.Equals(providerId) 
                    && c.CourseId.Equals(courseId) 
                    && c.Location.Equals(location.Name)
                    && c.Lat.Equals(location.Lat)
                    && c.Lon.Equals(location.Lon)
                    ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "", "","");
            
            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            cookieStorageService.Verify(x=>x.Update(Constants.LocationCookieName,It.Is<LocationCookieItem>(c=>c.Name.Equals(response.Location)),2));
        }

        [Test, MoqAutoData]
        public async Task Then_ProviderFilters_Populated_From_Cookie_And_Id_From_Request_If_Matches_CookieValue(
            int providerId,
            int courseId,
            GetCourseProvidersRequest providersRequest,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<GetCourseProvidersRequest>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            providersRequest.Id = courseId;
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            cookieStorageService
                .Setup(x => x.Get(Constants.ProvidersCookieName))
                .Returns(providersRequest);
            mediator
                .Setup(x => x.Send(
                    It.IsAny<GetCourseProviderQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "", "","") as ViewResult;
            
            //Assert
            var model = actual!.Model as CourseProviderViewModel;
            model!.GetCourseProvidersRequest.Where(key=>key.Key!="Id").Should().BeEquivalentTo(providersRequest.ToDictionary().Where(key=>key.Key!="Id"));
            
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Course_Differs_From_The_Cookie_Request_The_Filter_And_Id_Are_Updated_And_Filters_Cleared(
            int providerId,
            int courseId,
            GetCourseProvidersRequest providersRequest,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<GetCourseProvidersRequest>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            cookieStorageService
                .Setup(x => x.Get(Constants.ProvidersCookieName))
                .Returns(providersRequest);
            mediator
                .Setup(x => x.Send(
                    It.IsAny<GetCourseProviderQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "", "","") as ViewResult;
            
            //Assert
            var model = actual!.Model as CourseProviderViewModel;
            model!.GetCourseProvidersRequest["Id"].Should().Be(courseId.ToString());
            model!.GetCourseProvidersRequest.ContainsKey("DeliveryModes[0]").Should().BeFalse();
            model!.GetCourseProvidersRequest.ContainsKey("ProviderRatings[0]").Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task And_Error_Then_Redirect_To_Error_Route(
            int providerId,
            int courseId,
            string location,
            Exception exception,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller
            )
        {
            // Arrange
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) && c.CourseId.Equals(courseId)), 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, "","") as RedirectToRouteResult;

            // Assert
            actual!.RouteName.Should().Be(RouteNames.Error500);
        }

        [Test, MoqAutoData]
        public async Task Then_No_Course_Returns_Page_Not_Found(
            int providerId,
            int courseId,
            GetCourseProvidersRequest providersRequest,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<GetCourseProvidersRequest>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course = null;
            cookieStorageService
                .Setup(x => x.Get(Constants.ProvidersCookieName))
                .Returns(providersRequest);
            mediator
                .Setup(x => x.Send(
                    It.IsAny<GetCourseProviderQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "", "","") as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(actual);
            actual.RouteName.Should().Be(RouteNames.Error404);
        }

        [Test, MoqAutoData]
        public async Task Then_If_Course_Is_After_Last_Start_Then_Redirected_To_Course_Page(
            int providerId,
            int courseId,
            GetCourseProvidersRequest providersRequest,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<GetCourseProvidersRequest>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(-1);
            cookieStorageService
                .Setup(x => x.Get(Constants.ProvidersCookieName))
                .Returns(providersRequest);
            mediator
                .Setup(x => x.Send(
                    It.IsAny<GetCourseProviderQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, "", "","") as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(actual);
            actual.RouteName.Should().Be(RouteNames.CourseDetails);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Removed_Is_In_The_Request_It_Is_Decoded_And_Added_To_ViewModel(
            int providerId,
            int courseId,
            string location,
            string removed,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) 
                && c.CourseId.Equals(courseId) 
                && c.Location.Equals(location)
                && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, WebEncoders.Base64UrlEncode(encodedData),"") as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.BannerUpdateMessage.Should().Be($"{removed} removed from shortlist.");
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Added_Is_In_The_Request_It_Is_Decoded_And_Added_To_ViewModel(
            int providerId,
            int courseId,
            string location,
            string added,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            var encodedData = Encoding.UTF8.GetBytes($"{added}");
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) 
                && c.CourseId.Equals(courseId) 
                && c.Location.Equals(location)
                && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, "",WebEncoders.Base64UrlEncode(encodedData)) as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.BannerUpdateMessage.Should().Be($"{added} added to shortlist.");
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Removed_And_Added_Is_In_The_Request_And_Throws_Cryptographic_Exception_Then_Empty_String_Is_Added_To_ViewModel(
            int providerId,
            int courseId,
            string location,
            string removed,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Throws(new CryptographicException());
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) 
                && c.CourseId.Equals(courseId) 
                && c.Location.Equals(location)
                && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, WebEncoders.Base64UrlEncode(encodedData),WebEncoders.Base64UrlEncode(encodedData)) as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.BannerUpdateMessage.Should().BeEmpty();
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Removed_And_Added_Is_In_The_Request_And_Is_Invalid_Then_Empty_String_Is_Added_To_ViewModel(
            int providerId,
            int courseId,
            string location,
            string removed,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            var encodedData = Encoding.UTF8.GetBytes($"{removed}");
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            protector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(encodedData);
            provider.Setup(x => x.CreateProtector(Constants.ShortlistProtectorName)).Returns(protector.Object);
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) 
                && c.CourseId.Equals(courseId) 
                && c.Location.Equals(location)
                && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, encodedData.ToString(),encodedData.ToString()) as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.BannerUpdateMessage.Should().BeEmpty();
        }
        
        
        [Test, MoqAutoData]
        public async Task Then_The_Help_Url_Is_Built_From_Config_If_Feature_Enabled_And_Show_Demand_Is_Returned(
            int providerId,
            int courseId,
            string location,
            string removed,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingWeb>> config,
            [Greedy] CoursesController controller)
        {
            //Arrange
            config.Object.Value.EmployerDemandFeatureToggle = true;
            response.ShowEmployerDemand = true;
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                    c.ProviderId.Equals(providerId) 
                    && c.CourseId.Equals(courseId) 
                    && c.Location.Equals(location)
                    && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);
            
            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, "", "") as ViewResult;
            
            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.HelpFindingCourseUrl.Should().Be($"{config.Object.Value.EmployerDemandUrl}/registerdemand/course/{actualModel.Course.Id}/share-interest");
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Help_Url_Set_If_Feature_Disabled(
            int providerId,
            int courseId,
            string location,
            string removed,
            GetCourseProviderResult response,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Frozen] Mock<IDataProtector> protector,
            [Frozen] Mock<IDataProtectionProvider> provider,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingWeb>> config,
            [Greedy] CoursesController controller)
        {
            //Arrange
            config.Object.Value.EmployerDemandFeatureToggle = false;
            response.ShowEmployerDemand = true;
            response.Course.StandardDates.LastDateStarts = DateTime.UtcNow.AddDays(5);
            response.Location=string.Empty;
            response.LocationGeoPoint = null;
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                    c.ProviderId.Equals(providerId) 
                    && c.CourseId.Equals(courseId) 
                    && c.Location.Equals(location)
                    && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);
            
            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location, "", "") as ViewResult;
            
            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.HelpFindingCourseUrl.Should().Be("https://help.apprenticeships.education.gov.uk/hc/en-gb#contact-us");
        }
    }
}
