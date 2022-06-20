using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenBuildingProviderRatingLinks
    {
        [Test, AutoData]
        public void And_ProviderRating_Selected_Then_Link_Returned_With_No_DeliveryModes(CourseProvidersViewModel model)
        {
            // Arrange
            foreach (var providerRating in model.EmployerProviderRatings )
            {
                providerRating.Selected = true;
            }

            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = false;
            }

            // Act
            var links = model.ClearEmployerProviderRatingLinks;

            // Assert
            foreach (var providerRating in model.EmployerProviderRatings.Where(vm => vm.Selected))
            {
                var link = links.Single(pair => pair.Key == providerRating.Description);
                var selectedProviderRatings = model.EmployerProviderRatings
                    .Where(vm =>
                        vm.Selected &&
                        vm.ProviderRatingType != providerRating.ProviderRatingType)
                    .Select(vm => vm.ProviderRatingType);

                link.Value.Should().Be($"?location={model.Location}&providerRatings={string.Join("&providerRatings=", selectedProviderRatings)}");
            }
        }

        [Test, AutoData]
        public void And_ProviderRating_Selected_Then_Link_Returned_With_DeliveryModes_Selected(CourseProvidersViewModel model)
        {
            // Arrange
            foreach (var providerRating in model.EmployerProviderRatings)
            {
                providerRating.Selected = true;
            }

            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = true;
            }

            // Act
            var links = model.ClearEmployerProviderRatingLinks;

            // Assert
            foreach (var providerRating in model.EmployerProviderRatings.Where(vm => vm.Selected))
            {
                var link = links.Single(pair => pair.Key == providerRating.Description);
                var selectedProviderRatings = model.EmployerProviderRatings
                    .Where(vm =>
                        vm.Selected &&
                        vm.ProviderRatingType != providerRating.ProviderRatingType)
                    .Select(vm => vm.ProviderRatingType);
                var deliveryModeSelected = model.DeliveryModes
                    .Where(vm => vm.Selected)
                    .Select(vm => vm.DeliveryModeType);

                link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", deliveryModeSelected)}&providerRatings={string.Join("&providerRatings=", selectedProviderRatings)}");
            }
        }

        [Test, AutoData]
        public void And_ProviderType_Not_Selected_Then_Empty_List_Returned(CourseProvidersViewModel model)
        {
            foreach (var providerRating in model.EmployerProviderRatings)
            {
                providerRating.Selected = false;
            }

            var links = model.ClearEmployerProviderRatingLinks;

            links.Should().BeEmpty();
        }
    }
}
