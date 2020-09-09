﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.FAT.Web.Models;
using System.Collections;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourseProviderDetails
    {

        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Provider_Detail_Retrieved_And_Shown(
            int providerId,
            int courseId,
            string location,
            GetCourseProviderResult response,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller
            )
        {
            //Arrange
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) && c.CourseId.Equals(courseId) && c.Location.Equals(location)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location);

            //Assert
            Assert.IsNotNull(actual);
            var actualResult = actual as ViewResult;
            Assert.IsNotNull(actualResult);
            var actualModel = actualResult.Model as CourseProviderViewModel;
            Assert.IsNotNull(actualModel);
            Assert.IsNotNull(actualModel.AdditionalCourses);
            Assert.IsNotNull(actualModel.AdditionalCourses.Courses);
        }

        [Test, MoqAutoData]
        public async Task And_Error_Then_Redirect_To_Error_Route(
            int providerId,
            int courseId,
            string location,
            Exception exception,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller
            )
        {
            // Arrange
            mediator.Setup(x => x.Send(It.Is<GetCourseProviderQuery>(c =>
                c.ProviderId.Equals(providerId) && c.CourseId.Equals(courseId)), 
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            var actual = await controller.CourseProviderDetail(courseId, providerId, location) as RedirectToRouteResult;

            // Assert
            actual.RouteName.Should().Be(RouteNames.Error500);
        }
    }
}
