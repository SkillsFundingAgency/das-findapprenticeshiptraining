using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Shortlist.Api;

public class WhenCreatingGetShortlistsCountForUserRequest
{
    [Test, AutoData]
    public void Then_The_Request_Is_Built_Correctly(string baseUrl, Guid shortlistUserId)
    {
        //Arrange Act
        var actual = new GetShortlistsCountForUserRequest(baseUrl, shortlistUserId);
        //Assert
        actual.GetUrl.Should().Be($"{baseUrl}shortlists/users/{shortlistUserId}/count");
    }
}
