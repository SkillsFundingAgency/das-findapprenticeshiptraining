using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Providers.Api.Requests;

namespace SFA.DAS.FAT.Domain.UnitTests.Providers.Api;
public class WhenCreatingGetProviderApiRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Built(string baseUrl, int ukprn)
    {
        //Arrange Act
        var actual = new GetProviderApiRequest(baseUrl, ukprn);

        //Assert
        actual.GetUrl.Should().Be($"{baseUrl}providers/{ukprn}");
    }
}
