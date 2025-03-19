using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Shortlist.Api;

public class WhenCreatingTheDeleteShortlistItemForUserRequest
{
    [Test, AutoData]
    public void Then_The_Request_Is_Built_Correctly(string baseUrl, Guid id, Guid shortlistUserid)
    {
        //Arrange Act
        var actual = new DeleteShortlistForUserRequest(baseUrl, id, shortlistUserid);

        //Assert
        actual.DeleteUrl.Should().Be($"{baseUrl}shortlists/{id}");
    }
}
