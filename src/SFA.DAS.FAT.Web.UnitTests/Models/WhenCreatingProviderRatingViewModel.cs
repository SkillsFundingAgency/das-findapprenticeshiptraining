using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.UnitTests.Models;
public class WhenCreatingProviderRatingViewModel
{
    [TestCase("None", "-", ProviderRating.VeryPoor, ProviderRatingType.Apprentice, "0 apprentice reviews", 0, "poor")]
    [TestCase("None", "-", ProviderRating.VeryPoor, ProviderRatingType.Employer, "0 employer reviews", 0, "poor")]
    [TestCase("1", "1", ProviderRating.VeryPoor, ProviderRatingType.Apprentice, "1 apprentice review", 1, "poor")]
    [TestCase("1", "1", ProviderRating.VeryPoor, ProviderRatingType.Employer, "1 employer review", 1, "poor")]
    [TestCase("2", "2", ProviderRating.Poor, ProviderRatingType.Apprentice, "2 apprentice reviews", 2, "poor")]
    [TestCase("2", "2", ProviderRating.Poor, ProviderRatingType.Employer, "2 employer reviews", 2, "poor")]
    [TestCase("3", "3", ProviderRating.Good, ProviderRatingType.Apprentice, "3 apprentice reviews", 3, "good")]
    [TestCase("3", "3", ProviderRating.Good, ProviderRatingType.Employer, "3 employer reviews", 3, "good")]
    [TestCase("4", "5", ProviderRating.Excellent, ProviderRatingType.Apprentice, "5 apprentice reviews", 4, "good")]
    [TestCase("4", "5", ProviderRating.Excellent, ProviderRatingType.Employer, "5 employer reviews", 4, "good")]
    public void Then_The_Fields_Are_Correctly_Mapped(string stars, string reviews, ProviderRating providerRating, ProviderRatingType providerRatingType, string totalMessage, int starsValue, string ratingGroup)
    {
        var sut = new ProviderRatingViewModel { Stars = stars, Reviews = reviews, ProviderRating = providerRating, ProviderRatingType = providerRatingType };

        sut.TotalMessage.Should().Be(totalMessage);
        sut.StarsValue.Should().Be(starsValue);
        sut.RatingGroup.Should().Be(ratingGroup);
    }
}
