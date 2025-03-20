using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public class WhenCreatingTheGetCourseApiRequest
{
    [Test, AutoData]
    public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl, int id, string locationName, int? distance)
    {
        var actual = new GetCourseApiRequest(baseUrl, id, locationName, distance);
        actual.GetUrl.Should().Be($"{baseUrl}courses/{id}?location={locationName}&distance={distance}");
    }
}
