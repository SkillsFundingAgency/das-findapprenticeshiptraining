using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public sealed class WhenGettingRoutes
{
    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetRoutesListResponse sessionResponse,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetRoutesListResponse>())
            .Returns(true);

        sessionServiceMock
            .Setup(x => x.Get<GetRoutesListResponse>())
            .Returns(sessionResponse);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionResponse.Routes);

        sessionServiceMock.Verify(x => x.Contains<GetRoutesListResponse>(), Times.Once);
        sessionServiceMock.Verify(x => x.Get<GetRoutesListResponse>(), Times.Once);
        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync<GetRoutesListResponse>(It.IsAny<string>(), It.IsAny<Func<Task<GetRoutesListResponse>>>(), It.IsAny<TimeSpan>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Cache_If_Not_In_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetRoutesListResponse cacheResponse,
        FindApprenticeshipTrainingApi config,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetRoutesListResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Routes.Key,
                It.IsAny<Func<Task<GetRoutesListResponse>>>(),
                CacheSetting.Routes.CacheDuration))
            .ReturnsAsync(cacheResponse);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(cacheResponse.Routes);

        sessionServiceMock.Verify(x => x.Contains<GetRoutesListResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetRoutesListResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(cacheResponse), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.Routes.Key,
            It.IsAny<Func<Task<GetRoutesListResponse>>>(),
            CacheSetting.Routes.CacheDuration), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Outer_Api_If_Not_In_Cache_Or_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetRoutesListResponse response,
        FindApprenticeshipTrainingApi config,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Contains<GetRoutesListResponse>())
            .Returns(false);

        configMock
            .Setup(x => x.Value)
            .Returns(config);

        sessionServiceMock
            .Setup(x => x.Contains<GetRoutesListResponse>())
            .Returns(false);

        sessionServiceMock
            .Setup(x => x.Get<GetRoutesListResponse>())
            .Returns((GetRoutesListResponse)null);

        distributedCacheServiceMock
        .Setup(x => x.GetOrSetAsync(
            CacheSetting.Routes.Key,
            It.IsAny<Func<Task<GetRoutesListResponse>>>(),
            CacheSetting.Routes.CacheDuration))
        .Returns<Func<Task<GetRoutesListResponse>>, TimeSpan>((func, duration) => func());

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Routes.Key,
                It.IsAny<Func<Task<GetRoutesListResponse>>>(),
                CacheSetting.Routes.CacheDuration))
            .Returns(async (string key, Func<Task<GetRoutesListResponse>> func, TimeSpan duration) => await func());

        apiClientMock
            .Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(response);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(response.Routes);

        sessionServiceMock.Verify(x => x.Contains<GetRoutesListResponse>(), Times.Once);

        sessionServiceMock.Verify(x => x.Get<GetRoutesListResponse>(), Times.Never);

        sessionServiceMock.Verify(x => x.Set(response), Times.Once);

        apiClientMock.Verify(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.Routes.Key,
            It.IsAny<Func<Task<GetRoutesListResponse>>>(),
            CacheSetting.Routes.CacheDuration
        ), Times.Once);
    }
}
