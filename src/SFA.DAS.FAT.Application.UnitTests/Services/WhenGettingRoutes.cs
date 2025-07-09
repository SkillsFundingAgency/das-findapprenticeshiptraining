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

public sealed class WhenGettingRoutes
{
    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        List<Route> sessionRoutes,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Get<List<Route>>())
            .Returns(sessionRoutes);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionRoutes);

        sessionServiceMock.Verify(x => x.Get<List<Route>>(), Times.Once);
        distributedCacheServiceMock.Verify(x => x.GetAsync<List<Route>>(It.IsAny<string>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Routes_Are_Returned_From_The_Cache_If_Not_In_Session(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        List<Route> cachedRoutes,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Get<List<Route>>())
            .Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetAsync<List<Route>>(CacheSetting.Routes.Key))
            .ReturnsAsync(cachedRoutes);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(cachedRoutes);

        sessionServiceMock.Verify(x => x.Get<List<Route>>(), Times.Once);
        sessionServiceMock.Verify(x => x.Set(cachedRoutes), Times.Once);
        distributedCacheServiceMock.Verify(x => x.GetAsync<List<Route>>(CacheSetting.Routes.Key), Times.Once);
        apiClientMock.Verify(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()), Times.Never);
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
        sessionServiceMock
            .Setup(x => x.Get<List<Route>>())
            .Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetAsync<List<Route>>(CacheSetting.Routes.Key))
            .ReturnsAsync(() => null);

        configMock.Setup(x => x.Value).Returns(config);

        apiClientMock
            .Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(apiResponse);

        var result = await sut.GetRoutesAsync(cancellationToken);

        result.Should().BeEquivalentTo(apiResponse.Routes);

        sessionServiceMock.Verify(x => x.Set(apiResponse.Routes), Times.Once);
        distributedCacheServiceMock.Verify(x => x.SetAsync(
            CacheSetting.Routes.Key,
            apiResponse.Routes,
            CacheSetting.Routes.CacheDuration), Times.Once);
    }

    [Test, MoqAutoData]
    public void Then_Exception_Is_Thrown_If_All_Sources_Empty(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        FindApprenticeshipTrainingApi config,
        RoutesService sut,
        CancellationToken cancellationToken
    )
    {
        sessionServiceMock
            .Setup(x => x.Get<List<Route>>())
            .Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetAsync<List<Route>>(CacheSetting.Routes.Key))
            .ReturnsAsync(() => null);

        configMock.Setup(x => x.Value).Returns(config);

        apiClientMock
            .Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(new GetRoutesListResponse { Routes = [] });

        Func<Task> act = async () => await sut.GetRoutesAsync(cancellationToken);

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve course routes from any source.");
    }

    [Test, MoqAutoData]
    public async Task Then_Exception_Is_Thrown_If_RoutesList_Is_Empty(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
    [Frozen] Mock<IApiClient> apiClientMock,
    [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
    FindApprenticeshipTrainingApi config,
    GetRoutesListResponse apiResponse,
    RoutesService sut,
    CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(x => x.Get<List<Route>>())
            .Returns((List<Route>)null);

        distributedCacheServiceMock.Setup(x => x.GetAsync<List<Route>>(CacheSetting.Routes.Key))
            .ReturnsAsync((List<Route>)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiResponse.Routes = new List<Route>(); 
        apiClientMock.Setup(x => x.Get<GetRoutesListResponse>(It.IsAny<GetCourseRoutesApiRequest>()))
            .ReturnsAsync(apiResponse);

        Func<Task> action = async () => await sut.GetRoutesAsync(cancellationToken);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve course routes from any source.");
    }
}
