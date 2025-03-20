using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Shortlist;
using SFA.DAS.FAT.Domain.Shortlist.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Services;

public class WhenDeletingShortlistItemForUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Request_Is_Sent_To_The_Api(
        Guid id,
        Guid shortlistUserId,
        [Frozen] Mock<IApiClient> apiClient,
        ShortlistService sut)
    {
        //Act
        await sut.DeleteShortlistItemForUser(id);

        //Assert
        apiClient.Verify(x =>
            x.Delete(
                It.Is<DeleteShortlistForUserRequest>(c => c.DeleteUrl.Contains($"shortlists/{id}", StringComparison.InvariantCultureIgnoreCase))), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Count_Is_Reduced_In_Session(
        Guid id,
        Guid shortlistUserId,
        int count,
        [Frozen] Mock<ISessionService> sessionService,
        ShortlistService sut)
    {
        ShortlistsCount shortlistsCount = new() { Count = count };
        sessionService.Setup(s => s.Get<ShortlistsCount>()).Returns(shortlistsCount);
        //Act
        await sut.DeleteShortlistItemForUser(id);

        //Assert
        sessionService.Verify(x => x.Set(It.Is<ShortlistsCount>(c => c.Count == count - 1)), Times.Once);
    }
}
