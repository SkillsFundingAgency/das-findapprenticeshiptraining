using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Locations.Queries.GetLocations;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Locations.Queries.GetLocations
{
    public class WhenGettingLocations
    {
        [Test, MoqAutoData]
        public async Task Then_Returns_Results_From_Service(
            GetLocationsQuery query,
            Domain.Locations locationsFromService,
            [Frozen] Mock<ILocationService> mockService,
            GetLocationsQueryHandler handler)
        {
            mockService
                .Setup(service => service.GetLocations(query.SearchTerm))
                .ReturnsAsync(locationsFromService);

            var result = await handler.Handle(query, CancellationToken.None);

            result.LocationItems.Should().BeEquivalentTo(locationsFromService.LocationItems);
        }
    }
}
