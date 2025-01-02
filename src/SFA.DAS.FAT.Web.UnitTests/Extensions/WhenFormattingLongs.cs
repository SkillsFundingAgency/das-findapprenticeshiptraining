using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Extensions;

namespace SFA.DAS.FAT.Web.UnitTests.Extensions
{
    public class WhenFormattingLongs
    {
        [TestCase(1, "£1")]
        [TestCase(0, "£0")]
        [TestCase(123456, "£123,456")]
        public void Then_The_Integer_Is_Formatted_Correctly(long value, string expected)
        {
            value.ToGdsCostFormat().Should().Be(expected);
        }
    }
}
