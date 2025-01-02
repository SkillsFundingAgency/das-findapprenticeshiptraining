using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingCourseDetailViewModel
    {
        [Test, AutoData]
        public void Then_The_Model_Is_Mapped_Correctly(Course course)
        {
            //Act
            var actual = (CourseViewModel)course;
            actual.Should().BeEquivalentTo(course, options => options
                .Excluding(c => c.Route)
                .Excluding(c => c.StandardPageUrl)
                .Excluding(c => c.MaxFunding)
                .Excluding(c => c.StandardDates)
            );

            //Assert
            actual.MaximumFunding.Should().Be(course.MaxFunding.ToGdsCostFormat());
            actual.TitleAndLevel.Should().Be($"{course.Title} (level {course.Level})");
            actual.Sector.Should().Be(course.Route);
            actual.ExternalCourseUrl.Should().Be(course.StandardPageUrl);
            actual.LastDateStarts.Should().Be(course.StandardDates.LastDateStarts);
            actual.AfterLastStartDate.Should().Be(DateTime.Now > course.StandardDates?.LastDateStarts);
        }

        [Test, AutoData]
        public void Then_If_CoreSkills_Is_Null_An_Empty_List_Is_Returned(Course course)
        {
            //Arrange
            course.CoreSkills = new List<string>();

            //Act
            var actual = (CourseViewModel)course;

            //Assert
            actual.CoreSkills.Should().BeEmpty();
            actual.CoreSkills.Should().BeAssignableTo<List<string>>();
        }

        [Test, AutoData]
        public void Then_If_TypicalJobTitles_Is_Null_An_Empty_List_Is_Returned(Course course)
        {
            //Arrange
            course.TypicalJobTitles = new List<string>();

            //Act
            var actual = (CourseViewModel)course;

            //Assert
            actual.TypicalJobTitles.Should().BeEmpty();
            actual.TypicalJobTitles.Should().BeAssignableTo<List<string>>();
        }

        [Test, AutoData]
        public void Then_If_Approval_Body_Is_Null_An_Empty_String_Is_Returned(Course course)
        {
            //Arrange
            course.ApprovalBody = null;

            //Act
            var actual = (CourseViewModel)course;

            //Assert
            actual.ApprovalBody.Should().BeNull();
        }
    }
}
