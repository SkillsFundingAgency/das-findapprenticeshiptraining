using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenBuildingApprenticeProviderRatingLinks
    {
        [Test, AutoData]
        public void And_ApprenticeProviderRating_Selected_Then_Link_Returned_With_No_DeliveryModes_And_No_EmployerRatings(CourseProvidersViewModel model)
        {
            // Arrange
            foreach (var employerProviderRating in model.EmployerProviderRatings)
            {
                employerProviderRating.Selected = false;
            }

            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = false;
            }

            foreach (var apprenticeProviderRating in model.ApprenticeProviderRatings)
            {
                apprenticeProviderRating.Selected = true;
            }

            // Act
            var links = model.ClearApprenticeProviderRatingLinks;

            // Assert
            foreach (var providerRating in model.ApprenticeProviderRatings.Where(vm => vm.Selected))
            {
                var link = links.Single(pair => pair.Key == providerRating.Description);
                var selectedProviderRatings = model.ApprenticeProviderRatings
                    .Where(vm =>
                        vm.Selected &&
                        vm.ProviderRatingType != providerRating.ProviderRatingType)
                    .Select(vm => vm.ProviderRatingType);

                link.Value.Should().Be($"?location={model.Location}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", selectedProviderRatings)}");
            }
        }

        [Test, AutoData]
        public void And_ApprenticeProviderRating_Selected_Then_Link_Returned_With_DeliveryModes_Selected_And_Employer_Ratings(CourseProvidersViewModel model)
        {
            // Arrange
            foreach (var employerProviderRating in model.EmployerProviderRatings)
            {
                employerProviderRating.Selected = true;
            }

            foreach (var apprenticeProviderRating in model.ApprenticeProviderRatings)
            {
                apprenticeProviderRating.Selected = true;
            }

            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = true;
            }

            // Act
            var links = model.ClearApprenticeProviderRatingLinks;

            // Assert
            foreach (var providerRating in model.ApprenticeProviderRatings.Where(vm => vm.Selected))
            {
                var link = links.Single(pair => pair.Key == providerRating.Description);
                var selectedProviderRatings = model.EmployerProviderRatings
                    .Where(vm => vm.Selected)
                    .Select(vm => vm.ProviderRatingType);
                var deliveryModeSelected = model.DeliveryModes
                    .Where(vm => vm.Selected)
                    .Select(vm => vm.DeliveryModeChoice);
                var apprenticeProviderRatings = model.ApprenticeProviderRatings
                    .Where(vm => vm.Selected && 
                    vm.ProviderRatingType != providerRating.ProviderRatingType)
                    .Select(vm => vm.ProviderRatingType);


                link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", deliveryModeSelected)}&employerProviderRatings={string.Join("&employerProviderRatings=", selectedProviderRatings)}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatings)}");
            }
        }

        [Test, AutoData]
        public void And_ApprenticeProviderType_Not_Selected_Then_Empty_List_Returned(CourseProvidersViewModel model)
        {
            foreach (var providerRating in model.ApprenticeProviderRatings)
            {
                providerRating.Selected = false;
            }

            var links = model.ClearApprenticeProviderRatingLinks;

            links.Should().BeEmpty();
        }
    }
}
