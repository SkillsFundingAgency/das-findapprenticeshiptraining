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
        public void Then_The_Get_Url_Is_Constructed_Correctly(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> providerRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder, Guid shortlistUserId)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, providerRatingTypes, apprenticeProviderRatingTypes, sortOrder, shortlistUserId: shortlistUserId);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&providerRatings={string.Join("&providerRatings=", providerRatingTypes)}&shortlistUserId={shortlistUserId}");
        }

        [Test, AutoData]
        public void Then_If_No_DelvieryModes_Its_Not_Added_To_Url_And_ProviderRating_Added(string baseUrl, int id, string location, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, new List<DeliveryModeType>(), employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&providerRatings={string.Join("&providerRatings=", employerProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_DelvieryModes_Is_Null_Its_Not_Added_To_Url_And_ProviderRating_Added(string baseUrl, int id, string location, List<ProviderRating> providerRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, null, providerRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&providerRatings={string.Join("&providerRatings=", providerRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_ProviderRating_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, new List<ProviderRating>(), new List<ProviderRating>(), sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}");
        }

        [Test, AutoData]
        public void Then_If_No_EmployerProviderRating_Is_Null_Its_Not_Added_To_Url_And_DeliveryMode_Added(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, location, deliveryModeTypes, null, new List<ProviderRating>(), sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={location}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}");
        }

        [Test, AutoData]
        public void Then_The_Location_Is_Url_Encoded(string baseUrl, int id, string location, List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> employerProviderRatingTypes, List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, $"{location} & {location}", deliveryModeTypes, employerProviderRatingTypes, apprenticeProviderRatingTypes, sortOrder);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={HttpUtility.UrlEncode($"{location} & {location}")}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&providerRatings={string.Join("&providerRatings=", employerProviderRatingTypes)}");
        }

        [Test, AutoData]
        public void Then_The_Lat_Lon_Is_Added_If_Not_Zero(string baseUrl, int id, string location,
            List<DeliveryModeType> deliveryModeTypes, List<ProviderRating> providerRatingTypes,
            List<ProviderRating> apprenticeProviderRatingTypes, int sortOrder, double lat, double lon)
        {
            //Arrange Act
            var actual = new GetCourseProvidersApiRequest(baseUrl, id, $"{location} & {location}", deliveryModeTypes, providerRatingTypes, apprenticeProviderRatingTypes, sortOrder, lat, lon);

            //Assert
            actual.GetUrl.Should().Be($"{baseUrl}trainingcourses/{id}/providers?location={HttpUtility.UrlEncode($"{location} & {location}")}&sortOrder={sortOrder}&deliveryModes={string.Join("&deliveryModes=", deliveryModeTypes)}&providerRatings={string.Join("&providerRatings=", providerRatingTypes)}&lat={lat}&lon={lon}");
        }
    }
}
