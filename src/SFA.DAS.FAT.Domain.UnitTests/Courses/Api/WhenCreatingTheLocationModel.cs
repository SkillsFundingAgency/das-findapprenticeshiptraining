using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheLocationModel
{
    [Test]
    public void Then_FormatAddress_Returns_FormattedAddress_With_All_Fields()
    {
        var sut = new LocationModel
        {
            AddressLine1 = "123 Main St",
            AddressLine2 = "Suite 100",
            Town = "Belfast",
            County = "Antrim",
            Postcode = "BT1 2AB"
        };

        Assert.That(sut.FormatAddress(), Is.EqualTo("123 Main St, Suite 100, Belfast, Antrim, BT1 2AB"));
    }

    [Test]
    public void Then_FormatAddress_Skips_Null_Or_Empty_Fields()
    {
        var sut = new LocationModel
        {
            AddressLine1 = "123 Main St",
            AddressLine2 = null,
            Town = "",
            County = "Antrim",
            Postcode = "BT1 2AB"
        };

        Assert.That(sut.FormatAddress(), Is.EqualTo("123 Main St, Antrim, BT1 2AB"));
    }

    [Test]
    public void Then_FormatAddress_Returns_Empty_When_All_Fields_Are_Null_Or_Empty()
    {
        var sut = new LocationModel
        {
            AddressLine1 = "",
            AddressLine2 = null,
            Town = null,
            County = "",
            Postcode = null
        };

        Assert.That(sut.FormatAddress(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Then_FormatAddress_Trims_Whitespace_From_Fields()
    {
        var sut = new LocationModel
        {
            AddressLine1 = "  123 Main St  ",
            County = "  Antrim ",
            Postcode = " BT1 2AB "
        };

        Assert.That(sut.FormatAddress(), Is.EqualTo("123 Main St, Antrim, BT1 2AB"));
    }
}
