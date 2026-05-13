using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Providers;
using ProviderReviewsModel = SFA.DAS.FAT.Web.Models.Providers.ReviewsModel;
using ReviewsModel = SFA.DAS.FAT.Domain.Courses.ReviewsModel;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;

public class ProviderDetailsViewModelSharedSectionsTests
{
    [Test]
    public void AchievementsAndParticipation_QarAndEndpointAssessmentsAreNull_ReturnsDefaultValues()
    {
        var sut = new ProviderDetailsViewModel
        {
            Qar = null,
            EndpointAssessments = null
        };

        var result = sut.AchievementsAndParticipation;

        Assert.Multiple(() =>
        {
            Assert.That(result.Qar.AchievementRate, Is.Null);
            Assert.That(result.Qar.NationalAchievementRate, Is.Null);
            Assert.That(result.Qar.Leavers, Is.EqualTo("0"));
            Assert.That(result.AchievementRateInformation, Is.EqualTo("of apprentices (0 of 0) completed a course and passed the end-point assessment with this training provider in academic year  to . % did not pass or left the course before taking their assessment."));
            Assert.That(result.EndpointAssessmentsCountDisplay, Is.EqualTo(string.Empty));
            Assert.That(result.EndpointAssessmentDisplayMessage, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    public void ProviderReviews_ReviewRatingsCannotBeParsed_ReturnsNotYetReviewedRatings()
    {
        var sut = new ProviderDetailsViewModel
        {
            Reviews = new ProviderReviewsModel
            {
                EmployerRating = "Unknown",
                ApprenticeRating = "AnotherUnknown",
                EmployerStarsValue = 2,
                ApprenticeStarsValue = 3,
                EmployerStarsMessage = "employer",
                ApprenticeStarsMessage = "apprentice"
            }
        };

        var result = sut.ProviderReviews;

        Assert.Multiple(() =>
        {
            Assert.That(result.Reviews.EmployerRating, Is.EqualTo(ProviderRating.NotYetReviewed));
            Assert.That(result.Reviews.ApprenticeRating, Is.EqualTo(ProviderRating.NotYetReviewed));
            Assert.That(result.EmployerReviewsDisplayMessage, Is.EqualTo("employer"));
            Assert.That(result.ApprenticeReviewsDisplayMessage, Is.EqualTo("apprentice"));
            Assert.That(result.Reviews.EmployerStars, Is.EqualTo("2"));
            Assert.That(result.Reviews.ApprenticeStars, Is.EqualTo("3"));
        });
    }

    [Test]
    public void ProviderReviews_ReviewsIsNull_ReturnsDefaultReviewValues()
    {
        var sut = new ProviderDetailsViewModel
        {
            Reviews = null
        };

        var result = sut.ProviderReviews;

        Assert.Multiple(() =>
        {
            Assert.That(result.Reviews.EmployerRating, Is.EqualTo(ProviderRating.NotYetReviewed));
            Assert.That(result.Reviews.ApprenticeRating, Is.EqualTo(ProviderRating.NotYetReviewed));
            Assert.That(result.Reviews.EmployerStars, Is.EqualTo("0"));
            Assert.That(result.Reviews.ApprenticeStars, Is.EqualTo("0"));
            Assert.That(result.EmployerReviewsDisplayMessage, Is.EqualTo(string.Empty));
            Assert.That(result.ApprenticeReviewsDisplayMessage, Is.EqualTo(string.Empty));
        });
    }
}
