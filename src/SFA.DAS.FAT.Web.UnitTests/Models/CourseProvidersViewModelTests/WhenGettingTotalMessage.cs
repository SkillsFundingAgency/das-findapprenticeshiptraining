using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Web.Models;
using DeliveryModeType = SFA.DAS.FAT.Web.Models.DeliveryModeType;
using EmployerProviderRating = SFA.DAS.FAT.Web.Models.ProviderRating;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenGettingTotalMessage
    {
        [Test]
        [InlineAutoData(10, 5, false, "10 results")]
        [InlineAutoData(10, 5, true, "5 results")]
        [InlineAutoData(1, 5, false, "1 result")]
        [InlineAutoData(5, 1, true, "1 result")]
        public void Then_The_Total_Message_Is_Created_Correctly_For_Delivery_Modes(
            int totalCount,
            int filterTotal,
            bool hasFilter,
            string expectedMessage,
            Dictionary<uint, string> providerOrder,
            GetCourseProvidersResult result)
        {
            //arrange
            var selectedDeliveryModes = new List<DeliveryModeType>();
            if (hasFilter)
            {
                selectedDeliveryModes.Add(DeliveryModeType.BlockRelease);
            }
            var request = new GetCourseProvidersRequest { DeliveryModes = selectedDeliveryModes };
            result.TotalFiltered = filterTotal;
            result.Total = totalCount;

            //act
            var viewModel = new CourseProvidersViewModel(request, result, providerOrder);

            //assert
            viewModel.TotalMessage.Should().Be(expectedMessage);
        }

        [Test]
        [InlineAutoData(0, 5, false, "0 results")]
        [InlineAutoData(5, 0, true, "0 results")]
        public void Then_The_Total_Message_Is_Created_Correctly_For_Delivery_Modes_With_Location(
            int totalCount,
            int filterTotal,
            bool hasFilter,
            string expectedMessage,
            Dictionary<uint, string> providerOrder,
            GetCourseProvidersResult result)
        {
            //arrange
            var selectedDeliveryModes = new List<DeliveryModeType>();
            if (hasFilter)
            {
                selectedDeliveryModes.Add(DeliveryModeType.BlockRelease);
            }
            var request = new GetCourseProvidersRequest { DeliveryModes = selectedDeliveryModes };
            result.TotalFiltered = filterTotal;
            result.Total = totalCount;

            //act
            var viewModel = new CourseProvidersViewModel(request, result, providerOrder);

            //assert
            viewModel.TotalMessage.Should().Be($"{expectedMessage} at {viewModel.Location}");
        }

        [Test]
        [InlineAutoData(10, 5, false, "10 results")]
        [InlineAutoData(10, 5, true, "5 results")]
        [InlineAutoData(1, 5, false, "1 result")]
        [InlineAutoData(5, 1, true, "1 result")]
        public void Then_The_Total_Message_Is_Created_Correctly_For_Provider_Ratings(
            int totalCount,
            int filterTotal,
            bool hasFilter,
            string expectedMessage,
            Dictionary<uint, string> providerOrder,
            GetCourseProvidersResult result)
        {
            //arrange
            var selectedProviderRatings = new List<ProviderRating>();
            if (hasFilter)
            {
                selectedProviderRatings.Add(ProviderRating.Good);
            }
            var request = new GetCourseProvidersRequest { EmployerProviderRatings = selectedProviderRatings };
            result.TotalFiltered = filterTotal;
            result.Total = totalCount;

            //act
            var viewModel = new CourseProvidersViewModel(request, result, providerOrder);

            //assert
            viewModel.TotalMessage.Should().Be(expectedMessage);
        }

        [Test]
        [InlineAutoData(0, 5, false, "0 results")]
        [InlineAutoData(5, 0, true, "0 results")]
        public void Then_The_Total_Message_Is_Created_Correctly_For_Provider_Ratings_With_Location(
            int totalCount,
            int filterTotal,
            bool hasFilter,
            string expectedMessage,
            Dictionary<uint, string> providerOrder,
            GetCourseProvidersResult result)
        {
            //arrange
            var selectedProviderRatings = new List<ProviderRating>();
            if (hasFilter)
            {
                selectedProviderRatings.Add(ProviderRating.Good);
            }
            var request = new GetCourseProvidersRequest { EmployerProviderRatings = selectedProviderRatings };
            result.TotalFiltered = filterTotal;
            result.Total = totalCount;

            //act
            var viewModel = new CourseProvidersViewModel(request, result, providerOrder);

            //assert
            viewModel.TotalMessage.Should().Be($"{expectedMessage} at {viewModel.Location}");
        }

        [Test, AutoData]
        public void And_Zero_Results_And_Has_Location_And_No_Other_Filters_Then_Adds_Location_Text(
            Dictionary<uint, string> providerOrder,
            GetCourseProvidersRequest request,
            GetCourseProvidersResult result)
        {
            //arrange
            result.TotalFiltered = 0;
            result.Total = 0;

            //act
            var viewModel = new CourseProvidersViewModel(request, result, providerOrder);

            //assert
            viewModel.TotalMessage.Should().Be($"0 results at {viewModel.Location}");
        }
    }
}
