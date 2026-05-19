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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.MarketingInfo, Is.EqualTo(string.Empty));
            Assert.That(sut.Email, Is.EqualTo(string.Empty));
            Assert.That(sut.PhoneNumber, Is.EqualTo(string.Empty));
            Assert.That(sut.Website, Is.EqualTo(string.Empty));
            Assert.That(sut.RegisteredAddress, Is.EqualTo(string.Empty));
        }
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

    [Test]
    public void ImplicitOperator_SourceHasContactFields_MapsFieldsToViewModel()
    {
        var source = new GetProviderContactDetails
        {
            MarketingInfo = "marketing",
            Email = "provider@test.com",
            PhoneNumber = "0123456789"
        };

        var sut = (ContactDetailsViewModel)source;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.MarketingInfo, Is.EqualTo("marketing"));
            Assert.That(sut.Email, Is.EqualTo("provider@test.com"));
            Assert.That(sut.PhoneNumber, Is.EqualTo("0123456789"));
        }
    }

    [Test]
    public void HasRegisteredAddress_RegisteredAddressIsPopulated_ReturnsTrue()
    {
        var sut = new ContactDetailsViewModel
        {
            RegisteredAddress = "1 Test Street"
        };

        Assert.That(sut.HasRegisteredAddress, Is.True);
    }

    [Test]
    public void HasRegisteredAddress_RegisteredAddressIsEmpty_ReturnsFalse()
    {
        var sut = new ContactDetailsViewModel();

        Assert.That(sut.HasRegisteredAddress, Is.False);
    }

    [Test]
    public void HasEmail_EmailIsPopulated_ReturnsTrue()
    {
        var sut = new ContactDetailsViewModel
        {
            Email = "provider@test.com"
        };

        Assert.That(sut.HasEmail, Is.True);
    }

    [Test]
    public void HasEmail_EmailIsEmpty_ReturnsFalse()
    {
        var sut = new ContactDetailsViewModel();

        Assert.That(sut.HasEmail, Is.False);
    }

    [Test]
    public void HasPhoneNumber_PhoneNumberIsPopulated_ReturnsTrue()
    {
        var sut = new ContactDetailsViewModel
        {
            PhoneNumber = "0123456789"
        };

        Assert.That(sut.HasPhoneNumber, Is.True);
    }

    [Test]
    public void HasPhoneNumber_PhoneNumberIsEmpty_ReturnsFalse()
    {
        var sut = new ContactDetailsViewModel();

        Assert.That(sut.HasPhoneNumber, Is.False);
    }

    [Test]
    public void HasWebsite_WebsiteIsPopulated_ReturnsTrue()
    {
        var sut = new ContactDetailsViewModel
        {
            Website = "http://provider.test"
        };

        Assert.That(sut.HasWebsite, Is.True);
    }

    [Test]
    public void HasWebsite_WebsiteIsEmpty_ReturnsFalse()
    {
        var sut = new ContactDetailsViewModel();

        Assert.That(sut.HasWebsite, Is.False);
    }
}
