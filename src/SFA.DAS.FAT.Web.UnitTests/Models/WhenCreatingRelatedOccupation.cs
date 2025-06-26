using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

namespace SFA.DAS.FAT.Web.UnitTests.Models;
public class WhenCreatingRelatedOccupation
{
    [TestCase("Plumber", 2, "Plumber (level 2)")]
    [TestCase("Advanced plumber", 5, "Advanced plumber (level 5)")]
    public void Then_Description_Is_Built_From_Title_And_Level(string title, int level, string expectedDescription)
    {
        var sut = new RelatedOccupation { Title = title, Level = level };
        sut.Description.Should().Be(expectedDescription);
    }
}
