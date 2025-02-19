using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourses;

public class WhenGettingCourses
{
    private Mock<ICourseService> _courseServiceMock;
    private Mock<ILevelsService> _levelsServiceMock;
    private Mock<IRoutesService> _routesServiceMock;
    private GetCoursesQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _levelsServiceMock = new Mock<ILevelsService>();
        _routesServiceMock = new Mock<IRoutesService>();

        _handler = new GetCoursesQueryHandler(
            _courseServiceMock.Object,
            _levelsServiceMock.Object,
            _routesServiceMock.Object
        );
    }

    [Test]
    public async Task Handle_Should_Return_Correct_Response()
    {
        var query = new GetCoursesQuery() {
            Keyword = "test", 
            Location = "London", 
            Distance = 10, 
            Routes = new List<string> { "Route1" }, 
            Levels = new List<int> { 3 }, 
            OrderBy = OrderBy.Title
        };

        var cancellationToken = new CancellationToken();

        var levels = new List<Level> 
        { 
            new Level() 
            { 
                Code = 3, 
                Name = "GCSE" 
            } 
        };

        var routes = new List<Route> 
        { 
            new Route 
            { 
                Id = 1, 
                Name = "Route1" 
            }, 
            new Route 
            { 
                Id = 2, 
                Name = "Route2" 
            } 
        };

        var coursesResponse = new GetCoursesResponse
        {
            Standards = new List<StandardModel>(),
            Page = 1,
            PageSize = 10,
            TotalPages = 2,
            TotalCount = 20
        };

        _levelsServiceMock.Setup(x => x.GetLevelsAsync(cancellationToken)).ReturnsAsync(levels);
        _routesServiceMock.Setup(x => x.GetRoutesAsync(cancellationToken)).ReturnsAsync(routes);
        _courseServiceMock.Setup(x => x.GetCourses("test", "London", 10, new List<int> { 1 }, new List<int> { 3 }, OrderBy.Title, cancellationToken)).ReturnsAsync(coursesResponse);

        var _sut = await _handler.Handle(query, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.Standards, Is.EquivalentTo(coursesResponse.Standards));
            Assert.That(_sut.Page, Is.EqualTo(coursesResponse.Page));
            Assert.That(_sut.PageSize, Is.EqualTo(coursesResponse.PageSize));
            Assert.That(_sut.TotalCount, Is.EqualTo(coursesResponse.TotalCount));
            Assert.That(_sut.TotalPages, Is.EqualTo(coursesResponse.TotalPages));
            Assert.That(_sut.Levels, Is.EquivalentTo(levels));
            Assert.That(_sut.Routes, Is.EquivalentTo(routes));
        });
    }
}
