using System.Net;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public class WhenGettingCourseProviderDetails
{
    [Test]
    [MoqAutoData]
    public async Task Then_The_Correct_Response_Is_Returned_From_The_Api(
        string baseUrl,
        int ukprn,
        int larsCode,
        string location,
        int? distance,
        Guid shortlistUserId,
        CourseProviderDetailsModel CourseProviderDetailsResponse,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> findApprenticeshipTrainingApiConfigurationMock,
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService sut
    )
    {
        mockApiClient.Setup(x => x.Get<CourseProviderDetailsModel>(
            It.Is<GetCourseProviderDetailsApiRequest>(request =>
                request.GetUrl.Contains(ukprn.ToString()) &&
                request.GetUrl.Contains(larsCode.ToString()) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(shortlistUserId.ToString())
            )
        )).ReturnsAsync(CourseProviderDetailsResponse);

        var result = await sut.GetCourseProvider(
            ukprn,
            larsCode,
            location,
            distance,
            shortlistUserId
        );

        Assert.Multiple(() =>
        {
            Assert.That(result.Ukprn, Is.EqualTo(CourseProviderDetailsResponse.Ukprn));
            Assert.That(result.ProviderName, Is.EqualTo(CourseProviderDetailsResponse.ProviderName));
            Assert.That(result.ProviderAddress, Is.EqualTo(CourseProviderDetailsResponse.ProviderAddress));
            Assert.That(result.Contact, Is.EqualTo(CourseProviderDetailsResponse.Contact));
            Assert.That(result.CourseName, Is.EqualTo(CourseProviderDetailsResponse.CourseName));
            Assert.That(result.Level, Is.EqualTo(CourseProviderDetailsResponse.Level));
            Assert.That(result.LarsCode, Is.EqualTo(CourseProviderDetailsResponse.LarsCode));
            Assert.That(result.IFateReferenceNumber, Is.EqualTo(CourseProviderDetailsResponse.IFateReferenceNumber));
            Assert.That(result.Qar, Is.EqualTo(CourseProviderDetailsResponse.Qar));
            Assert.That(result.Reviews, Is.EqualTo(CourseProviderDetailsResponse.Reviews));
            Assert.That(result.EndpointAssessments, Is.EqualTo(CourseProviderDetailsResponse.EndpointAssessments));
            Assert.That(result.TotalProvidersCount, Is.EqualTo(CourseProviderDetailsResponse.TotalProvidersCount));
            Assert.That(result.ShortlistId, Is.EqualTo(CourseProviderDetailsResponse.ShortlistId));
            Assert.That(result.Locations, Is.EqualTo(CourseProviderDetailsResponse.Locations));
            Assert.That(result.Courses, Is.EqualTo(CourseProviderDetailsResponse.Courses));
            Assert.That(result.AnnualEmployerFeedbackDetails, Is.EqualTo(CourseProviderDetailsResponse.AnnualEmployerFeedbackDetails));
            Assert.That(result.AnnualApprenticeFeedbackDetails, Is.EqualTo(CourseProviderDetailsResponse.AnnualApprenticeFeedbackDetails));
        });
    }

    [Test]
    [MoqAutoData]
    public async Task When_Api_Response_Is_Null_Then_Null_Is_Returned_From_The_Service(
        string baseUrl,
        int ukprn,
        int larsCode,
        string location,
        int? distance,
        Guid shortlistUserId,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> findApprenticeshipTrainingApiConfigurationMock,
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService sut
    )
    {
        mockApiClient.Setup(x => x.Get<CourseProviderDetailsModel>(
            It.Is<GetCourseProviderDetailsApiRequest>(request =>
                request.GetUrl.Contains(ukprn.ToString()) &&
                request.GetUrl.Contains(larsCode.ToString()) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(shortlistUserId.ToString())
            )
        )).ReturnsAsync((CourseProviderDetailsModel)null);

        var result = await sut.GetCourseProvider(
            ukprn,
            larsCode,
            location,
            distance,
            shortlistUserId
        );

        Assert.That(result, Is.Null);
    }

    [Test]
    [MoqAutoData]
    public async Task When_Api_Response_Is_NotFound_Then_Null_Is_Returned_From_The_Service(
        string baseUrl,
        int ukprn,
        int larsCode,
        string location,
        int? distance,
        Guid shortlistUserId,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> findApprenticeshipTrainingApiConfigurationMock,
        [Frozen] Mock<IApiClient> mockApiClient,
        CourseService sut
    )
    {
        mockApiClient.Setup(x => x.Get<CourseProviderDetailsModel>(
            It.Is<GetCourseProviderDetailsApiRequest>(request =>
                request.GetUrl.Contains(ukprn.ToString()) &&
                request.GetUrl.Contains(larsCode.ToString()) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(location) &&
                request.GetUrl.Contains(shortlistUserId.ToString())
            )
        )).ThrowsAsync(new HttpRequestException("Not Found", null, HttpStatusCode.NotFound)); ;

        var result = await sut.GetCourseProvider(
            ukprn,
            larsCode,
            location,
            distance,
            shortlistUserId
        );

        Assert.That(result, Is.Null);
    }
}
