using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingProviderCourseViewModel
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Correctly_Mapped(AdditionalCourse source)
        {
            var actual = (ProviderCourseViewModel)source;

            actual.TitleAndLevel.Should().Be($"{source.Title} (level {source.Level})");
            actual.Should().BeEquivalentTo(source);
        }
    }
}
