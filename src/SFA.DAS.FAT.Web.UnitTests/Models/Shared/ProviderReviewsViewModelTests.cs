using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.Shared;

namespace SFA.DAS.FAT.Web.UnitTests.Models.Shared;

public class ProviderReviewsViewModelTests
{
    [Test]
    public void EmployerReviewed_ReviewsIsNull_ReturnsFalseAndDefaultValues()
    {
        var sut = new ProviderReviewsViewModel();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EmployerReviewed, Is.True);
            Assert.That(sut.EmployerStarsValue, Is.Zero);
            Assert.That(sut.EmployerRating, Is.EqualTo(ProviderRating.Excellent.ToString()));
            Assert.That(sut.EmployerReviewsMessage, Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void ApprenticeReviewed_ReviewsIsNull_ReturnsFalseAndDefaultValues()
    {
        var sut = new ProviderReviewsViewModel();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.ApprenticeReviewed, Is.True);
            Assert.That(sut.ApprenticeStarsValue, Is.Zero);
            Assert.That(sut.ApprenticeRating, Is.EqualTo(ProviderRating.Excellent.ToString()));
            Assert.That(sut.ApprenticeReviewsMessage, Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void EmployerAndApprenticeProperties_ReviewsSet_ReturnsMappedValues()
    {
        var sut = new ProviderReviewsViewModel
        {
            Reviews = new ReviewsModel
            {
                EmployerRating = ProviderRating.Good,
                ApprenticeRating = ProviderRating.Excellent,
                EmployerStars = "3",
                ApprenticeStars = "4"
            },
            EmployerReviewsDisplayMessage = "employer message",
            ApprenticeReviewsDisplayMessage = "apprentice message"
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EmployerReviewed, Is.True);
            Assert.That(sut.ApprenticeReviewed, Is.True);
            Assert.That(sut.EmployerStarsValue, Is.EqualTo(3));
            Assert.That(sut.ApprenticeStarsValue, Is.EqualTo(4));
            Assert.That(sut.EmployerRating, Is.EqualTo(ProviderRating.Good.ToString()));
            Assert.That(sut.ApprenticeRating, Is.EqualTo(ProviderRating.Excellent.ToString()));
            Assert.That(sut.EmployerReviewsMessage, Is.EqualTo("employer message"));
            Assert.That(sut.ApprenticeReviewsMessage, Is.EqualTo("apprentice message"));
        }
    }
}
