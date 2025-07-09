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
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns(sessionResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(sessionResponse);

        distributedCacheServiceMock.Verify(x => x.GetAsync<GetAcademicYearsLatestResponse>(It.IsAny<string>()), Times.Never);
        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_From_Cache_When_Session_Empty(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse cacheResponse,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {
        sessionServiceMock
            .Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns((GetAcademicYearsLatestResponse)null);

        cacheResponse.QarPeriod = "Q2";
        cacheResponse.ReviewPeriod = "R2";
        distributedCacheServiceMock
            .Setup(x => x.GetAsync<GetAcademicYearsLatestResponse>(CacheSetting.AcademicYearsLatest.Key))
            .ReturnsAsync(cacheResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(cacheResponse);
        sessionServiceMock.Verify(x => x.Set(cacheResponse), Times.Once);
        apiClientMock.Verify(x => x.Get<GetAcademicYearsLatestResponse>(It.IsAny<GetAcademicYearsLatestRequest>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_From_Api_When_Not_In_Session_Or_Cache(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        GetAcademicYearsLatestResponse apiResponse,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut,
        CancellationToken cancellationToken)
    {

        sessionServiceMock.Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns((GetAcademicYearsLatestResponse)null);


        distributedCacheServiceMock.Setup(x => x.GetAsync<GetAcademicYearsLatestResponse>(
            CacheSetting.AcademicYearsLatest.Key))
            .ReturnsAsync((GetAcademicYearsLatestResponse)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiResponse.QarPeriod = "Q3";
        apiResponse.ReviewPeriod = "R3";

        apiClientMock.Setup(x => x.Get<GetAcademicYearsLatestResponse>(
            It.IsAny<GetAcademicYearsLatestRequest>())).ReturnsAsync(apiResponse);

        var result = await sut.GetAcademicYearsLatestAsync(cancellationToken);

        result.Should().BeEquivalentTo(apiResponse);
        sessionServiceMock.Verify(x => x.Set(apiResponse), Times.Once);
        distributedCacheServiceMock.Verify(x => x.SetAsync(
            CacheSetting.AcademicYearsLatest.Key,
            apiResponse,
            CacheSetting.AcademicYearsLatest.CacheDuration), Times.Once);
    }

    [Test, MoqAutoData]
    public void Then_Throws_If_All_Sources_Fail(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IApiClient> apiClientMock,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> configMock,
        FindApprenticeshipTrainingApi config,
        AcademicYearService sut)
    {
        sessionServiceMock.Setup(x => x.Get<GetAcademicYearsLatestResponse>())
            .Returns((GetAcademicYearsLatestResponse)null);

        distributedCacheServiceMock.Setup(x => x.GetAsync<GetAcademicYearsLatestResponse>(
            CacheSetting.AcademicYearsLatest.Key))
            .ReturnsAsync((GetAcademicYearsLatestResponse)null);

        configMock.Setup(x => x.Value).Returns(config);

        apiClientMock.Setup(x => x.Get<GetAcademicYearsLatestResponse>(
            It.IsAny<GetAcademicYearsLatestRequest>()))
            .ReturnsAsync(new GetAcademicYearsLatestResponse());

        Func<Task> act = async () => await sut.GetAcademicYearsLatestAsync(CancellationToken.None);

        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Could not retrieve academic years from any source.");
    }
}
