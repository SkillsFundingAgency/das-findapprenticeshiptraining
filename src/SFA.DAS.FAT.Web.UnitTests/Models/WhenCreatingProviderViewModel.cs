using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Models;
using DeliveryModeType = SFA.DAS.FAT.Web.Models.DeliveryModeType;
using EmployerProviderRating = SFA.DAS.FAT.Web.Models.ProviderRating;

namespace SFA.DAS.FAT.Web.UnitTests.Models
{
    public class WhenCreatingProviderViewModel
    {
        [Test, AutoData]
        public void Then_Maps_The_Fields(Provider source)
        {
            var actual = (ProviderViewModel)source;

            actual.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.OverallAchievementRate)
                .Excluding(c => c.NationalOverallAchievementRate)
                .Excluding(c => c.NationalOverallCohort)
                .Excluding(c => c.DeliveryModes)
                .Excluding(c => c.EmployerFeedback)
                .Excluding(c => c.ApprenticeFeedback)
                .Excluding(c => c.ProviderAddress)
            );

            actual.OverallAchievementRatePercentage.Should().Be($"{(Math.Round(source.OverallAchievementRate.Value) / 100):0%}");
            actual.NationalOverallAchievementRatePercentage.Should().Be($"{(Math.Round(source.NationalOverallAchievementRate.Value) / 100):0%}");
            actual.EmployerFeedback.TotalFeedbackResponses.Should().Be(source.EmployerFeedback.TotalEmployerResponses);
            actual.EmployerFeedback.TotalFeedbackRating.Should().Be(source.EmployerFeedback.TotalFeedbackRating);
            actual.EmployerFeedback.FeedbackAttributeSummary.Select(c => c.StrengthCount).Should().BeEquivalentTo(source.EmployerFeedback.FeedbackAttributes.Select(x => x.Strength));
            actual.EmployerFeedback.FeedbackAttributeSummary.Select(c => c.WeaknessCount).Should().BeEquivalentTo(source.EmployerFeedback.FeedbackAttributes.Select(x => x.Weakness));
            actual.ProviderDistance.Should().Be(source.ProviderAddress.DistanceInMiles.FormatDistance());
        }

        [Test]
        public void Then_If_Source_Is_Null_Then_Null_Returned()
        {
            var actual = (ProviderViewModel)null;

            actual.Should().BeNull();
        }


        [Test]
        public void Then_If_Source_Is_Empty_Then_Null_Returned()
        {
            var actual = (ProviderViewModel)new Provider();

            actual.Should().BeNull();
        }

        [Test, AutoData]
        public void Then_Return_Null_When_Trading_Name_Matches_Name(Provider source)
        {
            // arrange
            source.TradingName = "university of suffolk";
            source.Name = "UNIVERSITY OF SUFFOLK";

            // act
            var actual = (ProviderViewModel)source;

            // assert
            actual.TradingName.Should().BeNull();
        }

        [Test, AutoData]
        public void Then_Return_Null_When_Trading_Name_Is_Null(Provider source)
        {
            // arrange
            source.TradingName = null;

            // act
            var actual = (ProviderViewModel)source;

            // assert
            actual.TradingName.Should().BeNull();
        }

        //Feedback
        [Test]
        [InlineAutoData(50, "(50 employer reviews)")]
        [InlineAutoData(51, "(50+ employer reviews)")]
        [InlineAutoData(1, "(1 employer review)")]
        [InlineAutoData(0, "Not yet reviewed (employer reviews)")]
        public void Then_The_Feedback_Text_Is_Formatted_Correctly(int numberOfReviews, string expectedText, Provider source)
        {
            source.EmployerFeedback.TotalEmployerResponses = numberOfReviews;
            var actual = (ProviderViewModel)source;

            actual.EmployerFeedback.TotalFeedbackRatingText.Should().Be(expectedText);
        }
        [Test]
        [InlineAutoData(50, "(50 reviews)")]
        [InlineAutoData(51, "(51 reviews)")]
        [InlineAutoData(1, "(1 review)")]
        [InlineAutoData(0, "Not yet reviewed")]
        public void Then_The_Feedback_Provider_Detail_Text_Is_Formatted_Correctly(int numberOfReviews, string expectedText, Provider source)
        {
            source.EmployerFeedback.TotalEmployerResponses = numberOfReviews;
            var actual = (ProviderViewModel)source;

            actual.EmployerFeedback.TotalFeedbackRatingTextProviderDetail.Should().Be(expectedText);
        }

        [Test]
        [InlineAutoData(1, "Very poor")]
        [InlineAutoData(2, "Poor")]
        [InlineAutoData(3, "Good")]
        [InlineAutoData(4, "Excellent")]
        public void Then_The_Feedback_Rating_Is_Mapped_To_The_Description(int feedbackRating, string expected, Provider source)
        {
            source.EmployerFeedback.TotalFeedbackRating = feedbackRating;

            var actual = (ProviderViewModel)source;

            actual.EmployerFeedback.TotalFeedbackText.GetDescription().Should().Be(expected);
        }

        [Test, AutoData]
        public void Then_The_Feedback_Detail_Exists_For_Each_Rating_Type(Provider source)
        {

            var actual = (ProviderViewModel)source;

            actual.EmployerFeedback.FeedbackDetail.Count.Should().Be(4);
            actual.EmployerFeedback.FeedbackDetail.Select(c => c.Rating).Contains(EmployerProviderRating.Excellent).Should().BeTrue();
            actual.EmployerFeedback.FeedbackDetail.Select(c => c.Rating).Contains(EmployerProviderRating.Good).Should().BeTrue();
            actual.EmployerFeedback.FeedbackDetail.Select(c => c.Rating).Contains(EmployerProviderRating.Poor).Should().BeTrue();
            actual.EmployerFeedback.FeedbackDetail.Select(c => c.Rating).Contains(EmployerProviderRating.VeryPoor).Should().BeTrue();
        }

        [Test]
        [InlineAutoData(50, 50, 100.0, "50 reviews")]
        [InlineAutoData(51, 60, 85.0, "51 reviews")]
        [InlineAutoData(1, 11, 9.1, "1 review")]
        [InlineAutoData(0, 0, 0.0, "0 reviews")]
        public void Then_The_Text_Is_Generated_For_Number_Of_Reviews_With_Percentage(int numberOfReviews, int totalReviews, double expectedPercentage, string expectedText, Provider source)
        {
            source.EmployerFeedback.TotalEmployerResponses = totalReviews;
            source.EmployerFeedback.FeedbackDetail.FirstOrDefault().FeedbackCount = numberOfReviews;
            source.EmployerFeedback.FeedbackDetail.FirstOrDefault().FeedbackName = "Good";

            var actual = (ProviderViewModel)source;

            var actualFeedbackDetail = actual.EmployerFeedback.FeedbackDetail.FirstOrDefault(c => c.Rating.Equals(EmployerProviderRating.Good));
            Assert.IsNotNull(actualFeedbackDetail);
            actualFeedbackDetail.RatingText.Should().Be(expectedText);
            actualFeedbackDetail.RatingCount.Should().Be(numberOfReviews);
            actualFeedbackDetail.RatingPercentage.Should().Be((decimal)expectedPercentage);
        }

        // achievement rate
        [Test, AutoData]
        public void Then_No_Delivery_Modes_Has_An_Empty_List(Provider source)
        {
            source.DeliveryModes = null;

            var actual = (ProviderViewModel)source;

            actual.DeliveryModes.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_No_Achievement_Data_Shows_Empty_String(Provider source)
        {
            source.OverallAchievementRate = null;
            source.NationalOverallAchievementRate = null;

            var actual = (ProviderViewModel)source;

            actual.OverallAchievementRatePercentage.Should().BeEmpty();
            actual.NationalOverallAchievementRatePercentage.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_Rounds_Values_For_Achievement_Data(Provider source)
        {
            source.OverallAchievementRate = 38.9m;
            source.NationalOverallAchievementRate = 78.9m;

            var actual = (ProviderViewModel)source;

            actual.OverallAchievementRatePercentage.Should().Be("39%");
            actual.NationalOverallAchievementRatePercentage.Should().Be("79%");
        }

        // delivery modes

        [Test, AutoData]
        public void Then_Maps_Fields_From_Source(DeliveryMode source)
        {
            var actual = new DeliveryModeViewModel().Map(source, DeliveryModeType.BlockRelease);

            actual.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.DeliveryModeType)
                .Excluding(c => c.DistanceInMiles)
            );
            actual.AddressFormatted.Should()
                .Be($"{source.Address1}, {source.Address2}, {source.Town}, {source.County}, {source.Postcode}");
        }

        [Test]
        [InlineAutoData("Address1", "Address2", "Town", "County", "Postcode", "Address1, Address2, Town, County, Postcode")]
        [InlineAutoData("Address1", "", "Town", "County", "Postcode", "Address1, Town, County, Postcode")]
        [InlineAutoData("Address1", "", "", "County", "Postcode", "Address1, County, Postcode")]
        [InlineAutoData("", "", "", "County", "Postcode", "County, Postcode")]
        [InlineAutoData("", "", "", "County", "", "County")]
        public void Then_Builds_Address_Correctly(string address1, string address2, string town, string county, string postcode, string expected, DeliveryMode source)
        {
            source.Address1 = address1;
            source.Address2 = address2;
            source.County = county;
            source.Postcode = postcode;
            source.Town = town;

            var actual = new DeliveryModeViewModel().Map(source, DeliveryModeType.BlockRelease);

            actual.AddressFormatted.Should().Be(expected);
        }

        [Test, AutoData]
        public void Then_Has_Empty_List_Returned_If_No_DeliveryModes(Provider source)
        {
            source.DeliveryModes = new List<DeliveryMode>();

            var actual = (ProviderViewModel)source;

            actual.DeliveryModes.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_Has_3_DeliveryModes_In_Correct_Order(Provider source)
        {
            var actual = (ProviderViewModel)source;

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
            dayReleaseDeliveryMode.FormattedDistanceInMiles.Should().Be($": {distanceInMiles.FormatDistance()} miles away");
            dayReleaseDeliveryMode.IsAvailable.Should().BeTrue();
        }

        [Test, AutoData]
        public void And_Has_DayRelease_Delivery_Then_Formatted_DeliveryMode_With_Trailing_Zeros_Removed(
            Provider source)
        {
            var distanceInMiles = 1.0m;
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
            dayReleaseDeliveryMode.FormattedDistanceInMiles.Should().Be(": 1 mile away");
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
            blockReleaseDeliveryMode.FormattedDistanceInMiles.Should().Be($": {distanceInMiles:##.#} miles away");
            blockReleaseDeliveryMode.IsAvailable.Should().BeTrue();
        }

        [Test, AutoData]
        public void And_Has_NotFound_Then_NotFound_And_All_Others_Added_As_Unavailable_To_Delivery_Mode(Provider source)
        {
            // Arrange
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.NotFound
                }
            };

            // Act
            var actual = (ProviderViewModel)source;

            // Assert
            actual.DeliveryModes.Count().Should().Be(4);
            actual.DeliveryModes.SingleOrDefault(c => c.DeliveryModeType == DeliveryModeType.NotFound && c.IsAvailable).Should().NotBeNull();
            actual.DeliveryModes.SingleOrDefault(c => c.DeliveryModeType == DeliveryModeType.Workplace && !c.IsAvailable).Should().NotBeNull();
            actual.DeliveryModes.SingleOrDefault(c => c.DeliveryModeType == DeliveryModeType.DayRelease && !c.IsAvailable).Should().NotBeNull();
            actual.DeliveryModes.SingleOrDefault(c => c.DeliveryModeType == DeliveryModeType.BlockRelease && !c.IsAvailable).Should().NotBeNull();

        }

        [Test, AutoData]
        public void Then_Only_Three_Delivery_Modes_Are_Added_If_There_Is_No_NotFound(Provider source,
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
            actual.DeliveryModes.Count().Should().Be(3);
            actual.DeliveryModes.ToList().TrueForAll(x => x.DeliveryModeType == DeliveryModeType.NotFound).Should().BeFalse();
        }

        [Test, AutoData]
        public void Then_Adds_National_Text_If_DeliveryMode_Is_Workplace_And_Flag_Set(Provider source)
        {
            // Arrange
            source.DeliveryModes = new List<DeliveryMode>
            {
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.Workplace,
                    National = true
                },
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.DayRelease,
                    National = false
                },
                new DeliveryMode
                {
                    DeliveryModeType = Domain.Courses.DeliveryModeType.BlockRelease,
                    National = true
                }

            };

            // Act
            var actual = (ProviderViewModel)source;

            // Assert
            actual.DeliveryModes.Count(x => x.NationalText == "(national)").Should().Be(1);
        }

        [Test]
        [InlineAutoData(-1.0, "Head office")]
        [InlineAutoData(1.0, "Head office 1 mile away")]
        [InlineAutoData(10.5, "Head office 10.5 miles away")]
        public void Then_Builds_Head_Office_Label(double distance, string expectedText, Provider source)
        {
            //Arrange
            source.ProviderAddress.DistanceInMiles = (decimal)distance;

            // Act
            var actual = (ProviderViewModel)source;

            //Assert
            actual.ProviderDistanceText.Should().Be(expectedText);
        }

        [Test, AutoData]
        public void Then_If_Null_Distance_Returns_Correct_Head_Office_Label(Provider source)
        {
            //Arrange
            source.ProviderAddress.DistanceInMiles = null;

            // Act
            var actual = (ProviderViewModel)source;

            //Assert
            actual.ProviderDistanceText.Should().Be("Head office");
        }

        [Test]
        [InlineAutoData("Address 1", "Address 2", "Address 3", "Address 4", "Town", "Postcode", "Address 1, Address 2, Address 3, Address 4, Town, Postcode")]
        [InlineAutoData("Address 1", "", "Address 3", "Address 4", "Town", "Postcode", "Address 1, Address 3, Address 4, Town, Postcode")]
        [InlineAutoData("Address 1", "Address 2", null, "Address 4", "Town", "Postcode", "Address 1, Address 2, Address 4, Town, Postcode")]
        [InlineAutoData("Address 1", "", "", "", "", "Postcode", "Address 1, Postcode")]
        [InlineAutoData(null, null, "", "Address 4", "Town", "Postcode", "Address 4, Town, Postcode")]
        [InlineAutoData(null, null, "", "", "", "", "")]
        [InlineAutoData(null, null, null, null, null, null, "")]
        [InlineAutoData("", "", "", "", "", "", "")]
        public void Then_Builds_The_Head_Office_Address(string address1, string address2, string address3, string address4, string town, string postcode, string expected, Provider source)
        {
            //Arrange
            source.ProviderAddress = new ProviderAddress
            {
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Address4 = address4,
                Town = town,
                Postcode = postcode
            };

            // Act
            var actual = (ProviderViewModel)source;

            //Assert
            actual.ProviderAddress.Should().Be(expected);
        }

        [Test, AutoData]
        public void Then_No_Address_Data_Gives_Defaults(Provider source)
        {
            //Arrange
            source.ProviderAddress = null;

            //Act
            var actual = (ProviderViewModel)source;

            //Assert
            actual.ProviderAddress.Should().BeEmpty();
            actual.ProviderDistanceText.Should().BeEmpty();
            actual.ProviderDistance.Should().BeEmpty();
        }
    }
}
