using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Requests;
using SFA.DAS.FAT.Domain.AcademicYears.Api.Responses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Infrastructure;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Services;

public sealed class WhenGettingAcademicYearsLatest
{
    [Test, MoqAutoData]
    public async Task Then_Returns_From_Session_When_Valid(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse sessionResponse,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {
        sessionResponse.QarPeriod = "Q1";
        sessionResponse.ReviewPeriod = "R1";

        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears))
            .Returns(sessionResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionResponse);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            It.IsAny<string>(), It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(), It.IsAny<TimeSpan>()), Times.Never);

        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_From_Cache_If_Session_Empty(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse cachedResponse,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears))
            .Returns((GetAcademicYearsLatestResponse)null);

        cachedResponse.QarPeriod = "Q2";
        cachedResponse.ReviewPeriod = "R2";

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .ReturnsAsync(cachedResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(cachedResponse);

        sessionServiceMock.Verify(x => x.Set(SessionKeys.AcademicYears, cachedResponse), Times.Once);

        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_From_Api_If_Not_In_Session_Or_Cache(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse apiResponse,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears))
            .Returns((GetAcademicYearsLatestResponse)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiResponse.QarPeriod = "Q3";
        apiResponse.ReviewPeriod = "R3";

        apiClientMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()))
            .ReturnsAsync(apiResponse);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .Returns<string, Func<Task<GetAcademicYearsLatestResponse>>, TimeSpan>((key, factory, duration) =>
            {
                return factory();
            });

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(apiResponse);

        sessionServiceMock.Verify(x => x.Set(SessionKeys.AcademicYears, apiResponse), Times.Once);

        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
            CacheSetting.AcademicYearsLatest.CacheDuration), Times.Once);

        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Once);
    }


    [Test, MoqAutoData]
    public async Task Then_Throws_If_All_Sources_Fail(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears))
            .Returns((GetAcademicYearsLatestResponse)null);

        configMock.Setup(x => x.Value).Returns(config);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .ThrowsAsync(new InvalidOperationException("Could not retrieve academic years from any source."));

        Func<Task> act = async () => await sut.GetAcademicYearsLatestAsync(cancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve academic years from any source.");

        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Throws_If_Api_Returns_Invalid_Data(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
    [Frozen] Mock<IApiClient> apiClientMock,
    [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
    FindApprenticeshipTrainingApi config,
    AcademicYearService sut,
    CancellationToken cancellationToken)
    {
        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(SessionKeys.AcademicYears))
            .Returns((GetAcademicYearsLatestResponse)null);

        configMock.Setup(x => x.Value).Returns(config);

        var invalidApiResponse = new GetAcademicYearsLatestResponse
        {
            QarPeriod = null,
            ReviewPeriod = null
        };

        apiClientMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()))
            .ReturnsAsync(invalidApiResponse);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.AcademicYearsLatest.Key,
                It.IsAny<Func<Task<GetAcademicYearsLatestResponse>>>(),
                CacheSetting.AcademicYearsLatest.CacheDuration))
            .Returns<string, Func<Task<GetAcademicYearsLatestResponse>>, TimeSpan>((key, factory, duration) => factory());

        Func<Task> act = async () => await sut.GetAcademicYearsLatestAsync(cancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve academic years from any source.");
    }

}
