using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.CourseProviders.Query.GetCourseProviders;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Requests;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Responses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public sealed class WhenGettingAcademicYearsLatest
{
    [Test, MoqAutoData]
    public async Task Then_AcademicYearsLatest_Are_Returned_From_The_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse sessionResponse,
        AcademicYearService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(true);

        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns(sessionResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionResponse);

        sessionServiceMock.Verify(x => x.Contains<GetAcademicYearsLatestResponse>(), Times.Once);
        sessionServiceMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(), Times.Once);
        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<GetLevelsListResponse>>>(), It.IsAny<TimeSpan>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_AcademicYearsLatest_Are_Returned_From_The_Cache_If_Not_In_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse cacheResponse,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .ReturnsAsync(cacheResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(cacheResponse);

        sessionServiceMock.Verify(x => x.Contains<GetAcademicYearsLatestResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(cacheResponse), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
            CacheSetting.AcademicYearsLatest.CacheDuration), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_AcademicYearsLatest_Are_Returned_From_The_Outer_Api_If_Not_In_Cache_Or_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse response,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(false);

        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns((GetAcademicYearsLatestResponse)null);

        distributedCacheServiceMock
        .Setup(x => x.GetOrSetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
            CacheSetting.AcademicYearsLatest.CacheDuration))
        .Returns<Func<Task<GetAcademicYearsLatestResponse>>, TimeSpan>((func, duration) => func());

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .Returns(async (string key, Func<Task<GetAcademicYearsLatestResponse>> func, TimeSpan duration) => await func());

        apiClientMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()))
            .ReturnsAsync(response);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(response);

        sessionServiceMock.Verify(x => x.Contains<GetAcademicYearsLatestResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(response), Times.Once);

        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
            CacheSetting.AcademicYearsLatest.CacheDuration
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Api_Client_Is_Called_With_The_Request_Url(
        GetCourseProvidersQuery query,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        [Frozen] Mock<IApiClient> mockApiClient,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {

        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        sessionServiceMock
            .Setup(x => x.Contains<GetAcademicYearsLatestResponse>())
            .Returns(false);

        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns((GetAcademicYearsLatestResponse)null);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .Returns<Func<Task<GetAcademicYearsLatestResponse>>, TimeSpan>((func, duration) => func());

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .Returns(async (string key, Func<Task<GetAcademicYearsLatestResponse>> func, TimeSpan duration) => await func());

        var expectedUrl = new GetAcademicYearsLatestRequest(configMock.Object.Value.BaseUrl).GetUrl;

        await sut.GetAcademicYearsLatestAsync(cancellationToken);

        mockApiClient.Verify(client => client.Get<GetAcademicYearsLatestResponse>(
            It.IsAny<GetAcademicYearsLatestRequest>()), Times.Once);
        mockApiClient.Verify(client => client.Get<GetAcademicYearsLatestResponse>(
            It.Is<GetAcademicYearsLatestRequest>(request => request.GetUrl == expectedUrl)), Times.Once);
    }
}
