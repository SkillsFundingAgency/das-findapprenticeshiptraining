using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItemForUser;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Commands;

public class WhenDeletingShortlistItemForUser
{
    [Test, MoqAutoData]
    public async Task Then_The_Command_Is_Handled_And_Service_Called(
        DeleteShortlistItemForUserCommand command,
        [Frozen] Mock<IShortlistService> service,
        DeleteShortlistItemForUserCommandHandler handler)
    {
        //Act
        await handler.Handle(command, CancellationToken.None);

        //Assert
        service.Verify(x => x.DeleteShortlistItemForUser(command.Id));
    }
}
