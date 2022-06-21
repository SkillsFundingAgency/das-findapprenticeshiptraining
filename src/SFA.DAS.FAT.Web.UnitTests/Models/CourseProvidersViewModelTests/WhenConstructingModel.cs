using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenConstructingModel
    {
        [Test, AutoData]
        public void Then_Sets_Properties(GetCourseProvidersRequest request, GetCourseProvidersResult result, Dictionary<uint, string> providerOrder)
        {
            var model = new CourseProvidersViewModel(request, result, providerOrder);

            model.Course.Should().BeEquivalentTo((CourseViewModel)result.Course);
            model.Providers.Should().BeEquivalentTo(
                result.Providers.Select(provider => (ProviderViewModel)provider));
            model.Total.Should().Be(result.Total);
            model.TotalFiltered.Should().Be(result.TotalFiltered);
            model.Location.Should().Be(result.Location);
            model.ProviderOrder.Should().BeEquivalentTo(providerOrder);
            model.ShortlistItemCount.Should().Be(result.ShortlistItemCount);
        }

        [Test, AutoData]
        public void Then_Builds_Delivery_Modes_Excluding_NotFound(GetCourseProvidersRequest request, GetCourseProvidersResult result, Dictionary<uint, string> providerOrder)
        {
            var expectedDeliveryModes = new List<DeliveryModeOptionViewModel>();
            foreach (DeliveryModeType deliveryModeType in Enum.GetValues(typeof(DeliveryModeType)))
            {
                expectedDeliveryModes.Add(new DeliveryModeOptionViewModel
                {
                    DeliveryModeType = deliveryModeType,
                    Description = deliveryModeType.GetDescription(),
                    Selected = request.DeliveryModes.Any(type => type == deliveryModeType)
                });
            }

            var model = new CourseProvidersViewModel(request, result, providerOrder);

            model.DeliveryModes.Should().BeEquivalentTo(expectedDeliveryModes.Where(c=>c.DeliveryModeType!=DeliveryModeType.NotFound));
        }

        [Test, AutoData]
        public void Then_If_National_Delivery_Mode_Is_Selected_Then_AtWorkplace_Is_Selected(GetCourseProvidersRequest request, GetCourseProvidersResult result, Dictionary<uint, string> providerOrder)
        {
            request.DeliveryModes = new List<DeliveryModeType> {DeliveryModeType.National};
            
            var model = new CourseProvidersViewModel(request, result, providerOrder);

            model.DeliveryModes.Count(c => c.Selected).Should().Be(2);
            model.DeliveryModes.Where(c => c.Selected).Select(c => c.DeliveryModeType).ToList().Should()
                .BeEquivalentTo(new List<DeliveryModeType> {DeliveryModeType.Workplace, DeliveryModeType.National});
        }
        
        [Test, AutoData]
        public void Then_Builds_Provider_Ratings(GetCourseProvidersRequest request, GetCourseProvidersResult result, Dictionary<uint, string> providerOrder)
        {
            var expectedProviderRatings = new List<EmployerProviderRatingOptionViewModel>();
            foreach (ProviderRating providerRatingType in Enum.GetValues(typeof(ProviderRating)))
            {
                expectedProviderRatings.Add(new EmployerProviderRatingOptionViewModel
                {
                    ProviderRatingType = providerRatingType,
                    Description = providerRatingType.GetDescription(),
                    Selected = request.EmployerProviderRatings.Any(type => type == providerRatingType)
                });
            }

            var model = new CourseProvidersViewModel(request, result, providerOrder);

            model.EmployerProviderRatings.Should().BeEquivalentTo(expectedProviderRatings);
        }
    }
}
