using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api;

public sealed class WhenCreatingTheProviderCourseModel
{
    [Test]
    public void Then_Name_And_Level_Should_Combine_Course_Name_And_Level()
    {
        var sut = new ProviderCourseModel
        {
            CourseName = "Software Developer",
            Level = 4
        };

        Assert.That(sut.NameAndLevel, Is.EqualTo("Software Developer (level 4)"));
    }
}
