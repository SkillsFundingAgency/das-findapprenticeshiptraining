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
    private const string BASE_URL = "https://test.local/";

    [SetUp]
    public void Setup()
    {
        _apiClientMock = new Mock<IApiClient>();
        _levelsServiceMock = new Mock<ILevelsService>();
        _routesServiceMock = new Mock<IRoutesService>();
        _config = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
        _config.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi()
        {
            BaseUrl = BASE_URL
        });
    }

    [Test, MoqAutoData]
    public async Task Handle_Should_Return_Correct_Response()
    {
        var query = new GetCoursesQuery()
        {
            Keyword = "test",
            Location = "London",
            Distance = 10,
            Routes = new List<string> { "Route1" },
            ApprenticeshipTypes = new List<string> { ApprenticeshipType.FoundationApprenticeship.ToString(), ApprenticeshipType.Apprenticeship.ToString() },
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
                    r.BaseUrl == BASE_URL &&
                    r.Keyword == query.Keyword &&
                    r.Location == query.Location &&
                    r.Distance == query.Distance &&
                    r.RouteIds.SequenceEqual(new List<int>() { 1 }) &&
                    r.Levels.SequenceEqual(query.Levels) &&
                    r.OrderBy == query.OrderBy &&
                    r.ApprenticeshipType == String.Empty
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
                r.BaseUrl == BASE_URL &&
                r.ApprenticeshipType == string.Empty
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
    [MoqInlineAutoData("", "", "")]
    [MoqInlineAutoData("Foundation apprenticeship", "", "FoundationApprenticeship")]
    [MoqInlineAutoData("Apprenticeship", "", "Apprenticeship")]
    [MoqInlineAutoData("Foundation", "Standard", "")]
    public async Task Handle_Should_Call_With_Expected_ApprenticeType_When_Only_One_Selected(
        string apprenticeshipType1,
        string apprenticeshipType2,
        string requestApprenticeshipType,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> mockConfig,
        [Frozen] Mock<IApiClient> mockApiClient,
        [Frozen] Mock<ILevelsService> mockLevelsService,
        [Frozen] Mock<IRoutesService> mockRoutesService,
        [Greedy] GetCoursesQueryHandler sut
        )
    {
        mockConfig.Setup(x => x.Value).Returns(new FindApprenticeshipTrainingApi()
        {
            BaseUrl = BASE_URL
        });

        var apprenticeshipTypes = new List<string>();
        if (!string.IsNullOrWhiteSpace(apprenticeshipType1)) { apprenticeshipTypes.Add(apprenticeshipType1); }
        if (!string.IsNullOrWhiteSpace(apprenticeshipType2)) { apprenticeshipTypes.Add(apprenticeshipType2); }

        var query = new GetCoursesQuery()
        {
            Routes = new List<string>(),
            ApprenticeshipTypes = apprenticeshipTypes,
            OrderBy = OrderBy.Title
        };

        var cancellationToken = new CancellationToken();

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = Constants.DefaultPageSize,
            TotalPages = 1,
            TotalCount = Constants.DefaultPageSize
        };

        mockApiClient.Setup(x => x.Get<GetCoursesResponse>(
                It.IsAny<GetCoursesApiRequest>()
            )
        )
        .ReturnsAsync(coursesResponse);

        var response = await sut.Handle(query, cancellationToken);

        mockApiClient.Verify(x => x.Get<GetCoursesResponse>(
            It.Is<GetCoursesApiRequest>(r =>

                r.ApprenticeshipType == requestApprenticeshipType
            )
        ), Times.Once);
    }
}
