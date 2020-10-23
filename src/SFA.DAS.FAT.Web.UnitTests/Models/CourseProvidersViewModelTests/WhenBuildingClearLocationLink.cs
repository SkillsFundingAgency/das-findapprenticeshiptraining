﻿using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests
{
    public class WhenBuildingClearLocationLink
    {
        [Test, AutoData]
        public void Then_The_Location_Is_Set_To_Minus_One_And_No_Delivery_Modes(CourseProvidersViewModel model)
        {
            var actual = model.ClearLocationLink;

            actual.Should().StartWith("?location=-1");
        }

        [Test, AutoData]
        public void Then_Any_Selected_Employer_Reviews_Are_Maintained(CourseProvidersViewModel model)
        {
            model.DeliveryModes = model.DeliveryModes.Select(c =>
            {
                c.Selected = false;
                return c;
            }).ToList();
            model.ProviderRatings = model.ProviderRatings.Select(c =>
            {
                c.Selected = true;
                return c;
            }).ToList();
            
            var actual = model.ClearLocationLink;
            actual.Should().StartWith($"?location=-1&providerRatings={string.Join("&providerRatings=", model.ProviderRatings.Select(c => c.ProviderRatingType))}");
        }
        
        [Test, AutoData]
        public void Then_Any_Selected_Delivery_Options_Are_Maintained(CourseProvidersViewModel model)
        {
            model.DeliveryModes = model.DeliveryModes.Select(c =>
            {
                c.Selected = true;
                return c;
            }).ToList();
            model.ProviderRatings = model.ProviderRatings.Select(c =>
            {
                c.Selected = false;
                return c;
            }).ToList();
            
            var actual = model.ClearLocationLink;
            actual.Should().StartWith($"?location=-1&deliveryModes={string.Join("&deliveryModes=", model.DeliveryModes.Select(c => c.DeliveryModeType))}");
        }
        
        [Test, AutoData]
        public void Then_Any_Selected_Delivery_Options_And_Ratings_Are_Maintained(CourseProvidersViewModel model)
        {
            model.DeliveryModes = model.DeliveryModes.Select(c =>
            {
                c.Selected = true;
                return c;
            }).ToList();
            model.ProviderRatings = model.ProviderRatings.Select(c =>
            {
                c.Selected = true;
                return c;
            }).ToList();
            
            var actual = model.ClearLocationLink;
            actual.Should().StartWith($"?location=-1&providerRatings={string.Join("&providerRatings=", model.ProviderRatings.Select(c => c.ProviderRatingType))}&deliveryModes={string.Join("&deliveryModes=", model.DeliveryModes.Select(c => c.DeliveryModeType))}");
            
        }
    }
}