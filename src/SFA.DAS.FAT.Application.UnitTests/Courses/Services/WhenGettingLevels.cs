using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Services;

public sealed class WhenGettingLevels
{
    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_The_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetLevelsListResponse sessionResponse,
        LevelsService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetLevelsListResponse>())
            .Returns(true);

        sessionServiceMock
            .Setup(x => x.Get<GetLevelsListResponse>())
            .Returns(sessionResponse);

        var result = await sut.GetLevelsAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionResponse.Levels);

        sessionServiceMock.Verify(x => x.Contains<GetLevelsListResponse>(), Times.Once);
        sessionServiceMock.Verify(x => x.Get<GetLevelsListResponse>(), Times.Once);
        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync<GetLevelsListResponse>(It.IsAny<string>(), It.IsAny<Func<Task<GetLevelsListResponse>>>(), It.IsAny<TimeSpan>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_The_Cache_If_Not_In_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetLevelsListResponse cacheResponse,
        FindApprenticeshipTrainingApi config,
        LevelsService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetLevelsListResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Levels.Key,
                It.IsAny<Func<Task<GetLevelsListResponse>>>(),
                CacheSetting.Levels.CacheDuration))
            .ReturnsAsync(cacheResponse);

        var result = await sut.GetLevelsAsync(cancellationToken);

        result.Should().BeEquivalentTo(cacheResponse.Levels);

        sessionServiceMock.Verify(x => x.Contains<GetLevelsListResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetLevelsListResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(cacheResponse), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.Levels.Key,
            It.IsAny<Func<Task<GetLevelsListResponse>>>(),
            CacheSetting.Levels.CacheDuration), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_The_Outer_Api_If_Not_In_Cache_Or_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetLevelsListResponse response,
        FindApprenticeshipTrainingApi config,
        LevelsService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetLevelsListResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        sessionServiceMock
            .Setup(x => x.Contains<GetLevelsListResponse>())
            .Returns(false);

        sessionServiceMock
            .Setup(x => x.Get<GetLevelsListResponse>())
            .Returns((GetLevelsListResponse)null);

        distributedCacheServiceMock
        .Setup(x => x.GetOrSetAsync(
            CacheSetting.Levels.Key,
            It.IsAny<Func<Task<GetLevelsListResponse>>>(),
            CacheSetting.Levels.CacheDuration))
        .Returns<Func<Task<GetLevelsListResponse>>, TimeSpan>((func, duration) => func());

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Levels.Key,
                It.IsAny<Func<Task<GetLevelsListResponse>>>(),
                CacheSetting.Levels.CacheDuration))
            .Returns(async (string key, Func<Task<GetLevelsListResponse>> func, TimeSpan duration) => await func());

        apiClientMock
            .Setup(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()))
            .ReturnsAsync(response);

        var result = await sut.GetLevelsAsync(cancellationToken);

        result.Should().BeEquivalentTo(response.Levels);

        sessionServiceMock.Verify(x => x.Contains<GetLevelsListResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetLevelsListResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(response), Times.Once);

        apiClientMock.Verify(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.Levels.Key,
            It.IsAny<Func<Task<GetLevelsListResponse>>>(),
            CacheSetting.Levels.CacheDuration
        ), Times.Once);
    }
}
