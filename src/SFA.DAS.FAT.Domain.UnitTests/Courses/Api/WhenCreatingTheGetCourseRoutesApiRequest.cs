using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheGetCourseRoutesApiRequest
{
    [Test]
    public void Then_The_Constructor_Should_Set_The_Base_Url()
    {
        var _sut = new GetCourseRoutesApiRequest("https://api.test/");

        Assert.That(_sut.BaseUrl, Is.EqualTo("https://api.test/"));
    }

    [Test]
    public void Then_The_Base_Url_Should_Help_Populate_The_GetUrl_Property()
    {
        var _sut = new GetCourseRoutesApiRequest("https://api.test/");

        Assert.That(_sut.GetUrl, Is.EqualTo("https://api.test/courses/routes"));
    }
}
