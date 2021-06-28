using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Services
{
    public class WhenGettingACourseProviderDetails
    {
        [Test, MoqAutoData]
        public async Task Then_The_Response_Is_Returned_From_The_Api(
            int providerId,
            int courseId,
            string location,
            string baseUrl,
            double lat,
            double lon,
            Guid shortlistUserId,
            TrainingCourseProviderDetails apiResponse,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            apiClient.Setup(x => x.Get<TrainingCourseProviderDetails>(
                It.Is<GetCourseProviderDetailsApiRequest>(request =>
                    request.GetUrl.Contains(courseId.ToString())
                    && request.GetUrl.Contains(providerId.ToString())
                    && request.GetUrl.Contains(location)
                    && request.GetUrl.Contains(shortlistUserId.ToString())
                    ))).ReturnsAsync(apiResponse);
            //Act
            var actual = await courseService.GetCourseProviderDetails(providerId, courseId, location, lat, lon, shortlistUserId);
            //Assert
            actual.Should().BeEquivalentTo(apiResponse);
        }
    }
}
