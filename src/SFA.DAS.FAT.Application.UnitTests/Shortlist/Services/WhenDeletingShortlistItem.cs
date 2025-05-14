using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services;

public class WhenDeletingShortlistItem
{
    [Test, MoqAutoData]
    public async Task Then_The_Request_Is_Sent_To_The_Api(
        Guid id,
        Guid shortlistUserId,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        apiClient.Setup(x => x.Delete<DeleteShortlistItemResponse>(It.IsAny<DeleteShortlistItemRequest>()))
            .ReturnsAsync(new DeleteShortlistItemResponse(true));
        //Act
        await sut.DeleteShortlistItem(id);

        //Assert
        apiClient.Verify(x =>
            x.Delete<DeleteShortlistItemResponse>(
                It.Is<DeleteShortlistItemRequest>(c => c.DeleteUrl.Contains($"shortlists/{id}", StringComparison.InvariantCultureIgnoreCase))), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Reduced_In_Session_If_Successfully_Deleted(
        Guid id,
        int count,
        [Frozen] Mock<ISessionService> sessionService,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        ShortlistsCount shortlistsCount = new() { Count = count };
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(shortlistsCount);
        apiClient.Setup(x => x.Delete<DeleteShortlistItemResponse>(It.IsAny<DeleteShortlistItemRequest>()))
            .ReturnsAsync(new DeleteShortlistItemResponse(true));

        //Act
        await sut.DeleteShortlistItem(id);

        //Assert
        sessionService.Verify(x => x.Set(It.Is<ShortlistsCount>(c => c.Count == count - 1)), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Not_Updated_In_Session_If_Item_Not_Deleted(
        Guid id,
        int count,
        [Frozen] Mock<ISessionService> sessionService,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        ShortlistsCount shortlistsCount = new() { Count = count };
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(shortlistsCount);
        apiClient.Setup(x => x.Delete<DeleteShortlistItemResponse>(It.IsAny<DeleteShortlistItemRequest>()))
            .ReturnsAsync(new DeleteShortlistItemResponse(false));

        //Act
        await sut.DeleteShortlistItem(id);

        //Assert
        sessionService.Verify(x => x.Set(It.IsAny<ShortlistsCount>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_If_Session_Is_Empty_Count_Is_Not_Updated(
        Guid id,
        [Frozen] Mock<IApiClient> apiClient,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        apiClient.Setup(x => x.Delete<DeleteShortlistItemResponse>(It.IsAny<DeleteShortlistItemRequest>()))
            .ReturnsAsync(new DeleteShortlistItemResponse(true));
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(() => null);
        //Act
        await sut.DeleteShortlistItem(id);

        //Assert
        sessionService.Verify(x => x.Set(It.IsAny<ShortlistsCount>()), Times.Never);
    }
}
