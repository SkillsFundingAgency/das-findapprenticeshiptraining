using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public class WhenCreatingTheGetCourseApiRequest
{
    [Test, AutoData]
    public void WhenGettingUrl_ThenConstructsCorrectly(string baseUrl, string id, string locationName, int? distance)
    {
        var actual = new GetCourseApiRequest(baseUrl, id, locationName, distance);
        actual.GetUrl.Should().Be($"{baseUrl}courses/{id}?locationName={locationName}&distance={distance}");
    }
}
