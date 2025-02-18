using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourses
{
    public class WhenGettingCourses
    {
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_And_The_Data_Returned(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            [Frozen] Mock<ILevelsService> mockLevelService,
            [Frozen] Mock<IRoutesService> mockRoutesService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            request.Keyword = null;
            request.RouteIds = null;
            request.Levels = null;
            request.ShortlistUserId = null;
            mockService.Setup(x => x.GetCourses(null, null, null, OrderBy.None, null)).ReturnsAsync(courseResponse);

            mockLevelService.Setup(x => x.GetLevelsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(courseResponse.Levels);

            mockRoutesService.Setup(x => x.GetRoutesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(courseResponse.Sectors);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(null, null, null, OrderBy.None, null), Times.Once);
            actual.Should().NotBeNull();
            actual.Courses.Should().BeEquivalentTo(courseResponse.Courses);
            actual.Routes.Should().BeEquivalentTo(courseResponse.Sectors);
            actual.TotalFiltered.Should().Be(courseResponse.TotalFiltered);
            actual.Total.Should().Be(courseResponse.Total);
            actual.ShortlistItemCount.Should().Be(courseResponse.ShortlistItemCount);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_Passing_Keywords_From_Query(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            [Frozen] Mock<ILevelsService> mockLevelService,
            [Frozen] Mock<IRoutesService> mockRoutesService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            request.RouteIds = null;
            request.Levels = null;
            mockService.Setup(x => x.GetCourses(request.Keyword, null, null, OrderBy.None, request.ShortlistUserId)).ReturnsAsync(courseResponse);

            mockLevelService.Setup(x => x.GetLevelsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(courseResponse.Levels);

            mockRoutesService.Setup(x => x.GetRoutesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(courseResponse.Sectors);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(request.Keyword, null, null, OrderBy.None, request.ShortlistUserId), Times.Once);
            actual.Should().NotBeNull();
            actual.Courses.Should().BeEquivalentTo(courseResponse.Courses);
            actual.Routes.Should().BeEquivalentTo(courseResponse.Sectors);
            actual.TotalFiltered.Should().Be(courseResponse.TotalFiltered);
            actual.Total.Should().Be(courseResponse.Total);
        }

        [Test, MoqAutoData]
        public async Task Then_Keywords_And_Sectors_Are_Passed_To_The_Service_And_Levels_Is_Null(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            request.Levels = null;
            mockService.Setup(x => x.GetCourses(request.Keyword, request.RouteIds, null, OrderBy.None, request.ShortlistUserId)).ReturnsAsync(courseResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(request.Keyword, request.RouteIds, null, OrderBy.None, request.ShortlistUserId), Times.Once);
            actual.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_Keywords_And_Levels_Are_Passed_To_The_Service_And_Sectors_Is_Null(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            request.RouteIds = null;
            mockService.Setup(x => x.GetCourses(request.Keyword, null, request.Levels, OrderBy.None, request.ShortlistUserId)).ReturnsAsync(courseResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(request.Keyword, null, request.Levels, OrderBy.None, request.ShortlistUserId), Times.Once);
            actual.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_Keywords_And_Levels_And_Sectors_Are_Passed_To_The_Service_And_ShortlistId_Null(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            request.ShortlistUserId = null;
            mockService.Setup(x => x.GetCourses(request.Keyword, request.RouteIds, request.Levels, OrderBy.None, null)).ReturnsAsync(courseResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(request.Keyword, request.RouteIds, request.Levels, OrderBy.None, null), Times.Once);
            actual.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_Keywords_And_Levels_And_Sectors_Are_Passed_To_The_Service(
            GetCoursesQuery request,
            TrainingCourses courseResponse,
            [Frozen] Mock<ICourseService> mockService,
            GetCoursesQueryHandler handler)
        {
            //Arrange
            mockService.Setup(x => x.GetCourses(request.Keyword, request.RouteIds, request.Levels, OrderBy.None, request.ShortlistUserId)).ReturnsAsync(courseResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourses(request.Keyword, request.RouteIds, request.Levels, OrderBy.None, request.ShortlistUserId), Times.Once);
            actual.Should().NotBeNull();
        }
    }
}
