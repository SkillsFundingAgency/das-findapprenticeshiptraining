﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Application.UnitTests.Courses.Queries.GetCourseProviderDetails
{
    public class WhenGettingCourseProviderDetails
    {
        [Test, MoqAutoData]
        public async Task Then_If_The_Query_Is_Valid_The_Service_Is_Called_And_The_Data_Returned(
            GetCourseProviderQuery request,
            TrainingCourseProviderDetails courseProviderResponse,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseProviderQueryHandler handler)
        {
            //Arrange
            mockService.Setup(x => x.GetCourseProviderDetails(request.ProviderId, request.CourseId, request.Location, request.Lat, request.Lon, request.ShortlistUserId.Value)).ReturnsAsync(courseProviderResponse);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourseProviderDetails(request.ProviderId, request.CourseId, request.Location, request.Lat, request.Lon, request.ShortlistUserId.Value), Times.Once);
            Assert.IsNotNull(actual);
            actual.Provider.Should().BeEquivalentTo(courseProviderResponse.CourseProviderDetails);
            actual.Course.Should().BeEquivalentTo(courseProviderResponse.TrainingCourse);
            actual.AdditionalCourses.Should().BeEquivalentTo(courseProviderResponse.AdditionalCourses);
            actual.Location.Should().Be(courseProviderResponse.Location.Name);
            actual.LocationGeoPoint.Should().BeEquivalentTo(courseProviderResponse.Location.LocationPoint.GeoPoint);
            actual.ProvidersAtLocation.Should().Be(courseProviderResponse.ProvidersCount.ProvidersAtLocation);
            actual.TotalProviders.Should().Be(courseProviderResponse.ProvidersCount.TotalProviders);
            actual.ShortlistItemCount.Should().Be(courseProviderResponse.ShortlistItemCount);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Course_Provider_Returns_Null(
            GetCourseProviderQuery request,
            Provider courseProviderResponse,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<ICourseService> mockService,
            GetCourseProviderQueryHandler handler)
        {
            //Arrange
            mockService.Setup(x => x.GetCourseProviderDetails(request.ProviderId, request.CourseId, request.Location, request.Lat, request.Lon, request.ShortlistUserId.Value)).ReturnsAsync((TrainingCourseProviderDetails)null);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(x => x.GetCourseProviderDetails(request.ProviderId, request.CourseId, request.Location, request.Lat, request.Lon, request.ShortlistUserId.Value), Times.Once);
            Assert.IsNull(actual.Provider);
            Assert.IsNull(actual.Course);
            Assert.IsNull(actual.AdditionalCourses);
            Assert.IsNull(actual.Location);
            Assert.IsNull(actual.LocationGeoPoint);
            Assert.AreEqual(0, actual.ShortlistItemCount);
            Assert.AreEqual(0, actual.TotalProviders);
            Assert.AreEqual(0, actual.ProvidersAtLocation);
        }
    }
}
