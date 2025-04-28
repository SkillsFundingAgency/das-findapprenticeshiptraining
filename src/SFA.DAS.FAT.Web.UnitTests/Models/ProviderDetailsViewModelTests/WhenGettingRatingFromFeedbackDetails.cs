using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;
public class WhenGettingRatingFromFeedbackDetails
{
    [TestCase(0, ProviderRating.NotYetReviewed)]
    [TestCase(1, ProviderRating.VeryPoor)]
    [TestCase(2, ProviderRating.Poor)]
    [TestCase(3, ProviderRating.Good)]
    [TestCase(4, ProviderRating.Excellent)]
    [TestCase(5, ProviderRating.NotYetReviewed)]
    public void When_Setting_Stars_Then_Getting_Rating(int stars, ProviderRating expectedRating)
    {
        var feedbackDetails = new EmployerFeedBackDetails { Stars = stars };
        feedbackDetails.Rating.Should().Be(expectedRating.GetDescription());
    }

    [TestCase(0, "0 reviews")]
    [TestCase(1, "1 review")]
    [TestCase(2, "2 reviews")]
    [TestCase(101, "101 reviews")]
    public void When_Setting_Reviews_Then_Getting_Review_Message(int reviewCount, string expectedMessage)
    {
        var feedbackDetails = new EmployerFeedBackDetails { ReviewCount = reviewCount };
        feedbackDetails.ReviewMessage.Should().Be(expectedMessage);
    }
}
