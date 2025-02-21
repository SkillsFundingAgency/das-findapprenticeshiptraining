using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public class WhenCreatingTheGetCoursesApiRequest
{
    [Test]
    public void Constructor_Should_Set_Properties()
    {
        var _sut = new GetCoursesApiRequest("https://api.test/", "test", "London", 10, new List<int> { 1, 2 }, new List<int> { 3, 4 }, 1, OrderBy.Score);

        Assert.Multiple(() =>
        {
            Assert.That(_sut.BaseUrl, Is.EqualTo("https://api.test/"));
            Assert.That(_sut.Keyword, Is.EqualTo("test"));
            Assert.That(_sut.Location, Is.EqualTo("London"));
            Assert.That(_sut.Distance, Is.EqualTo(10));
            Assert.That(_sut.RouteIds, Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That(_sut.Levels, Is.EquivalentTo(new List<int> { 3, 4 }));
            Assert.That(_sut.Page, Is.EqualTo(1));
            Assert.That(_sut.OrderBy, Is.EqualTo(OrderBy.Score));
        });
    }

    [Test]
    public void GetUrl_Should_Construct_Correct_Url()
    {
        var _sut = new GetCoursesApiRequest("https://api.test/", "test", "London", 10, new List<int> { 1, 2 }, new List<int> { 3, 4 }, 1, OrderBy.Title);
        var expectedUrl = "https://api.test/courses?OrderBy=Title&Keyword=test&Location=London&Distance=10&Page=1&RouteIds=1&RouteIds=2&Levels=3&Levels=4";
        Assert.That(_sut.GetUrl, Is.EqualTo(expectedUrl));
    }

    [Test]
    public void GetUrl_Should_Exclude_Empty_Parameters()
    {
        var _sut = new GetCoursesApiRequest("https://api.test/", null, null, null, new List<int>(), new List<int>(), 1, OrderBy.Title);
        var expectedUrl = "https://api.test/courses?OrderBy=Title&Page=1";
        Assert.That(_sut.GetUrl, Is.EqualTo(expectedUrl));
    }
}
