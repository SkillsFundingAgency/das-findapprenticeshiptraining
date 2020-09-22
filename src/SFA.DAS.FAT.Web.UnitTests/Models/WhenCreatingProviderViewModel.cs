﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using DeliveryModeType = SFA.DAS.FAT.Web.Models.DeliveryModeType;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingProviderViewModel
    {
        [Test, AutoData]
        public void Then_Maps_The_Fields(Provider source)
        {
            var actual = (ProviderViewModel) source;
            
            actual.Should().BeEquivalentTo(source, options => options
                .Excluding(c=>c.OverallAchievementRate)
                .Excluding(c=>c.NationalOverallAchievementRate)
                .Excluding(c=>c.NationalOverallCohort)
                .Excluding(c => c.DeliveryModes)
                .Excluding(c=>c.Feedback)
            );

            actual.OverallAchievementRatePercentage.Should().Be($"{(Math.Round(source.OverallAchievementRate.Value)/100):0%}");
            actual.NationalOverallAchievementRatePercentage.Should().Be($"{(Math.Round(source.NationalOverallAchievementRate.Value)/100):0%}");
            actual.TotalEmployerResponses.Should().Be(source.Feedback.TotalEmployerResponses);
            actual.TotalFeedbackRating.Should().Be(source.Feedback.TotalFeedbackRating);
        }

        //Feedback
        [Test]
        [InlineAutoData(50, "(50 employer reviews)")]
        [InlineAutoData(51, "(50+ employer reviews)")]
        [InlineAutoData(1, "(1 employer review)")]
        [InlineAutoData(0, "Not yet reviewed (employer reviews)")]
        public void Then_The_Feedback_Text_Is_Formatted_Correctly(int numberOfReviews, string expectedText, Provider source)
        {
            source.Feedback.TotalEmployerResponses = numberOfReviews;
            var actual = (ProviderViewModel) source;    

            actual.TotalFeedbackRatingText.Should().Be(expectedText);
        }


        // achievement rate
        [Test, AutoData]
        public void Then_No_Delivery_Modes_Has_An_Empty_List(Provider source)
        {
            source.DeliveryModes = null;
            
            var actual = (ProviderViewModel) source;
            
            actual.DeliveryModes.Should().BeEmpty();
        }
        
        [Test, AutoData]
        public void Then_No_Achievement_Data_Shows_Empty_String(Provider source)
        {
            source.OverallAchievementRate = null;
            source.NationalOverallAchievementRate = null;
            
            var actual = (ProviderViewModel) source;

            actual.OverallAchievementRatePercentage.Should().BeEmpty();
            actual.NationalOverallAchievementRatePercentage.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_Rounds_Values_For_Achievement_Data(Provider source)
        {
            source.OverallAchievementRate = 38.9m;
            source.NationalOverallAchievementRate = 78.9m;
            
            var actual = (ProviderViewModel) source;
            
            actual.OverallAchievementRatePercentage.Should().Be("39%");
            actual.NationalOverallAchievementRatePercentage.Should().Be("79%");
        }

        // delivery modes

        [Test, AutoData]
        public void Then_Maps_Fields_From_Source(DeliveryMode source)
        {
            var actual = new DeliveryModeViewModel().Map(source,DeliveryModeType.BlockRelease);
            
            actual.Should().BeEquivalentTo(source, options=>options
                .Excluding(c=>c.DeliveryModeType)
                .Excluding(c=>c.DistanceInMiles)
            );
            actual.AddressFormatted.Should()
                .Be($"{source.Address1}, {source.Address2}, {source.Town}, {source.County}, {source.Postcode}");
        }

        [Test]
        [InlineAutoData("Address1","Address2","Town","County","Postcode","Address1, Address2, Town, County, Postcode")]
        [InlineAutoData("Address1","","Town","County","Postcode","Address1, Town, County, Postcode")]
        [InlineAutoData("Address1","","","County","Postcode","Address1, County, Postcode")]
        [InlineAutoData("","","","County","Postcode","County, Postcode")]
        [InlineAutoData("","","","County","","County")]
        public void Then_Builds_Address_Correctly(string address1, string address2, string town,string county, string postcode, string expected, DeliveryMode source)
        {
            source.Address1 = address1;
            source.Address2 = address2;
            source.County = county;
            source.Postcode = postcode;
            source.Town = town;
            
            var actual = new DeliveryModeViewModel().Map(source,DeliveryModeType.BlockRelease);

            actual.AddressFormatted.Should().Be(expected);
        }

        [Test, AutoData]
        public void Then_Has_Empty_List_Returned_If_No_DeliveryModes(Provider source)
        {
            source.DeliveryModes = new List<DeliveryMode>();

            var actual = (ProviderViewModel) source;

            actual.DeliveryModes.Should().BeEmpty();
        }
        
        [Test, AutoData]
        public void Then_Has_3_DeliveryModes_In_Correct_Order(Provider source)
        {
            var actual = (ProviderViewModel) source;

            var modes = actual.DeliveryModes.ToList();
            modes.Count.Should().Be(3);
            modes[0].DeliveryModeType.Should().Be(DeliveryModeType.Workplace);
            modes[1].DeliveryModeType.Should().Be(DeliveryModeType.DayRelease);
            modes[2].DeliveryModeType.Should().Be(DeliveryModeType.BlockRelease);
        }

        [Test, AutoData]
        public void And_No_Workplace_Delivery_Then_Blank_DeliveryMode(Provider source,
            decimal distanceInMiles)
        {
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.BlockRelease,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var workplaceDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.Workplace);
            workplaceDeliveryMode.FormattedDistanceInMiles.Should().BeNullOrEmpty();
            workplaceDeliveryMode.IsAvailable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_Has_Workplace_Delivery_Then_Formatted_DeliveryMode_With_No_Distance(
            Provider source,
            decimal distanceInMiles)
        {
            distanceInMiles += 0.324m;
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.Workplace,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var workplaceDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.Workplace);
            workplaceDeliveryMode.FormattedDistanceInMiles.Should().BeNullOrEmpty();
            workplaceDeliveryMode.IsAvailable.Should().BeTrue();
        }

        [Test, AutoData]
        public void And_No_DayRelease_Delivery_Then_Blank_DeliveryMode(Provider source,
            decimal distanceInMiles)
        {
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.BlockRelease,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var dayReleaseDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.DayRelease);
            dayReleaseDeliveryMode.FormattedDistanceInMiles.Should().BeNullOrEmpty();
            dayReleaseDeliveryMode.IsAvailable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_Has_DayRelease_Delivery_Then_Formatted_DeliveryMode(
            Provider source,
            decimal distanceInMiles)
        {
            distanceInMiles += 0.324m;
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.DayRelease,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var dayReleaseDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.DayRelease);
            dayReleaseDeliveryMode.FormattedDistanceInMiles.Should().Be($"({distanceInMiles:##.#} miles away)");
            dayReleaseDeliveryMode.IsAvailable.Should().BeTrue();
        }

        [Test, AutoData]
        public void And_No_BlockRelease_Delivery_Then_Blank_DeliveryMode(Provider source,
            decimal distanceInMiles)
        {
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.DayRelease,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var blockReleaseDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.BlockRelease);
            blockReleaseDeliveryMode.FormattedDistanceInMiles.Should().BeNullOrEmpty();
            blockReleaseDeliveryMode.IsAvailable.Should().BeFalse();
        }

        [Test, AutoData]
        public void And_Has_BlockRelease_Delivery_Then_Formatted_DeliveryMode(
            Provider source,
            decimal distanceInMiles)
        {
            distanceInMiles += 0.324m;
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.BlockRelease,
                    DistanceInMiles = distanceInMiles
                }
            };

            var actual = (ProviderViewModel)source;

            var blockReleaseDeliveryMode = actual.DeliveryModes.Single(model =>
                model.DeliveryModeType == DeliveryModeType.BlockRelease);
            blockReleaseDeliveryMode.FormattedDistanceInMiles.Should().Be($"({distanceInMiles:##.#} miles away)");
            blockReleaseDeliveryMode.IsAvailable.Should().BeTrue();
        }
    }
}
