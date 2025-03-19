using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services;

public class WhenCreatingShortlistItemForUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Request_Is_Sent_To_The_Api(
        Guid itemId,
        Guid shortlistUserId,
        PostShortlistForUserRequest postShortlistForUserRequest,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService service)
    {
        //Arrange
        apiClient
            .Setup(x =>
                x.Post<string, PostShortlistForUserRequest>(
                    It.Is<CreateShortlistForUserRequest>(c => c.PostUrl.Contains($"shortlist", StringComparison.InvariantCultureIgnoreCase)
                    && c.Data == postShortlistForUserRequest)))
            .ReturnsAsync(itemId.ToString);

        //Act
        var actual = await service.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        actual.Should().Be(itemId);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Increased_In_Session(
        Guid itemId,
        PostShortlistForUserRequest postShortlistForUserRequest,
        int count,
        [Frozen] Mock<IApiClient> apiClient,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        //Arrange
        apiClient.Setup(x => x.Post<string, PostShortlistForUserRequest>(It.IsAny<CreateShortlistForUserRequest>())).ReturnsAsync(itemId.ToString);
        ShortlistsCount shortlistsCount = new() { Count = count };
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(shortlistsCount);
        //Act
        await sut.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        sessionService.Verify(x => x.Set(It.Is<ShortlistsCount>(c => c.Count == count + 1)), Times.Once);
    }
}
