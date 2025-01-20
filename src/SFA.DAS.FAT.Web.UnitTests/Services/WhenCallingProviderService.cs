using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Domain.Providers.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class WhenCallingProviderService
{
    [Test, MoqAutoData]
    public async Task Then_Return_Results(
        [Frozen] Mock<IApiClient> outerApiMock,
        GetRegisteredProvidersApiRequest request,
        RegisteredProviders registeredProviders,
        ProviderService providerService
    )
    {
        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
        .ReturnsAsync(registeredProviders);

        var actual = await providerService.GetRegisteredProviders();

        actual.Should().BeEquivalentTo(registeredProviders.Providers);
    }
}
