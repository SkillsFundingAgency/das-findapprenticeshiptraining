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
        CreateShortlistForUserResponse apiResponse,
        PostShortlistForUserRequest postShortlistForUserRequest,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        //Arrange
        apiClient
            .Setup(x =>
                x.Post<CreateShortlistForUserResponse, PostShortlistForUserRequest>(
                    It.Is<CreateShortlistForUserRequest>(c => c.Data == postShortlistForUserRequest)))
            .ReturnsAsync(apiResponse);

        //Act
        var actual = await sut.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        actual.Should().Be(apiResponse.ShortlistId);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Increased_In_Session_If_Shortlist_Got_Created(
        Guid shortlistId,
        PostShortlistForUserRequest postShortlistForUserRequest,
        int count,
        [Frozen] Mock<IApiClient> apiClient,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        //Arrange
        CreateShortlistForUserResponse apiResponse = new(shortlistId, true);
        apiClient.Setup(x => x.Post<CreateShortlistForUserResponse, PostShortlistForUserRequest>(It.IsAny<CreateShortlistForUserRequest>())).ReturnsAsync(apiResponse);
        ShortlistsCount shortlistsCount = new() { Count = count };
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(shortlistsCount);
        //Act
        await sut.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        sessionService.Verify(x => x.Set(It.Is<ShortlistsCount>(c => c.Count == count + 1)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Not_Increased_In_Session_If_Shortlist_Was_Not_Created(
        Guid shortlistId,
        PostShortlistForUserRequest postShortlistForUserRequest,
        [Frozen] Mock<IApiClient> apiClient,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        //Arrange
        CreateShortlistForUserResponse apiResponse = new(shortlistId, false);
        apiClient.Setup(x => x.Post<CreateShortlistForUserResponse, PostShortlistForUserRequest>(It.IsAny<CreateShortlistForUserRequest>())).ReturnsAsync(apiResponse);
        //Act
        await sut.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        sessionService.Verify(s => s.Get<ShortlistsCount>(), Times.Never);
        sessionService.Verify(x => x.Set(It.IsAny<ShortlistsCount>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Session_Is_Empty_Then_Count_Is_Not_Changed(
        Guid shortlistId,
        PostShortlistForUserRequest postShortlistForUserRequest,
        int count,
        [Frozen] Mock<IApiClient> apiClient,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        //Arrange
        CreateShortlistForUserResponse apiResponse = new(shortlistId, true);
        apiClient.Setup(x => x.Post<CreateShortlistForUserResponse, PostShortlistForUserRequest>(It.IsAny<CreateShortlistForUserRequest>())).ReturnsAsync(apiResponse);
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(() => null);
        //Act
        await sut.CreateShortlistItemForUser(postShortlistForUserRequest);

        //Assert
        sessionService.Verify(s => s.Get<ShortlistsCount>(), Times.Once);
        sessionService.Verify(x => x.Set(It.IsAny<ShortlistsCount>()), Times.Never);
    }
}
