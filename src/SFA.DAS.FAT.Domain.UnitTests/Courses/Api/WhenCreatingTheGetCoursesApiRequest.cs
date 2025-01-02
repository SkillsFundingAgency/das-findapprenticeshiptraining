using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api
{
    public class WhenCreatingTheGetCoursesApiRequest
    {
        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_Levels_And_Routes(string baseUrl, string keyword, List<string> sectors, List<int> levels)
        {
            //Arrange Act
            var actual = new GetCoursesApiRequest(baseUrl, keyword, sectors, levels);
            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}&routeIds={string.Join("&routeIds=", sectors.Select(HttpUtility.HtmlEncode))}&levels={string.Join("&levels=", levels)}");
        }

        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_Routes_And_Levels_Is_Null(string baseUrl, string keyword, List<string> sectors)
        {
            //Arrange Act
            var actual = new GetCoursesApiRequest(baseUrl, keyword, sectors);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}&routeIds={string.Join("&routeIds=", sectors.Select(HttpUtility.HtmlEncode))}");
        }

        [Test, AutoData]
        public void Then_If_The_List_Of_Sectors_And_Levels_Are_Null_The_Url_Is_Constructed_Correctly(string baseUrl, string keyword)
        {
            //Arrange Act
            var actual = new GetCoursesApiRequest(baseUrl, keyword);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}");
        }


        [Test, AutoData]
        public void Then_If_The_List_Of_Sectors_And_Levels_Are_Empty_The_Url_Is_Constructed_Correctly(string baseUrl, string keyword)
        {
            //Arrange Act
            var actual = new GetCoursesApiRequest(baseUrl, keyword, new List<string>(), new List<int>());

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}");
        }

        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_Levels_And_Routes_Null(string baseUrl, string keyword, List<int> levels)
        {
            //Arrange Act
            var actual = new GetCoursesApiRequest(baseUrl, keyword, null, levels);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}&levels={string.Join("&levels=", levels)}");
        }

        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_OrderBy(string baseUrl)
        {
            //Arrange Act
            var orderBy = OrderBy.Name;
            var actual = new GetCoursesApiRequest(baseUrl, "", null, null, orderBy);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword=&orderby={orderBy}");
        }

        [Test, AutoData]
        public void Then_OrderBy_Is_Not_Added_If_None(string baseUrl, string keyword)
        {
            //Arrange Act
            var orderBy = OrderBy.None;
            var actual = new GetCoursesApiRequest(baseUrl, keyword, null, null, orderBy);

            //Assert
            actual.GetUrl.Should().NotContain("orderby");
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}");

        }


        [Test, AutoData]
        public void Then_ShortlistId_Is_Not_Added_If_None(string baseUrl, string keyword)
        {
            //Arrange Act
            var orderBy = OrderBy.None;
            var actual = new GetCoursesApiRequest(baseUrl, keyword, null, null, orderBy, null);

            //Assert
            actual.GetUrl.Should().NotContain("shortlistUserId");
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}");

        }

        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly_With_Levels_And_Routes_And_OrderBy_And_ShortlistUserId(string baseUrl, string keyword, List<string> sectors, List<int> levels, Guid shortlistUserId)
        {
            //Arrange Act
            var orderBy = OrderBy.Relevance;
            var actual = new GetCoursesApiRequest(baseUrl, keyword, sectors, levels, orderBy, shortlistUserId);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses?keyword={keyword}&orderby={orderBy}&routeIds={string.Join("&routeIds=", sectors.Select(HttpUtility.HtmlEncode))}&levels={string.Join("&levels=", levels)}&shortlistUserId={shortlistUserId}");
        }
    }
}
