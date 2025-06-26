using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    class WhenCreatingLevelViewModel
    {
        [Test, AutoData]
        public void Then_The_Title_Is_Code_And_Name_Combined(Level level)
        {
            // Arrange
            var levelViewModel = new LevelViewModel
            {
                Name = level.Name,
                Code = level.Code
            };

            // Act
            var actual = levelViewModel.Title;

            // Assert
            actual.Should().Be("Level " + levelViewModel.Code + " - " + levelViewModel.Name);
        }
    }
}
