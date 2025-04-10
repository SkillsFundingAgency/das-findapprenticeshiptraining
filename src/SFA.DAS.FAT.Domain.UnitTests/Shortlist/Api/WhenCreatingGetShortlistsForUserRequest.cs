using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Shortlist.Api;

public class WhenCreatingGetShortlistsForUserRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Built(string baseUrl, Guid shortlistUserId)
    {
        //Arrange Act
        var actual = new GetShortlistsForUserRequest(baseUrl, shortlistUserId);

        //Assert
        actual.GetUrl.Should().Be($"{baseUrl}shortlists/users/{shortlistUserId}");
    }
}
