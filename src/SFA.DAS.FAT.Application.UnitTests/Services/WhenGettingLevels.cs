using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public sealed class WhenGettingLevels
{
    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_Session_If_Present(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        List<Level> sessionLevels,
        LevelsService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(x => x.Get<List<Level>>())
            .Returns(sessionLevels);

        var result = await sut.GetLevelsAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionLevels);
        distributedCacheServiceMock.Verify(x => x.GetAsync<List<Level>>(It.IsAny<string>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_Cache_If_Session_Empty(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        List<Level> cachedLevels,
        LevelsService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(x => x.Get<List<Level>>())
            .Returns((List<Level>)null);

        distributedCacheServiceMock.Setup(x => x.GetAsync<List<Level>>(CacheSetting.Levels.Key))
            .ReturnsAsync(cachedLevels);

        var result = await sut.GetLevelsAsync(cancellationToken);

        result.Should().BeEquivalentTo(cachedLevels);
        sessionServiceMock.Verify(x => x.Set(It.Is<IEnumerable<Level>>(levels => levels.SequenceEqual(cachedLevels))), Times.Once);
        apiClientMock.Verify(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Levels_Are_Returned_From_Api_And_Cached_If_Session_And_Cache_Empty(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
    [Frozen] Mock<IApiClient> apiClientMock,
    [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
    FindApprenticeshipTrainingApi config,
    GetLevelsListResponse apiResponse,
    LevelsService sut,
    CancellationToken cancellationToken)
    {
        
        var expectedLevels = new List<Level> { new Level() };
        apiResponse.Levels = expectedLevels;

        sessionServiceMock
            .Setup(x => x.Get<List<Level>>())
            .Returns((List<Level>)null);

        distributedCacheServiceMock
            .Setup(x => x.GetAsync<List<Level>>(CacheSetting.Levels.Key))
            .ReturnsAsync((List<Level>)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiClientMock
            .Setup(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()))
            .ReturnsAsync(apiResponse);

        
        var result = await sut.GetLevelsAsync(cancellationToken);

        
        result.Should().BeEquivalentTo(expectedLevels);

        sessionServiceMock.Verify(x => x.Set(It.Is<IEnumerable<Level>>(l => l.SequenceEqual(expectedLevels))), Times.Once);

        distributedCacheServiceMock.Verify(x =>
            x.SetAsync(CacheSetting.Levels.Key,
                       It.Is<IEnumerable<Level>>(l => l.SequenceEqual(expectedLevels)),
                       CacheSetting.Levels.CacheDuration),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Exception_Is_Thrown_If_All_Sources_Fail(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        FindApprenticeshipTrainingApi config,
        LevelsService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(x => x.Get<List<Level>>())
            .Returns((List<Level>)null);

        distributedCacheServiceMock.Setup(x => x.GetAsync<List<Level>>(CacheSetting.Levels.Key))
            .ReturnsAsync((List<Level>)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiClientMock.Setup(x => x.Get<GetLevelsListResponse>(It.IsAny<GetCourseLevelsApiRequest>()))
            .ReturnsAsync((GetLevelsListResponse)null);

        Func<Task> action = async () => await sut.GetLevelsAsync(cancellationToken);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve course levels from any source.");
    }
}
