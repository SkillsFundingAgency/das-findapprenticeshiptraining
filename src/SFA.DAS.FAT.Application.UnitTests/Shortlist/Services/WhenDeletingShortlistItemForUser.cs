using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
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
        await sut.DeleteShortlistItemForUser(id, shortlistUserId);

        //Assert
        apiClient.Verify(x =>
            x.Delete(
                It.Is<DeleteShortlistForUserRequest>(c => c.DeleteUrl.Contains($"shortlists/{id}", StringComparison.InvariantCultureIgnoreCase))), Times.Once);
    }
}
