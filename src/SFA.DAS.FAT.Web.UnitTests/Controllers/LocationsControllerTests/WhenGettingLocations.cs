using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Locations.Queries.GetLocations;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.LocationsControllerTests
{
    public class WhenGettingLocations
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_Json_Returned(
            string searchTerm,
            GetLocationsQueryResponse response,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] LocationsController controller)
        {
            //Arrange
            mediator.Setup(x =>
                    x.Send(It.Is<GetLocationsQuery>(c =>
                        c.SearchTerm.Equals(searchTerm)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.Locations(searchTerm);

            //Assert
            actual.Should().NotBeNull();
            var actualResult = actual as JsonResult;
            actualResult.Should().NotBeNull();
            var model = (LocationsViewModel)actualResult!.Value;
            model.Should().NotBeNull();
        }
    }
}
