#nullable enable
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.RegisteredProvidersControllerTests;
public class WhenGettingRegisteredProviders
{
    private const string LongQuery = "query";
    private const string UkprnSearchString = "100";
    private const int UkprnSeed = 1000000;



    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_Matched_By_Name(
        [Frozen] Mock<IProviderService> mockProviderService,
        [Greedy] RegisteredProvidersController controller,
        List<RegisteredProvider> registeredProviders,
        CancellationToken cancellationToken
    )
    {
        //Arrange
        foreach (var provider in registeredProviders)
        {
            provider.Name = LongQuery + provider.Name;
        }


        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(registeredProviders);

        //Act
        var actual = await controller.GetRegisteredProviders(LongQuery, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_Matched_By_Ukprn(
        [Frozen] Mock<IProviderService> mockProviderService,
        [Greedy] RegisteredProvidersController controller,
        CancellationToken cancellationToken
    )
    {
        //Arrange
        int numberOfMockedRegisteredProviders = 10;
        var registeredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            registeredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }


        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(registeredProviders);

        //Act
        var actual = await controller.GetRegisteredProviders(UkprnSearchString, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(registeredProviders);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Once);
    }


    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_For_First_100_Records(
        [Frozen] Mock<IProviderService> mockProviderService,
        [Greedy] RegisteredProvidersController controller,
        CancellationToken cancellationToken
    )
    {
        // Arrange
        int numberOfMockedRegisteredProviders = 200;
        int numberOfExpectedRegisteredProviders = 100;
        var mockedRegisteredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            mockedRegisteredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = UkprnSeed + i });
        }


        mockProviderService.Setup(x => x.GetRegisteredProviders()).ReturnsAsync(mockedRegisteredProviders);

        //Act
        var actual = await controller.GetRegisteredProviders(UkprnSearchString, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Count.Should().Be(numberOfExpectedRegisteredProviders);
        returnedProviders.Should().BeEquivalentTo(mockedRegisteredProviders.Take(numberOfExpectedRegisteredProviders));

    }

    [TestCase(null)]
    [TestCase("a")]
    [TestCase("ab")]
    [TestCase(" AB ")]
    [TestCase("")]
    public async Task Then_Return_No_Results_Where_SearchTerm_Is_Less_Than_3_Characters(string? searchTerm)
    {
        //Arrange
        Mock<IProviderService> mockProviderService = new Mock<IProviderService>();

        var controller = new RegisteredProvidersController(mockProviderService.Object);

        //Act
        var actual = await controller.GetRegisteredProviders(searchTerm, new CancellationToken());
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Count.Should().Be(0);
        mockProviderService.Verify(x => x.GetRegisteredProviders(), Times.Never);
    }
}
