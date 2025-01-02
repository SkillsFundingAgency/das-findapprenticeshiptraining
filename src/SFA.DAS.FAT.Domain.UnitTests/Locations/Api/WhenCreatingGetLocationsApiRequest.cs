using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Locations.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Locations.Api
{
    public class WhenCreatingGetLocationsApiRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Built(string baseUrl, string searchTerm)
        {
            //Arrange Act
            var actual = new GetLocationsApiRequest(baseUrl, searchTerm);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}locations?searchTerm={searchTerm}");
        }
    }
}
