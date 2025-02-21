using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Services;

public class WhenGettingCourses
{
    private Mock<IApiClient> _apiClientMock;
    private Mock<IOptions<FindApprenticeshipTrainingApi>> _config;
    private CourseService _service;

    [SetUp]
    public void Setup()
    {
        _apiClientMock = new Mock<IApiClient>();
        _config = new Mock<IOptions<FindApprenticeshipTrainingApi>>();
        _config.Setup(a => a.Value).Returns(new FindApprenticeshipTrainingApi()
        {
            BaseUrl = "https://api.test/"
        });

        _service = new CourseService(_apiClientMock.Object, _config.Object);
    }

    [Test, MoqAutoData]
    public async Task GetCourses_Should_Return_Response_From_ApiClient(
        GetCoursesResponse response
    )
    {
        var keyword = "test";
        var location = "London";
        var distance = 10;
        var routeIds = new List<int> { 1, 2 };
        var levels = new List<int> { 3, 4 };
        var pageNumber = 1;
        var orderBy = OrderBy.Title;
        var cancellationToken = new CancellationToken();

        _apiClientMock.Setup(x => x.Get<GetCoursesResponse>(
                It.IsAny<GetCoursesApiRequest>()
            )
        )
        .ReturnsAsync(response);

        var _sut = await _service.GetCourses(keyword, location, distance, routeIds, levels, pageNumber, orderBy, cancellationToken);

        Assert.That(_sut, Is.EqualTo(response));
        _apiClientMock.Verify(x => x.Get<GetCoursesResponse>(It.Is<GetCoursesApiRequest>(r =>
            r.Keyword == keyword &&
            r.Location == location &&
            r.Distance == distance &&
            r.RouteIds == routeIds &&
            r.Levels == levels &&
            r.Page == pageNumber &&
            r.OrderBy == orderBy
        )), Times.Once);
    }
}
