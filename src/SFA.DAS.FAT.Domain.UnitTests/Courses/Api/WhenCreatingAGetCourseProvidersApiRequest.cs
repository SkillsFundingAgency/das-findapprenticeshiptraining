using System;
using System.Collections.Generic;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api;

namespace SFA.DAS.FAT.Domain.UnitTests.Courses.Api
{
    public class WhenCreatingAGetCourseProvidersApiRequest
    {
        [Test, AutoData]
        public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder, Guid shortlistUserId)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder, shortlistUserId: shortlistUserId);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}&shortlistUserId={shortlistUserId}");
        }

        [Test, AutoData]
        public void Then_If_No_DelvieryModes_Its_Not_Added_To_Url_And_ProviderRatings_Are_Added(string baseUrl, int id, string location, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, new List<DeliveryModeType>(), employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_DelvieryModes_Is_Null_Its_Not_Added_To_Url_And_ProviderRatings_Are_Added(string baseUrl, int id, string location, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, null, employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_EmployerProviderRating_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, new List<ProviderRating>(), apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_EmployerProviderRating_Is_Null_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, null, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_ApprenticeProviderRating_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, employerProviderRatingTypes, new List<ProviderRating>(), sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_ApprenticeProviderRating_Is_Null_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, employerProviderRatingTypes, new List<ProviderRating>(), sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_The_Location_Is_Url_Encoded(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, $"{location} & {location}", deliveryModeTypes, employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={HttpUtility.UrlEncode($"{location} & {location}")}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_The_Lat_Lon_Is_Added_If_Not_Zero(string baseUrl, int id, string location,
            List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes,
            List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder, double lat, double lon)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, $"{location} & {location}", deliveryModeTypes, employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder, lat, lon);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={HttpUtility.UrlEncode($"{location} & {location}")}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatingTypes)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatingTypes)}&lat={lat}&lon={lon}");
        }
    }
}
