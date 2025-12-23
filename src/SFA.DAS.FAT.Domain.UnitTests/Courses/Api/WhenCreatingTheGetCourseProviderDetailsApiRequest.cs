using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public class WhenCreatingTheGetCourseProviderDetailsApiRequest
{
    [Test, AutoData]
    public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl, string larsCode, int ukprn, int distance, string location, Guid shortlistUserId)
    {
        var actual = new GetCourseProviderDetailsApiRequest(baseUrl, larsCode, ukprn, location, distance, shortlistUserId);

        actual.GetUrl.Should().Be($"{baseUrl}courses/{larsCode}/providers/{ukprn}?location={location}&distance={distance}&shortlistUserId={shortlistUserId}");
    }
}
