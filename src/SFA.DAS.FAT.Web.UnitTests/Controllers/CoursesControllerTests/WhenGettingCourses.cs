using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourses
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
            GetCoursesRequest request,
            GetCoursesResult response,
            ShortlistCookieItem cookieItem,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            mediator.Setup(x => 
                    x.Send(It.Is<GetCoursesQuery>(c => 
                        c.Keyword.Equals(request.Keyword)
                        && c.ShortlistUserId.Equals(cookieItem.ShortlistUserId)),It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns(cookieItem);
            
            //Act
            var actual = await controller.Courses(request);
            var actualResult = actual as ViewResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actualResult);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_Sectors_And_Levels_And_Location_Are_Added_To_The_Query_And_Returned_To_The_View(
            GetCoursesRequest request,
            GetCoursesResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<ShortlistCookieItem>> shortlistCookieService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            mediator.Setup(x => 
                    x.Send(It.Is<GetCoursesQuery>(c 
                        => c.Keyword.Equals(request.Keyword)
                        && c.RouteIds.Equals(request.Sectors)
                        && c.Levels.Equals(request.Levels)),It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            shortlistCookieService.Setup(x => x.Get(Constants.ShortlistCookieName))
                .Returns((ShortlistCookieItem)null);
            

            //Act
            var actual = await controller.Courses(request);

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CoursesViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.Courses.Should().BeEquivalentTo(response.Courses, options => options.Including(course => course.Id));
            actualModel.Sectors.Should().BeEquivalentTo(response.Sectors);
            actualModel.Levels.Should().BeEquivalentTo(response.Levels);
            actualModel.Keyword.Should().Be(request.Keyword);
            actualModel.SelectedLevels.Should().BeEquivalentTo(request.Levels);
            actualModel.SelectedSectors.Should().BeEquivalentTo(request.Sectors);
            actualModel.Total.Should().Be(response.Total);
            actualModel.TotalFiltered.Should().Be(response.TotalFiltered);
            actualModel.ShortlistItemCount.Should().Be(response.ShortlistItemCount);
            actualModel.Location.Should().Be(request.Location);
        }

        [Test, MoqAutoData]
        public async Task Then_Any_Sectors_In_The_Request_Are_Marked_As_Selected_On_The_ViewModel(
            GetCoursesRequest request,
            GetCoursesResult response,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            request.Location = "";
            response.Sectors.Add(new Sector
            {
                Route = request.Sectors.First()
            });
            response.Sectors.Add(new Sector
            {
                Route = request.Sectors.Skip(1).First()
            });
            mediator.Setup(x => 
                    x.Send(It.Is<GetCoursesQuery>(c 
                        => c.Keyword.Equals(request.Keyword)
                           && c.RouteIds.Equals(request.Sectors)),It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns((LocationCookieItem)null);

            //Act
            var actual = await controller.Courses(request);
            
            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CoursesViewModel;
            Assert.IsNotNull(actualModel);
            Assert.AreEqual(2, actualModel.Sectors.Count(sector=>sector.Selected));
            Assert.IsNotNull(actualModel.Sectors.SingleOrDefault(c=>c.Route.Equals(request.Sectors.First())));
            Assert.IsNotNull(actualModel.Sectors.SingleOrDefault(c=>c.Route.Equals(request.Sectors.Skip(1).First())));
            actualModel.Location.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task Then_The_Location_Cookie_Is_Checked_And_Added_To_Model(
            GetCoursesRequest request,
            GetCoursesResult response,
            LocationCookieItem cookieItem,
            [Frozen] Mock<ICookieStorageService<LocationCookieItem>> locationCookieService,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            request.Location = string.Empty;
            mediator.Setup(x => 
                    x.Send(It.Is<GetCoursesQuery>(c 
                        => c.Keyword.Equals(request.Keyword)
                           && c.RouteIds.Equals(request.Sectors)
                           && c.Levels.Equals(request.Levels)),It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            locationCookieService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(cookieItem);
            
            //Act
            var actual = await controller.Courses(request);
            
            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CoursesViewModel;
            Assert.IsNotNull(actualModel);
            actualModel.Location.Should().Be(cookieItem.Name);
        }
    }
}