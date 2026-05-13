using NUnit.Framework;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Shared;

public class ContactDetailsViewModelTests
{
    [Test]
    public void ImplicitOperator_SourceIsNull_ReturnsEmptyViewModel()
    {
        GetProviderContactDetails source = null;

        var sut = (ContactDetailsViewModel)source;

        Assert.Multiple(() =>
        {
            Assert.That(sut.MarketingInfo, Is.EqualTo(string.Empty));
            Assert.That(sut.Email, Is.EqualTo(string.Empty));
            Assert.That(sut.PhoneNumber, Is.EqualTo(string.Empty));
            Assert.That(sut.Website, Is.EqualTo(string.Empty));
            Assert.That(sut.RegisteredAddress, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void ImplicitOperator_WebsiteMissingHttpPrefix_AddsHttpPrefix()
    {
        var source = new GetProviderContactDetails
        {
            Website = "www.provider.test"
        };

        var sut = (ContactDetailsViewModel)source;

        Assert.That(sut.Website, Is.EqualTo("http://www.provider.test"));
    }

    [TestCase("http://provider.test")]
    [TestCase("https://provider.test")]
    public void ImplicitOperator_WebsiteHasHttpPrefix_ReturnsWebsiteUnchanged(string website)
    {
        var source = new GetProviderContactDetails
        {
            Website = website
        };

        var sut = (ContactDetailsViewModel)source;

        Assert.That(sut.Website, Is.EqualTo(website));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void ImplicitOperator_WebsiteIsNullOrWhitespace_ReturnsEmptyWebsite(string website)
    {
        var source = new GetProviderContactDetails
        {
            Website = website
        };

        var sut = (ContactDetailsViewModel)source;

        Assert.That(sut.Website, Is.EqualTo(string.Empty));
    }
}
