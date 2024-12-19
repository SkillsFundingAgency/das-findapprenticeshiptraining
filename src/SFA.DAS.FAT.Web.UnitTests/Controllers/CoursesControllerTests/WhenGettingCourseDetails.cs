using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourseDetails
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
            int standardCode,
            GetCourseResult response,
            LocationCookieItem locationCookieItem,
            ShortlistCookieItem shortlistCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistStorageService,
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            cookieStorageService
                .Setup(x => x.Get(Constants.LocationCookieName))
                .Returns(locationCookieItem);
            shortlistStorageService
                .Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(shortlistCookieItem);
            mediator
                .Setup(x =>
                    x.Send(It.Is<GetCourseQuery>(c =>
                        c.CourseId.Equals(standardCode)
                        && c.ShortlistUserId.Equals(shortlistCookieItem.ShortlistUserId)
                        && c.Lat.Equals(locationCookieItem.Lat)
                        && c.Lon.Equals(locationCookieItem.Lon)
                        && c.LocationName.Equals(locationCookieItem.Name)
                        ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            validator.Setup(v =>
                v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());


            //Act
            var actual = await controller.CourseDetail(standardCode, "");

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CourseViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.TotalProvidersCount.Should().Be(response.ProvidersCount.TotalProviders);
            actualModel.ProvidersAtLocationCount.Should().Be(response.ProvidersCount.ProvidersAtLocation);
            actualModel.LocationName.Should().Be(locationCookieItem.Name);
            actualModel.ShortlistItemCount.Should().Be(response.ShortlistItemCount);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_A_Location_Cookie_The_Lat_Lon_And_Name_Are_Passed_To_The_Query(
            int standardCode,
            GetCourseResult response,
            LocationCookieItem locationCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistStorageService,
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName))
                .Returns(locationCookieItem);
            shortlistStorageService
                .Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns((ShortlistCookieItem)null);
            mediator.Setup(x =>
                    x.Send(It.Is<GetCourseQuery>(c =>
                        c.CourseId.Equals(standardCode)
                        && c.Lat.Equals(locationCookieItem.Lat)
                        && c.Lon.Equals(locationCookieItem.Lon)
                        && c.LocationName.Equals(locationCookieItem.Name)
                        && c.ShortlistUserId == null
                        ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            validator.Setup(v =>
                v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            //Act
            var actual = await controller.CourseDetail(standardCode, "");

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CourseViewModel;
            Assert.IsNotNull(actualModel);
        }

        [Test, MoqAutoData]
        public async Task And_Location_Minus_1_Then_Removes_Location(
            int standardCode,
            string locationName,
            GetCourseResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            locationName = "-1";
            mediator
                .Setup(x => x.Send(
                        It.Is<GetCourseQuery>(c => c.CourseId.Equals(standardCode)),
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

            validator.Setup(v =>
                v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            //Act
            await controller.CourseDetail(standardCode, locationName);

            //Assert
            cookieStorageService.Verify(service => service.Delete(Constants.LocationCookieName));
        }

        [Test, MoqAutoData]
        public async Task Then_If_No_Course_Is_Returned_Redirected_To_Page_Not_Found(
            int standardCode,
            GetCourseResult response,
            LocationCookieItem locationCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            cookieStorageService
                .Setup(x => x.Get(Constants.LocationCookieName))
                .Returns(locationCookieItem);
            mediator
                .Setup(x =>
                    x.Send(It.Is<GetCourseQuery>(c =>
                        c.CourseId.Equals(standardCode)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCourseResult());

            validator.Setup(v =>
                v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            //Act
            var actual = await controller.CourseDetail(standardCode, "");

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(actualResult);
            actualResult.RouteName.Should().Be(RouteNames.Error404);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Help_Url_Is_EmployerDemand_From_Config_If_EmployerDemandFeature_Enabled(
            int standardCode,
            GetCourseResult response,
            LocationCookieItem locationCookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> cookieStorageService,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingWeb>> config,
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            config.Object.Value.EmployerDemandFeatureToggle = true;
            mediator
                .Setup(x => x.Send(
                    It.Is<GetCourseQuery>(c => c.CourseId.Equals(standardCode)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            validator.Setup(v => v.ValidateAsync(It.IsAny<GetCourseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

            //Act
            var actual = await controller.CourseDetail(standardCode, "") as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            var actualModel = actual.Model as CourseViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.GetHelpFindingCourseUrl(config.Object.Value).Should().Be($"{config.Object.Value.EmployerDemandUrl}/registerdemand/course/{actualModel.Id}/share-interest?entrypoint=1");
        }

        [Test, MoqAutoData]
        public void Then_CourseId_Is_Invalid_And_ValidationExceptionThrown(
            [Frozen] Mock<IValidator<GetCourseQuery>> validator,
            [Greedy] CoursesController controller)
        {
            const int courseId = 0;
            Assert.ThrowsAsync<ValidationException>(() => controller.CourseDetail(courseId, ""));
        }
    }
}
