using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models.Shared;

public class ProviderReviewsViewModel
{
    public ReviewsModel Reviews { get; set; }
    public string EmployerReviewsDisplayMessage { get; set; }
    public string ApprenticeReviewsDisplayMessage { get; set; }

    public bool EmployerReviewed => (Reviews ?? new ReviewsModel()).EmployerRating != ProviderRating.NotYetReviewed;
    public int EmployerStarsValue => (Reviews ?? new ReviewsModel()).ConvertedEmployerStars;
    public string EmployerRating => (Reviews ?? new ReviewsModel()).EmployerRating.ToString();
    public string EmployerReviewsMessage => EmployerReviewsDisplayMessage ?? string.Empty;

    public bool ApprenticeReviewed => (Reviews ?? new ReviewsModel()).ApprenticeRating != ProviderRating.NotYetReviewed;
    public int ApprenticeStarsValue => (Reviews ?? new ReviewsModel()).ConvertedApprenticeStars;
    public string ApprenticeRating => (Reviews ?? new ReviewsModel()).ApprenticeRating.ToString();
    public string ApprenticeReviewsMessage => ApprenticeReviewsDisplayMessage ?? string.Empty;
}
