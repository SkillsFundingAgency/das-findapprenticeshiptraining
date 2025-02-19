using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourses
    {
        //[Test, MoqAutoData]
        //public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
        //    GetCoursesRequest request,
        //    GetCoursesQueryResult response,
        //    ShortlistCookieItem cookieItem,
        //    [Frozen] Mock<IMediator> mediator,
        //    [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        //    [Greedy] CoursesController controller)
        //{
        //    //Arrange
        //    controller.AddUrlHelperMock()
        //        .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
        //        .AddUrlForRoute(RouteNames.ShortList, Guid.NewGuid().ToString());

        //    mediator.Setup(x =>
        //            x.Send(It.Is<GetCoursesQuery>(c =>
        //                c.Keyword.Equals(request.Keyword)
        //                && c.ShortlistUserId.Equals(cookieItem.ShortlistUserId)), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(response);
        //    shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
        //        .Returns(cookieItem);

        //    //Act
        //    var actual = await controller.Courses(request);
        //    var actualResult = actual as ViewResult;

        //    //Assert
        //    actual.Should().NotBeNull();
        //    actualResult.Should().NotBeNull();
        //}

        //[Test, MoqAutoData]
        //public async Task Then_The_Keyword_And_Sectors_And_Levels_And_Location_Are_Added_To_The_Query_And_Returned_To_The_View(
        //    GetCoursesRequest request,
        //    GetCoursesResult response,
        //    Guid shortlistUrl,
        //    [Frozen] Mock<IMediator> mediator,
        //    [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
        //    [Greedy] CoursesController controller)
        //{
        //    //Arrange
        //    controller.AddUrlHelperMock()
        //        .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
        //        .AddUrlForRoute(RouteNames.ShortList, shortlistUrl.ToString());

        //    mediator.Setup(x =>
        //            x.Send(It.Is<GetCoursesQuery>(c
        //                => c.Keyword.Equals(request.Keyword)
        //                && c.RouteIds.Equals(request.Categories)
        //                && c.Levels.Equals(request.Levels)), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(response);
        //    shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
        //        .Returns((ShortlistCookieItem)null);


        //    //Act
        //    var actual = await controller.Courses(request);

        //    //Assert
        //    using (new AssertionScope())
        //    {
        //        actual.Should().NotBeNull();
        //        var actualResult = actual as ViewResult;
        //        actualResult.Should().NotBeNull();
        //        var actualModel = actualResult!.Model as CoursesViewModel;
        //        actualModel.Should().NotBeNull();
        //        actualModel!.Courses.Should()
        //            .BeEquivalentTo(response.Courses, options => options.Including(course => course.Id));
        //        actualModel.Routes.Should().BeEquivalentTo(response.Routes);
        //        actualModel.Levels.Should().BeEquivalentTo(response.Levels);
        //        actualModel.Keyword.Should().Be(request.Keyword);
        //        actualModel.SelectedLevels.Should().BeEquivalentTo(request.Levels);
        //        actualModel.SelectedRoutes.Should().BeEquivalentTo(request.Categories);
        //        actualModel.Total.Should().Be(response.Total);
        //        actualModel.TotalFiltered.Should().Be(response.TotalFiltered);
        //        actualModel.ShortListItemCount.Should().Be(response.ShortlistItemCount);
        //        actualModel.Location.Should().Be(request.Location);
        //        actualModel.ShowShortListLink.Should().BeTrue();
        //        actualModel.ShowSearchCrumb.Should().BeTrue();
        //        actualModel.CourseId.Should().Be(0);
        //        actualModel.ShowApprenticeTrainingCourseCrumb.Should().BeFalse();
        //        actualModel.ShowApprenticeTrainingCoursesCrumb.Should().BeFalse();
        //        actualModel.ShowApprenticeTrainingCourseProvidersCrumb.Should().BeFalse();
        //    }
        //}

        //[Test, MoqAutoData]
        //public async Task Then_Any_Sectors_In_The_Request_Are_Marked_As_Selected_On_The_ViewModel(
        //    GetCoursesRequest request,
        //    GetCoursesResult response,
        //    [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
        //    [Frozen] Mock<IMediator> mediator,
        //    [Greedy] CoursesController controller)
        //{
        //    //Arrange
        //    controller.AddUrlHelperMock()
        //        .AddUrlForRoute(RouteNames.ServiceStart, Guid.NewGuid().ToString())
        //        .AddUrlForRoute(RouteNames.ShortList, Guid.NewGuid().ToString());
        //    request.Location = "";

        //    response.Routes = new List<Route>() {
        //        new Route 
        //        {
        //            Name = request.Categories[0]
        //        },
        //        new Route
        //        {
        //            Name = request.Categories[1]
        //        }
        //    };

        //    mediator.Setup(x =>
        //            x.Send(It.Is<GetCoursesQuery>(c
        //                => c.Keyword.Equals(request.Keyword)
        //                   && c.RouteIds.Equals(request.Categories)), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(response);
        //    locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns((LocationCookieItem)null);

        //    //Act
        //    var actual = await controller.Courses(request);

        //    //Assert
        //    using (new AssertionScope())
        //    {
        //        actual.Should().NotBeNull();
        //        var actualResult = actual as ViewResult;
        //        actualResult.Should().NotBeNull();
        //        var actualModel = actualResult!.Model as CoursesViewModel;
        //        actualModel.Should().NotBeNull();
        //        actualModel!.Routes.Count(sector => sector.Selected).Should().Be(2);
        //        actualModel.Routes.SingleOrDefault(c => c.Name.Equals(request.Categories.First())).Should().NotBeNull();
        //        actualModel.Routes.SingleOrDefault(c => c.Name.Equals(request.Categories.Skip(1).First())).Should()
        //            .NotBeNull();
        //        actualModel.Location.Should().BeEmpty();
        //    }
        //}
    }
}
