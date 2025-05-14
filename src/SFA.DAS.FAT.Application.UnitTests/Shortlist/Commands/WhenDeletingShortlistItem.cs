using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Shortlist.Commands.DeleteShortlistItem;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Shortlist.Commands;

public class WhenDeletingShortlistItem
{
    [Test, MoqAutoData]
    public async Task Then_The_Command_Is_Handled_And_Service_Called(
        DeleteShortlistItemCommand command,
        [Frozen] Mock<IShortlistService> service,
        DeleteShortlistItemCommandHandler handler)
    {
        //Act
        await handler.Handle(command, CancellationToken.None);

        //Assert
        service.Verify(x => x.DeleteShortlistItem(command.Id));
    }
}
