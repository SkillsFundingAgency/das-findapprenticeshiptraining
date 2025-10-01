using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourse;

public sealed class WhenGettingCourse
{
    private Mock<ICourseService> _courseServiceMock;
    private Mock<ILevelsService> _levelsServiceMock;
    private GetCourseQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _levelsServiceMock = new Mock<ILevelsService>();

        _handler = new GetCourseQueryHandler(_courseServiceMock.Object, _levelsServiceMock.Object);
    }

    [Test]
    [MoqAutoData]
    public async Task Then_Handler_Maps_To_Response_Correctly(
        GetCourseResponse courseResponse,
        IEnumerable<Level> levels
    )
    {
        var query = new GetCourseQuery
        {
            LarsCode = 123,
            Location = "London",
            Distance = 20
        };

        _courseServiceMock
            .Setup(cs => cs.GetCourse(query.LarsCode, query.Location, query.Distance))
            .ReturnsAsync(courseResponse);

        _levelsServiceMock
            .Setup(ls => ls.GetLevelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(levels);

        var sut = await _handler.Handle(query, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(sut.StandardUId, Is.EqualTo(courseResponse.StandardUId));
            Assert.That(sut.IFateReferenceNumber, Is.EqualTo(courseResponse.IFateReferenceNumber));
            Assert.That(sut.LarsCode, Is.EqualTo(courseResponse.LarsCode));
            Assert.That(sut.ProvidersCountWithinDistance, Is.EqualTo(courseResponse.ProvidersCountWithinDistance));
            Assert.That(sut.TotalProvidersCount, Is.EqualTo(courseResponse.TotalProvidersCount));
            Assert.That(sut.Title, Is.EqualTo(courseResponse.Title));
            Assert.That(sut.Level, Is.EqualTo(courseResponse.Level));
            Assert.That(sut.Version, Is.EqualTo(courseResponse.Version));
            Assert.That(sut.OverviewOfRole, Is.EqualTo(courseResponse.OverviewOfRole));
            Assert.That(sut.Route, Is.EqualTo(courseResponse.Route));
            Assert.That(sut.RouteCode, Is.EqualTo(courseResponse.RouteCode));
            Assert.That(sut.MaxFunding, Is.EqualTo(courseResponse.MaxFunding));
            Assert.That(sut.TypicalDuration, Is.EqualTo(courseResponse.TypicalDuration));
            Assert.That(sut.TypicalJobTitles, Is.EqualTo(courseResponse.TypicalJobTitles));
            Assert.That(sut.StandardPageUrl, Is.EqualTo(courseResponse.StandardPageUrl));
            Assert.That(sut.Levels, Is.EqualTo(levels.ToList()));
        });

        _courseServiceMock.Verify(cs =>
            cs.GetCourse(
                query.LarsCode,
                query.Location,
                query.Distance
            ),
            Times.Once
        );

        _levelsServiceMock.Verify(ls =>
            ls.GetLevelsAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Test]
    [MoqAutoData]
    public async Task And_CourseService_Returns_Null_Then_Handler_Returns_Null()
    {
        _levelsServiceMock.Setup(x => x.GetLevelsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Level>());

        _courseServiceMock.Setup(x => x.GetCourse(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>()))
            .ReturnsAsync((GetCourseResponse)null);

        var sut = await _handler.Handle(new GetCourseQuery(), CancellationToken.None);

        sut.Should().BeNull();
    }
}
