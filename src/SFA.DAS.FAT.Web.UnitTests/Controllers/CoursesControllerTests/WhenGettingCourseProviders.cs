﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Controllers;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;
using DeliveryModeType = SFA.DAS.FAT.Web.Models.DeliveryModeType;

namespace SFA.DAS.FAT.Web.UnitTests.Controllers.CoursesControllerTests
{
    public class WhenGettingCourseProviders
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Data_Retrieved_And_View_Shown(
            int standardCode,
            string location,
            List<DeliveryModeType> deliveryModes,
            GetCourseProvidersResult response,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            var sortOrder = ProviderSortBy.Name;
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProvidersQuery>(c => c.CourseId.Equals(standardCode) 
                    && c.Location.Equals(location)
                    && c.DeliveryModes.SequenceEqual(deliveryModes.Select(type => (Domain.Courses.DeliveryModeType)type))
                    && c.SortOrder.Equals(sortOrder)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            //Act
            var actual = await controller.CourseProviders(standardCode, location, deliveryModes, sortOrder) as ViewResult;
            
            //Assert
            var actualModel = actual.Model as CourseProvidersViewModel;
            actualModel.Providers.Should().BeEquivalentTo(response.Providers.Select(provider => (ProviderViewModel)provider));
            actualModel.Course.Should().BeEquivalentTo((CourseViewModel)response.Course);
            actualModel.Total.Should().Be(response.Total);
            actualModel.TotalFiltered.Should().Be(response.TotalFiltered);
            actualModel.Location.Should().Be(response.Location);
            actualModel.SortOrder.Should().Be(sortOrder);
            actualModel.HasLocations.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_Sets_DeliveryModes(
            int standardCode,
            string location,
            List<DeliveryModeType> deliveryModes,
            GetCourseProvidersResult response,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            var expectedDeliveryModes = new List<DeliveryModeOptionViewModel>();
            foreach (DeliveryModeType deliveryModeType in Enum.GetValues(typeof(DeliveryModeType)))
            {
                expectedDeliveryModes.Add(new DeliveryModeOptionViewModel
                {
                    DeliveryModeType = deliveryModeType,
                    Description = deliveryModeType.GetDescription(),
                    Selected = deliveryModes.Any(type => type == deliveryModeType)
                });
            }
            mediator.Setup(x => x.Send(
                    It.IsAny<GetCourseProvidersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            //Act
            var actual = await controller.CourseProviders(standardCode, location, deliveryModes, ProviderSortBy.Name) as ViewResult;
            
            //Assert
            var actualModel = actual.Model as CourseProvidersViewModel;
            actualModel.DeliveryModes.Should().BeEquivalentTo(expectedDeliveryModes);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Location_Is_Added_To_The_Cookie_If_Set(
            int standardCode,
            string location,
            List<DeliveryModeType> deliveryModes,
            GetCourseProvidersResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<string>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            var sortOrder = ProviderSortBy.Name;
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProvidersQuery>(c => c.CourseId.Equals(standardCode) 
                                                        && c.Location.Equals(location)
                                                        && c.SortOrder.Equals(sortOrder)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            //Act
            var actual = await controller.CourseProviders(standardCode, location, deliveryModes, sortOrder) as ViewResult;
            var actualModel = actual.Model as CourseProvidersViewModel;
            
            //Assert
            cookieStorageService.Verify(x=>x.Update(Constants.LocationCookieName,location,2));
            actualModel.HasLocations.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_Location_Is_Removed_From_The_Cookie_If_Set_To_Minus_One(
            int standardCode,
            List<DeliveryModeType> deliveryModes,
            GetCourseProvidersResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<string>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            response.Location = null;
            var sortOrder = ProviderSortBy.Name;
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProvidersQuery>(c => c.CourseId.Equals(standardCode) 
                                                        && c.Location.Equals(string.Empty)
                                                        && c.SortOrder.Equals(sortOrder)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            //Act
            var actual = await controller.CourseProviders(standardCode, "-1", deliveryModes, sortOrder) as ViewResult;
            
            //Assert
            var actualModel = actual.Model as CourseProvidersViewModel;
            actualModel.Providers.Should().BeEquivalentTo(response.Providers.Select(provider => (ProviderViewModel)provider));
            actualModel.Course.Should().BeEquivalentTo((CourseViewModel)response.Course);
            actualModel.Total.Should().Be(response.Total);
            actualModel.TotalFiltered.Should().Be(response.TotalFiltered);
            actualModel.SortOrder.Should().Be(sortOrder);
            actualModel.HasLocations.Should().BeFalse();
            cookieStorageService.Verify(x=>x.Delete(Constants.LocationCookieName));
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_Location_Stored_In_Cookie_It_Is_Used_For_Results_And_Cookie_Updated(
            int standardCode,
            string location,
            List<DeliveryModeType> deliveryModes,
            GetCourseProvidersResult response,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<ICookieStorageService<string>> cookieStorageService,
            [Greedy] CoursesController controller)
        {
            //Arrange
            var sortOrder = ProviderSortBy.Name;
            cookieStorageService.Setup(x => x.Get(Constants.LocationCookieName)).Returns(location);
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProvidersQuery>(c => c.CourseId.Equals(standardCode) 
                                                        && c.Location.Equals(location)
                                                        && c.SortOrder.Equals(sortOrder)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            //Act
            var actual = await controller.CourseProviders(standardCode, location, deliveryModes, sortOrder) as ViewResult;
            
            //Assert
            var actualModel = actual.Model as CourseProvidersViewModel;
            actualModel.Providers.Should().BeEquivalentTo(response.Providers.Select(provider => (ProviderViewModel)provider));
            actualModel.HasLocations.Should().BeTrue();
            cookieStorageService.Verify(x=>x.Update(Constants.LocationCookieName,location,2));
        }

        [Test, MoqAutoData]
        public async Task And_Error_Then_Redirect_To_Error_Route(
            int standardCode,
            string location,
            List<DeliveryModeType> deliveryModes,
            ProviderSortBy sortOrder,
            Exception exception,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] CoursesController controller)
        {
            //Arrange
            mediator.Setup(x => x.Send(
                    It.Is<GetCourseProvidersQuery>(c => c.CourseId.Equals(standardCode)),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            
            //Act
            var actual = await controller.CourseProviders(standardCode, location, deliveryModes, sortOrder) as RedirectToRouteResult;
            
            //Assert
            actual.RouteName.Should().Be(RouteNames.Error500);
        }
    }
}
