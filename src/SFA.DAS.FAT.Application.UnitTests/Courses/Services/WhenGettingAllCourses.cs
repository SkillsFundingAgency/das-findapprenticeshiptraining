using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Services
{
    public class WhenGettingAllCourses
    {
        [Test, MoqAutoData]
        public async Task Then_The_Api_Client_Is_Called_With_The_Request(
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            var courseApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, null);
            
            //Act
            await courseService.GetCourses(null, null, null, OrderBy.None, null);
            
            //Assert
            apiClient.Verify(x=>x.Get<TrainingCourses>(
                It.Is<GetCoursesApiRequest>(request => request.GetUrl.Equals(courseApiRequest.GetUrl))));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_RouteIds_Are_Added_To_The_Request(
            string keyword,
            List<string> routeIds,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, routeIds);

            //Act
            await courseService.GetCourses(keyword, routeIds, null, OrderBy.None, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.Sectors.Equals(coursesApiRequest.Sectors))));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_Levels_Are_Added_To_The_Request(
        string keyword,
        List<int> levels,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, null, levels);

            //Act
            await courseService.GetCourses(keyword, null, levels, OrderBy.None, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.Levels.Equals(coursesApiRequest.Levels))));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_Levels_And_Sectors_Are_Added_To_The_Request(
        string keyword,
        List<string> routeIds,
        List<int> levels,
        [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
        [Frozen] Mock<IApiClient> apiClient,
        CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, routeIds, levels);

            //Act
            await courseService.GetCourses(keyword, routeIds, levels, OrderBy.None, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.Levels.Equals(coursesApiRequest.Levels)
                    && request.Sectors.Equals(coursesApiRequest.Sectors))));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_Is_Added_To_The_Request_And_OrderBy_Is_Default(
            string keyword,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, null, null, OrderBy.None);

            //Act
            await courseService.GetCourses(keyword, null, null, OrderBy.None, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.OrderBy.Equals(coursesApiRequest.OrderBy)
                    )));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_OrderBy_Is_Added_To_The_Request(
            string keyword,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, null, null, OrderBy.Name);

            //Act
            await courseService.GetCourses(keyword, null, null, OrderBy.Name, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.OrderBy.Equals(coursesApiRequest.OrderBy)
                    )));
        }

        [Test, MoqAutoData]
        public async Task Then_The_Keyword_And_OrderBy_Is_Set_To_Relevance_Added_To_The_Request(
            string keyword,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Arrange
            var coursesApiRequest = new GetCoursesApiRequest(config.Object.Value.BaseUrl, keyword, null, null, OrderBy.Relevance);

            //Act
            await courseService.GetCourses(keyword, null, null, OrderBy.Relevance, null);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.Keyword.Equals(coursesApiRequest.Keyword)
                    && request.OrderBy.Equals(coursesApiRequest.OrderBy)
                    )));
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_ShortlistUserId_Is_Added_To_The_Request(
            Guid shortlistUserId,
            [Frozen] Mock<IOptions<FindApprenticeshipTrainingApi>> config,
            [Frozen] Mock<IApiClient> apiClient,
            CourseService courseService)
        {
            //Act
            await courseService.GetCourses(null, null, null, OrderBy.Relevance, shortlistUserId);

            //Assert
            apiClient.Verify(x =>
                x.Get<TrainingCourses>(It.Is<GetCoursesApiRequest>(request =>
                    request.GetUrl.Contains($"&shortlistUserId={shortlistUserId}")
                )));
        }
    }
}