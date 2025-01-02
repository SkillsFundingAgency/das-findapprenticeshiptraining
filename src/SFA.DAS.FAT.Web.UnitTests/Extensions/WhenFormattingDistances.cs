using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Extensions;

namespace SFA.DAS.FAT.Web.UnitTests.Extensions
{
    public class WhenFormattingDistances
    {
        [TestCase(1.0, "1")]
        [TestCase(1.1, "1.1")]
        [TestCase(10.9, "10.9")]
        [TestCase(10.89, "10.9")]
        [TestCase(10.0, "10")]
        [TestCase(0.1, "0.1")]
        [TestCase(0, "0")]
        [TestCase(0.0, "0")]
        [TestCase(null, "")]
        public void Then_The_Trailing_Zero_Is_Removed(decimal? value, string expected)
        {
            value.FormatDistance().Should().Be(expected);
        }
    }
}
