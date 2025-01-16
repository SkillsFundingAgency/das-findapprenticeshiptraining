#nullable enable
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Domain.Providers.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Services;

public class WhenCallingProviderService
{
    [Test, MoqAutoData]
    public async Task Then_Return_Results_Where_SearchTerm_In_Name(
        [Frozen] Mock<IApiClient> outerApiMock,
        GetRegisteredProvidersApiRequest request,
        RegisteredProviders registeredProviders,
        ProviderService providerService,
        string searchTerm
    )
    {
        foreach (var provider in registeredProviders.Providers)
        {
            provider.Name = searchTerm + provider.Name;
        }

        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
        .ReturnsAsync(registeredProviders);

        var actual = await providerService.GetRegisteredProviders(searchTerm);

        actual.Should().BeEquivalentTo(registeredProviders.Providers);
    }

    [Test, MoqAutoData]
    public async Task Then_Return_Results_Where_SearchTerm_In_Ukprn(
        [Frozen] Mock<IApiClient> outerApiMock,
        GetRegisteredProvidersApiRequest request,
        RegisteredProviders registeredProviders,
        ProviderService providerService,
        int searchTerm
    )
    {
        if (searchTerm < 100) searchTerm += 100;

        foreach (var provider in registeredProviders.Providers)
        {
            provider.Ukprn = searchTerm;
        }

        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
            .ReturnsAsync(registeredProviders);

        var actual = await providerService.GetRegisteredProviders(searchTerm.ToString());

        actual.Should().BeEquivalentTo(registeredProviders.Providers);
    }

    [TestCase(null)]
    [TestCase("a")]
    [TestCase("ab")]
    [TestCase(" AB ")]
    [TestCase("")]
    public async Task Then_Return_No_Results_Where_SearchTerm_Is_Less_Than_3_Characters(string? searchTerm)
    {
        Mock<IApiClient> outerApiMock = new Mock<IApiClient>();
        Mock<IOptions<FindApprenticeshipTrainingApi>> options = new Mock<IOptions<FindApprenticeshipTrainingApi>>();

        RegisteredProviders registeredProviders = new RegisteredProviders
        {
            Providers = new List<RegisteredProvider> { new() }
        };

        outerApiMock
            .Setup(o => o.Get<RegisteredProviders>(It.IsAny<GetRegisteredProvidersApiRequest>()))
            .ReturnsAsync(registeredProviders);

        ProviderService providerService = new ProviderService(outerApiMock.Object, options.Object);

        var actual = await providerService.GetRegisteredProviders(searchTerm);
        actual.Count().Should().Be(0);
    }
}
