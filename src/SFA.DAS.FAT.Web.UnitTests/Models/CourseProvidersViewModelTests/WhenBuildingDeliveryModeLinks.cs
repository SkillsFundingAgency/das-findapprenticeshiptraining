namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    //MFCMFC
    // public class WhenBuildingDeliveryModeLinks
    // {
    //     [Test, AutoData]
    //     public void And_DeliveryMode_Selected_And_No_ProviderRatings_Selected_Then_Link_Returned(CourseProvidersViewModel model)
    //     {
    //         foreach (var deliveryMode in model.DeliveryModes)
    //         {
    //             deliveryMode.Selected = true;
    //         }
    //
    //         foreach (var employerProviderRating in model.EmployerProviderRatings)
    //         {
    //             employerProviderRating.Selected = false;
    //         }
    //
    //         foreach(var apprenticeProviderRating in model.ApprenticeProviderRatings)
    //         {
    //             apprenticeProviderRating.Selected = false;
    //         }
    //
    //         var links = model.ClearDeliveryModeLinks;
    //
    //         foreach (var deliveryMode in model.DeliveryModes.Where(viewModel => viewModel.Selected))
    //         {
    //             var link = links.Single(pair => pair.Key == deliveryMode.Description);
    //             var otherSelected = model.DeliveryModes
    //                 .Where(viewModel =>
    //                     viewModel.Selected &&
    //                     viewModel.DeliveryModeChoice != deliveryMode.DeliveryModeChoice)
    //                 .Select(viewModel => viewModel.DeliveryModeChoice);
    //
    //             link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", otherSelected)}");
    //         }
    //     }
    //
    //     [Test, AutoData]
    //     public void Then_When_National_And_At_Workplace_Are_Filtered_Then_Removing_At_Workplace_Removes_National(CourseProvidersViewModel model)
    //     {
    //         model.DeliveryModes = new List<DeliveryModeOptionViewModel>
    //         {
    //             new DeliveryModeOptionViewModel
    //             {
    //                 Selected = true,
    //                 Description = DeliveryModeType.National.GetDescription(),
    //                 DeliveryModeChoice = DeliveryModeType.National
    //             },
    //             new DeliveryModeOptionViewModel
    //             {
    //                 Selected = true,
    //                 Description = DeliveryModeType.Workplace.GetDescription(),
    //                 DeliveryModeChoice = DeliveryModeType.Workplace
    //             }
    //         };
    //         
    //         foreach (var employerProviderRating in model.EmployerProviderRatings)
    //         {
    //             employerProviderRating.Selected = false;
    //         }
    //
    //         foreach (var apprenticeProviderRating in model.ApprenticeProviderRatings)
    //         {
    //             apprenticeProviderRating.Selected = false;
    //         }
    //
    //         var links = model.ClearDeliveryModeLinks;
    //
    //         foreach (var deliveryMode in model.DeliveryModes.Where(viewModel => viewModel.Selected))
    //         {
    //             var link = links.Single(pair => pair.Key == deliveryMode.Description);
    //             var otherSelected = model.DeliveryModes
    //                 .Where(viewModel =>
    //                     viewModel.Selected &&
    //                     viewModel.DeliveryModeChoice != deliveryMode.DeliveryModeChoice)
    //                 .Select(viewModel => viewModel.DeliveryModeChoice);
    //
    //             if (link.Key == DeliveryModeType.Workplace.GetDescription())
    //             {
    //                 link.Value.Should().Be($"?location={model.Location}&deliveryModes=");
    //             }
    //             else
    //             {
    //                 link.Value.Should().Be($"?location={model.Location}&deliveryModes={string.Join("&deliveryModes=", otherSelected)}");    
    //             }
    //             
    //         }
    //     }
    //
    //     [Test, AutoData]
    //     public void And_DeliveryMode_Not_Selected_Then_Empty_List_Returned(CourseProvidersViewModel model)
    //     {
    //         foreach (var deliveryMode in model.DeliveryModes)
    //         {
    //             deliveryMode.Selected = false;
    //         }
    //
    //         var links = model.ClearDeliveryModeLinks;
    //
    //         links.Should().BeEmpty();
    //     }
    // }
}
