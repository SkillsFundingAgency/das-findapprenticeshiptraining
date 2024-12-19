using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;

using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourse
{
    public class WhenGettingCourse
    {
        [Test, MoqAutoData]
        public async Task Then_If_The_Query_Is_Valid_The_Service_Is_Called_And_The_Data_Returned(
            GetCourseQuery request,
            TrainingCourse courseResponse,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseQueryHandler handler)
        {
            //Arrange
            mockService.Setup(x => x.GetCourse(request.CourseId, request.Lat, request.Lon, request.LocationName, request.ShortlistUserId)).ReturnsAsync(courseResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourse(request.CourseId, request.Lat, request.Lon, request.LocationName, request.ShortlistUserId), Times.Once);
            Assert.IsNotNull(actual);
            actual.Course.Should().BeEquivalentTo(courseResponse.Course);
            actual.ProvidersCount.Should().BeEquivalentTo(courseResponse.ProvidersCount);
            actual.ShortlistItemCount.Should().Be(courseResponse.ShortlistItemCount);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Course_Returns_Null(
            GetCourseQuery request,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseQueryHandler handler)
        {
            //Arrange
            mockService.Setup(x => x.GetCourse(request.CourseId, request.Lat, request.Lon, request.LocationName, request.ShortlistUserId)).ReturnsAsync((TrainingCourse)null);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourse(request.CourseId, request.Lat, request.Lon, request.LocationName, request.ShortlistUserId), Times.Once);
            Assert.IsNull(actual.Course);
            Assert.IsNull(actual.ProvidersCount);
            Assert.AreEqual(0, actual.ShortlistItemCount);
        }
    }
}
