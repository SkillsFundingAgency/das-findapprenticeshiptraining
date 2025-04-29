using System;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ReviewsModel
{
    public string StartYear { get; set; } = string.Empty;

    public string EndYear { get; set; } = string.Empty;

    public string ApprenticeRating { get; set; } = string.Empty;
    public string EmployerRating { get; set; } = string.Empty;

    public int EmployerStarsValue { get; set; }

    public int ApprenticeStarsValue { get; set; }

    public int EmployerReviewCount { get; set; }

    public int ApprenticeReviewCount { get; set; }

    public string EmployerStarsMessage { get; set; } = string.Empty;
    public string ApprenticeStarsMessage { get; set; } = string.Empty;

    public static implicit operator ReviewsModel(GetProviderReviewsModel source)
    {
        if (source == null) return new ReviewsModel();


        var reviewsStartYear = $"20{source.ReviewPeriod.AsSpan(0, 2)}";
        var reviewsEndYear = $"20{source.ReviewPeriod.AsSpan(2, 2)}";
        var reviewEmployerStarsValue = int.TryParse(source.EmployerStars, out var employerStars) ? employerStars : 0;
        var reviewApprenticeStarsValue =
            int.TryParse(source.ApprenticeStars, out var apprenticeStars) ? apprenticeStars : 0;
        var employerReviewCount = int.TryParse(source.EmployerReviews, out var reviewCount) ? reviewCount : 0;
        var apprenticeReviewCount = int.TryParse(source.ApprenticeReviews, out var appReviewCount) ? appReviewCount : 0;


        var employerStarsMessage = employerReviewCount == 1
            ? $"average review from {employerReviewCount} employer when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'."
            : $"average review from {employerReviewCount} employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";

        var apprenticeStarsMessage = apprenticeReviewCount == 1
            ? $"average review from {apprenticeReviewCount} apprentice when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'."
            : $"average review from {apprenticeReviewCount} apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.";

        return new ReviewsModel
        {
            StartYear = reviewsStartYear,
            EndYear = reviewsEndYear,
            ApprenticeRating = source.ApprenticeRating,
            EmployerRating = source.EmployerRating,
            EmployerStarsValue = reviewEmployerStarsValue,
            ApprenticeStarsValue = reviewApprenticeStarsValue,
            EmployerStarsMessage = employerStarsMessage,
            EmployerReviewCount = employerReviewCount,
            ApprenticeStarsMessage = apprenticeStarsMessage,
            ApprenticeReviewCount = apprenticeReviewCount
        };
    }
}
