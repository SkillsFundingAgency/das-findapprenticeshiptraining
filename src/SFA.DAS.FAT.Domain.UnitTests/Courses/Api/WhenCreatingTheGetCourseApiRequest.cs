using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api
{
    public class WhenCreatingTheGetCourseApiRequest
    {
        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl, int id, double lat, double lon)
        {
            //Arrange Act
            var actual = new GetCourseApiRequest(baseUrl, id,lat,lon);
            
            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}?lat={lat}&lon={lon}&shortlistUserId=");
        }
        
        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_ShortlistUserId(string baseUrl, int id, double lat, double lon, Guid shortlistUserId)
        {
            //Arrange Act
            var actual = new GetCourseApiRequest(baseUrl, id,lat,lon, shortlistUserId);
            
            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}?lat={lat}&lon={lon}&shortlistUserId={shortlistUserId}");
        }
    }
}