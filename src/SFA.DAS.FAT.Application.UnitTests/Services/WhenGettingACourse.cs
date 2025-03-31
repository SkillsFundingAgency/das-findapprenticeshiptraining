using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public class WhenGettingACourse
{
    [Test, MoqAutoData]
    public async Task Then_The_Api_Client_Is_Called_With_The_Request(
        int larsCode,
        string location,
        int? distance,
        string baseUrl,
        GetCourseResponse response,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        CourseService courseService
    )
    {
        var courseApiRequest = new GetCourseApiRequest(config.Object.Value.BaseUrl, larsCode, location, distance);
        apiClient.Setup(x => 
            x.Get<GetCourseResponse>(
                It.Is<GetCourseApiRequest>(request => request.GetUrl.Equals(courseApiRequest.GetUrl)))).ReturnsAsync(response);

        var sut = await courseService.GetCourse(larsCode, location, distance);

        sut.Should().BeEquivalentTo(response);
    }
}
