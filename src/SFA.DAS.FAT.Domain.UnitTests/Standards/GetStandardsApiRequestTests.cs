using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Standards;

namespace SFA.DAS.FAT.Domain.UnitTests.Standards;

public class GetStandardsApiRequestTests
{
    [Test]
    public void Constructor_ShouldSetBaseUrl()
    {
        // Arrange
        var baseUrl = "https://example.com/api/";

        // Act
        var request = new GetStandardsApiRequest(baseUrl);

        // Assert
        request.GetUrl.Should().Be($"{baseUrl}standards");
    }
}
