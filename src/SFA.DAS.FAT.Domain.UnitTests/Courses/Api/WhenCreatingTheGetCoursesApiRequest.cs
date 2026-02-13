using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public class WhenCreatingTheGetCoursesApiRequest
{
    [Test]
    public void Constructor_GivenValidValues_SetsProperties()
    {
        var _sut = new GetCoursesApiRequest
        {
            BaseUrl = "https://api.test/",
            Keyword = "test",
            Location = "London",
            Distance = 10,
            RouteIds = [1, 2],
            ApprenticeshipType = new List<string> { ApprenticeshipType.FoundationApprenticeship.ToString() },
            Levels = [3, 4],
            Page = 1,
            OrderBy = OrderBy.Score
        };

        Assert.Multiple(() =>
        {
            Assert.That(_sut.BaseUrl, Is.EqualTo("https://api.test/"));
            Assert.That(_sut.Keyword, Is.EqualTo("test"));
            Assert.That(_sut.Location, Is.EqualTo("London"));
            Assert.That(_sut.Distance, Is.EqualTo(10));
            Assert.That(_sut.RouteIds, Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That(_sut.ApprenticeshipType, Is.EquivalentTo([ApprenticeshipType.FoundationApprenticeship.ToString()]));
            Assert.That(_sut.Levels, Is.EquivalentTo(new List<int> { 3, 4 }));
            Assert.That(_sut.Page, Is.EqualTo(1));
            Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Score));
        });
    }

    [Test]
    public void GetUrl_GivenAllParameters_ConstructsCorrectUrl()
    {
        var _sut = new GetCoursesApiRequest
        {
            BaseUrl = "https://api.test/",
            Keyword = "test",
            Location = "London",
            Distance = 10,
            RouteIds = [1, 2],
            ApprenticeshipType = [ApprenticeshipType.FoundationApprenticeship.ToString()],
            Levels = [3, 4],
            Page = 1,
            OrderBy = OrderBy.Title
        };
        var expectedUrl = "https://api.test/courses?orderby=Title&keyword=test&location=London&distance=10&apprenticeshipType=FoundationApprenticeship&routeIds=1&routeIds=2&levels=3&levels=4&Page=1";
        Assert.That(_sut.GetUrl, Is.EqualTo(expectedUrl));
    }

    [Test]
    public void GetUrl_GivenEmptyParameters_ExcludesEmptyParameters()
    {
        var _sut = new GetCoursesApiRequest
        {
            BaseUrl = "https://api.test/",
            Keyword = null,
            Location = null,
            Distance = null,
            RouteIds = [],
            ApprenticeshipType = new List<string>(),
            Levels = [],
            Page = 1,
            OrderBy = OrderBy.Title
        };
        var expectedUrl = "https://api.test/courses?orderby=Title&Page=1";
        Assert.That(_sut.GetUrl, Is.EqualTo(expectedUrl));
    }
}
