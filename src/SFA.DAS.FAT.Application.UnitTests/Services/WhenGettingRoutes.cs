using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
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
        List<Route> sessionRoutes,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Get<List<Route>>(SessionKeys.StandardRoutes))
            .Returns(sessionRoutes);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionRoutes);

        distributedCacheServiceMock.Verify(x =>
            x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<Route>>>>(), It.IsAny<TimeSpan>()),
            Times.Never);

        apiClientMock.Verify(x =>
            x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()),
            Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Cache_If_Not_In_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        List<Route> cachedRoutes,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock.Setup(x => x.Get<List<Route>>(SessionKeys.StandardRoutes)).Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Routes.Key,
                It.IsAny<Func<Task<IEnumerable<Route>>>>(),
                CacheSetting.Routes.CacheDuration))
            .ReturnsAsync(cachedRoutes);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(cachedRoutes);

        sessionServiceMock.Verify(x => x.Set(SessionKeys.StandardRoutes, It.Is<IEnumerable<Route>>(r => r.SequenceEqual(cachedRoutes))), Times.Once);

        apiClientMock.Verify(x =>
            x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()),
            Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Api_If_Not_In_Session_Or_Cache(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetRoutesListResponse apiResponse,
        FindApprenticeshipTrainingApi config,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        var expectedRoutes = new List<Route> { new Route() };
        apiResponse.Routes = expectedRoutes;

        sessionServiceMock.Setup(x => x.Get<List<Route>>(SessionKeys.StandardRoutes)).Returns(() => null);
        configMock.Setup(x => x.Value).Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Routes.Key,
                It.IsAny<Func<Task<IEnumerable<Route>>>>(),
                CacheSetting.Routes.CacheDuration))
            .Returns<string, Func<Task<IEnumerable<Route>>>, TimeSpan>(async (key, factory, duration) =>
            {
                return await factory();
            });

        apiClientMock
            .Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(apiResponse);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(expectedRoutes);

        sessionServiceMock.Verify(x => x.Set(SessionKeys.StandardRoutes, It.Is<IEnumerable<Route>>(r => r.SequenceEqual(expectedRoutes))), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_Exception_Is_Thrown_If_All_Sources_Empty(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        FindApprenticeshipTrainingApi config,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock.Setup(x => x.Get<List<Route>>(SessionKeys.StandardRoutes)).Returns(() => null);
        configMock.Setup(x => x.Value).Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Routes.Key,
                It.IsAny<Func<Task<IEnumerable<Route>>>>(),
                CacheSetting.Routes.CacheDuration))
            .Returns<string, Func<Task<IEnumerable<Route>>>, TimeSpan>(async (key, factory, duration) =>
            {
                return await factory();
            });

        apiClientMock
            .Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(new GetRoutesListResponse { Routes = [] });

        Func<Task> act = async () => await sut.GetRoutesAsync(cancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve course routes from any source.");
    }
}
