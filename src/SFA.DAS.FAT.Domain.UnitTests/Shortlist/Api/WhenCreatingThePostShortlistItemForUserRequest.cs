using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Shortlist.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Shortlist.Api;

public class WhenCreatingThePostShortlistItemForUserRequest
{
    [Test, AutoData]
    public void Then_The_Request_Is_Built_Correctly(string baseUrl, PostShortlistForUserRequest data)
    {
        //Arrange Act
        var actual = new CreateShortlistForUserRequest(baseUrl)
        {
            Data = data
        };

        //Assert
        actual.PostUrl.Should().Be($"{baseUrl}shortlists");
        actual.Data.Should().BeEquivalentTo(data);
    }
}
