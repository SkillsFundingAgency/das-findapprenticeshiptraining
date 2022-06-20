using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenBuildingDeliveryModeLinks
    {
        [Test, AutoData]
        public void And_DeliveryMode_Selected_And_No_ProviderRatings_Selected_Then_Link_Returned(CourseProvidersViewModel model)
        {
            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = true;
            }

            foreach (var providerRating in model.EmployerProviderRatings)
            {
                providerRating.Selected = false;
            }

            var links = model.ClearDeliveryModeLinks;

            foreach (var deliveryMode in model.DeliveryModes.Where(viewModel => viewModel.Selected))
            {
                var link = links.Single(pair => pair.Key == deliveryMode.Description);
                var otherSelected = model.DeliveryModes
                    .Where(viewModel =>
                        viewModel.Selected &&
                        viewModel.DeliveryModeType != deliveryMode.DeliveryModeType)
                    .Select(viewModel => viewModel.DeliveryModeType);

                link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", otherSelected)}");
            }
        }

        [Test, AutoData]
        public void Then_When_National_And_At_Workplace_Are_Filtered_Then_Removing_At_Workplace_Removes_National(CourseProvidersViewModel model)
        {
            model.DeliveryModes = new List<DeliveryModeOptionViewModel>
            {
                new DeliveryModeOptionViewModel
                {
                    Selected = true,
                    Description = DeliveryModeType.National.GetDescription(),
                    DeliveryModeType = DeliveryModeType.National
                },
                new DeliveryModeOptionViewModel
                {
                    Selected = true,
                    Description = DeliveryModeType.Workplace.GetDescription(),
                    DeliveryModeType = DeliveryModeType.Workplace
                }
            };
            
            foreach (var providerRating in model.EmployerProviderRatings)
            {
                providerRating.Selected = false;
            }

            
            var links = model.ClearDeliveryModeLinks;
    
            foreach (var deliveryMode in model.DeliveryModes.Where(viewModel => viewModel.Selected))
            {
                var link = links.Single(pair => pair.Key == deliveryMode.Description);
                var otherSelected = model.DeliveryModes
                    .Where(viewModel =>
                        viewModel.Selected &&
                        viewModel.DeliveryModeType != deliveryMode.DeliveryModeType)
                    .Select(viewModel => viewModel.DeliveryModeType);

                if (link.Key == DeliveryModeType.Workplace.GetDescription())
                {
                    link.Value.Should().Be($"?location={model.Location}&deliveryModes=");
                }
                else
                {
                    link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", otherSelected)}");    
                }
                
            }
        }

        [Test, AutoData]
        public void And_DeliveryMode_Not_Selected_Then_Empty_List_Returned(CourseProvidersViewModel model)
        {
            foreach (var deliveryMode in model.DeliveryModes)
            {
                deliveryMode.Selected = false;
            }

            var links = model.ClearDeliveryModeLinks;

            links.Should().BeEmpty();
        }
    }
}
