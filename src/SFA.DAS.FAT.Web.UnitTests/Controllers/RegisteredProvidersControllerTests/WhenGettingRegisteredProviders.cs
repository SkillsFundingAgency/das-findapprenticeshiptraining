using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Providers.Queries.GetRegisteredProviders;
using SFA.DAS.FAT.Domain.Providers;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.RegisteredProvidersControllerTests;
public class WhenGettingRegisteredProviders
{
    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved(
        GetRegisteredProvidersQuery request,
        GetRegisteredProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] RegisteredProvidersController controller,
        CancellationToken cancellationToken
        )
    {
        mediator.Setup(x =>
                x.Send(It.Is<GetRegisteredProvidersQuery>(c =>
                    c.Query.Equals(request.Query)), cancellationToken))
            .ReturnsAsync(response);


        //Act
        var actual = await controller.GetRegisteredProviders(request.Query, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        returnedProviders.Should().BeEquivalentTo(response.Providers);
    }

    [Test, MoqAutoData]
    public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_For_First_100_Records(
        GetRegisteredProvidersQuery request,
        GetRegisteredProvidersResult response,
        [Frozen] Mock<IMediator> mediator,
        [Greedy] RegisteredProvidersController controller,
        CancellationToken cancellationToken
    )
    {
        int numberOfMockedRegisteredProviders = 200;
        int numberOfExpectedRegisteredProviders = 100;
        var mockedRegisteredProviders = new List<RegisteredProvider>();

        for (var i = 0; i < numberOfMockedRegisteredProviders; i++)
        {
            mockedRegisteredProviders.Add(new RegisteredProvider { Name = Guid.NewGuid().ToString(), Ukprn = 1000000 + i });
        }

        response.Providers = mockedRegisteredProviders;

        mediator.Setup(x =>
                x.Send(It.Is<GetRegisteredProvidersQuery>(c =>
                    c.Query.Equals(request.Query)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        //Act
        var actual = await controller.GetRegisteredProviders(request.Query, cancellationToken);
        var actualResult = actual as OkObjectResult;
        var returnedProviders = new List<RegisteredProvider>(((IEnumerable<RegisteredProvider>)actualResult!.Value!));

        //Assert
        actual.Should().NotBeNull();
        actualResult.Should().NotBeNull();
        response.Providers.Count().Should().Be(numberOfMockedRegisteredProviders);
        returnedProviders.Count.Should().Be(numberOfExpectedRegisteredProviders);
        returnedProviders.Should().BeEquivalentTo(response.Providers.Take(numberOfExpectedRegisteredProviders));

    }
}
