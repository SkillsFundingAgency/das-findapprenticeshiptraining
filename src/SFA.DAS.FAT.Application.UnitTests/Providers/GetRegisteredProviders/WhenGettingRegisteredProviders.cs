using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Providers.GetRegisteredProviders;
public class WhenGettingRegisteredProviders
{
    [Test, MoqAutoData]
    public async Task Then_If_The_Query_Is_Valid_The_Service_Is_Called_And_The_Data_Returned(
        GetRegisteredProvidersQuery request,
        GetRegisteredProvidersResult result,
        [Frozen] ValidationResult validationResult,
        [Frozen] Mock<IProviderService> mockService,
        GetRegisteredProvidersQueryHandler handler)
    {
        //Arrange
        mockService.Setup(x => x.GetRegisteredProviders(request.Query)).ReturnsAsync(result.Providers);

        //Act
        var actual = await handler.Handle(request, CancellationToken.None);

        //Assert
        mockService.Verify(x => x.GetRegisteredProviders(request.Query), Times.Once);
        actual.Should().NotBeNull();
        actual.Providers.Should().BeEquivalentTo(result.Providers);
    }
}
