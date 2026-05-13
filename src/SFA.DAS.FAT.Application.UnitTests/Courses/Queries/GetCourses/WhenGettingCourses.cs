using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourses;

public class WhenGettingCourses
{
    private Mock<IApiClient> _apiClientMock;
    private Mock<ILevelsService> _levelsServiceMock;
    private Mock<IRoutesService> _routesServiceMock;
    private Mock<IOptions<FindApprenticeshipTrainingApi>> _config;
    private const string BaseUrl = "https://test.local/";

    [SetUp]
    public void Setup()
    {
        _apiClientMock = new Mock<IApiClient>();
        _levelsServiceMock = new Mock<ILevelsService>();
        _routesServiceMock = new Mock<IRoutesService>();
        _config = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
        _config.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi()
        {
            BaseUrl = BaseUrl
        });
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenQueryHasFilters_ReturnsCorrectResponse()
    {
        var query = new GetCoursesQuery()
        {
            Keyword = "test",
            Location = "London",
            Distance = 10,
            Routes = new List<string> { "Route1" },
            LearningTypes = new List<LearningType> { LearningType.FoundationApprenticeship, LearningType.Apprenticeship },
            Levels = new List<int> { 3 },
            OrderBy = OrderBy.Title
        };

        var cancellationToken = new CancellationToken();

        var levels = new List<Level>
        {
            new()
            {
                Code = 3,
                Name = "GCSE"
            }
        };

        var routes = new List<Route>
        {
            new()
            {
                Id = 1,
                Name = "Route1"
            },
            new()
            {
                Id = 2,
                Name = "Route2"
            }
        };

        var expectedLearningTypes = new List<LearningType>
        {
            LearningType.FoundationApprenticeship,
            LearningType.Apprenticeship
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 2,
            TotalCount = 20
        };

        _levelsServiceMock.Setup(x => x.GetLevelsAsync(cancellationToken)).ReturnsAsync(levels);
        _routesServiceMock.Setup(x => x.GetRoutesAsync(cancellationToken)).ReturnsAsync(routes);

        _apiClientMock.Setup(x => x.Get<GetCoursesResponse>(
                It.Is<GetCoursesApiRequest>(r =>
                    r.BaseUrl == BaseUrl &&
                    r.Keyword == query.Keyword &&
                    r.Location == query.Location &&
                    r.Distance == query.Distance &&
                    r.RouteIds.SequenceEqual(new List<int>() { 1 }) &&
                    r.Levels.SequenceEqual(query.Levels) &&
                    r.OrderBy == query.OrderBy &&
                    r.LearningTypes.SequenceEqual(expectedLearningTypes)
                )
            )
        )
        .ReturnsAsync(coursesResponse);

        var _sut = new GetCoursesQueryHandler(
            _levelsServiceMock.Object,
            _routesServiceMock.Object,
            _config.Object,
            _apiClientMock.Object
        );

        var response = await _sut.Handle(query, cancellationToken);

        _apiClientMock.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r =>
                r.Keyword == query.Keyword &&
                r.Location == query.Location &&
                r.Distance == query.Distance &&
                r.RouteIds.SequenceEqual(new List<int>() { 1 }) &&
                r.Levels.SequenceEqual(query.Levels) &&
                r.OrderBy == query.OrderBy &&
                r.BaseUrl == BaseUrl &&
                r.LearningTypes.SequenceEqual(expectedLearningTypes)
            )
        ), Times.Once);

        Assert.Multiple(() =>
        {
            Assert.That(response.Standards, Is.EquivalentTo(coursesResponse.Standards));
            Assert.That(response.Page, Is.EqualTo(coursesResponse.Page));
            Assert.That(response.PageSize, Is.EqualTo(coursesResponse.PageSize));
            Assert.That(response.TotalCount, Is.EqualTo(coursesResponse.TotalCount));
            Assert.That(response.TotalPages, Is.EqualTo(coursesResponse.TotalPages));
            Assert.That(response.Levels, Is.EquivalentTo(levels));
            Assert.That(response.Routes, Is.EquivalentTo(routes));
        });
    }

    [Test]
    [MoqInlineAutoData(LearningType.FoundationApprenticeship, LearningType.FoundationApprenticeship)]
    [MoqInlineAutoData(LearningType.Apprenticeship, LearningType.Apprenticeship)]
    [MoqInlineAutoData(LearningType.ApprenticeshipUnit, LearningType.ApprenticeshipUnit)]
    public async Task Handle_WhenOnlyOneLearningTypeSelected_CallsWithExpectedLearningType(
     LearningType selectedLearningType,
     LearningType requestLearningType,
     [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
     [Frozen] Mock<IApiClient> mockApiClient,
     [Frozen] Mock<ILevelsService> mockLevelsService,
     [Frozen] Mock<IRoutesService> mockRoutesService,
     [Greedy] GetCoursesQueryHandler sut
     )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi
        {
            BaseUrl = BaseUrl
        });

        mockLevelsService
            .Setup(x => x.GetLevelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Level>());

        mockRoutesService
            .Setup(x => x.GetRoutesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Route>());

        var query = new GetCoursesQuery
        {
            Routes = new List<string>(),
            LearningTypes = new List<LearningType> { selectedLearningType },
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = Constants.DefaultPageSize
        };

        mockApiClient
            .Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>()))
            .ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r =>
                r.LearningTypes.Count == 1 &&
                r.LearningTypes[0] == requestLearningType
            )
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenMultipleRoutesSelected_FiltersRouteIdsCorrectly(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var routes = new List<Route>
        {
            new() { Id = 1, Name = "Route1" },
            new() { Id = 2, Name = "Route2" },
            new() { Id = 3, Name = "Route3" },
            new() { Id = 4, Name = "Route4" }
        };

        var query = new GetCoursesQuery
        {
            Routes = new List<string> { "Route1", "Route3" },
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = 0
        };

        mockRoutesService.Setup(x => x.GetRoutesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(routes);
        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r => r.RouteIds.SequenceEqual(new List<int> { 1, 3 }))
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenNoRoutesSpecified_PassesEmptyRouteIds(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var routes = new List<Route>
        {
            new() { Id = 1, Name = "Route1" },
            new() { Id = 2, Name = "Route2" }
        };

        var query = new GetCoursesQuery
        {
            Routes = new List<string>(),
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = 0
        };

        mockRoutesService.Setup(x => x.GetRoutesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(routes);
        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r => r.RouteIds.Count == 0)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenPageSpecified_PassesPageParameterCorrectly(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var query = new GetCoursesQuery
        {
            Page = 5,
            Routes = new List<string>(),
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 5,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 10,
            TotalCount = 100
        };

        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r => r.Page == 5)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenKeywordAndLocationNotSpecified_PassesNullKeywordAndLocation(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var query = new GetCoursesQuery
        {
            Keyword = null,
            Location = null,
            Distance = null,
            Routes = new List<string>(),
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = 0
        };

        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r =>
                r.Keyword == null &&
                r.Location == null &&
                r.Distance == null)
        ), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenHandlingQuery_CallsLevelsServiceAndRoutesService(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var query = new GetCoursesQuery
        {
            Routes = new List<string>(),
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = 0
        };

        var cancellationToken = new CancellationToken();
        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, cancellationToken);

        mockLevelsService.Verify(x => x.GetLevelsAsync(cancellationToken), Times.Once);
        mockRoutesService.Verify(x => x.GetRoutesAsync(cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_WhenLevelsSpecified_PassesAllLevelsCorrectly(
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
    )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi { BaseUrl = BaseUrl });

        var expectedLevels = new List<int> { 2, 3, 4, 6 };
        var query = new GetCoursesQuery
        {
            Levels = expectedLevels,
            Routes = new List<string>(),
            LearningTypes = new List<LearningType>(),
            OrderBy = OrderBy.Title
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = 0
        };

        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(It.IsAny<GetCoursesApiRequest>())).ReturnsAsync(coursesResponse);

        await sut.Handle(query, CancellationToken.None);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r => r.Levels.SequenceEqual(expectedLevels))
        ), Times.Once);
    }
}
