using System;
using System.Collections.Generic;
using System.Linq;
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
            var actual = (CourseViewModel) course;
            
            //Assert
            Assert.AreEqual(course.Id, actual.Id);
            Assert.AreEqual(course.Title, actual.Title);
            Assert.AreEqual(course.MaxFunding.ToGdsCostFormat(), actual.MaximumFunding);
            Assert.AreEqual($"{course.Title} (level {course.Level})", actual.TitleAndLevel);
            Assert.AreEqual(course.Level, actual.Level);
            Assert.AreEqual(course.LevelEquivalent, actual.LevelEquivalent);
            Assert.AreEqual(course.Route, actual.Sector);
            Assert.AreEqual(course.IntegratedDegree, actual.IntegratedDegree);
            Assert.AreEqual(course.OverviewOfRole, actual.OverviewOfRole);
            Assert.AreEqual(course.CoreSkills, actual.CoreSkills);
            Assert.AreEqual(course.TypicalJobTitles, actual.TypicalJobTitles);
            Assert.AreEqual(course.StandardPageUrl, actual.ExternalCourseUrl);
            Assert.AreEqual(course.TypicalDuration, actual.TypicalDuration);
            Assert.AreEqual(course.OtherBodyApprovalRequired, actual.OtherBodyApprovalRequired);
            Assert.AreEqual(course.ApprovalBody, actual.ApprovalBody);
            Assert.AreEqual(course.StandardDates.LastDateStarts, actual.LastDateStarts);
            Assert.AreEqual(DateTime.Now > course.StandardDates?.LastDateStarts, actual.AfterLastStartDate);
        }
        

        [Test, AutoData]
        public void Then_If_CoreSkills_Is_Null_An_Empty_List_Is_Returned(Course course)
        {
            //Arrange
            course.CoreSkills = new List<string>();
            
            //Act
            var actual = (CourseViewModel) course;
            
            //Assert
            Assert.IsAssignableFrom<List<string>>(actual.CoreSkills);
            Assert.IsEmpty(actual.CoreSkills);
        }

        [Test, AutoData]
        public void Then_If_TypicalJobTitles_Is_Null_An_Empty_List_Is_Returned(Course course)
        {
            //Arrange
            course.TypicalJobTitles = new List<string>();
            
            //Act
            var actual = (CourseViewModel) course;
            
            //Assert
            Assert.IsAssignableFrom<List<string>>(actual.TypicalJobTitles);
            Assert.IsEmpty(actual.TypicalJobTitles);
        }

        [Test, AutoData]
        public void Then_If_Approval_Body_Is_Null_An_Empty_String_Is_Returned(Course course)
        {
            //Arrange
            course.ApprovalBody = null;

            //Act
            var actual = (CourseViewModel)course;

            //Assert
            Assert.IsNull(actual.ApprovalBody);
        }
    }
}
