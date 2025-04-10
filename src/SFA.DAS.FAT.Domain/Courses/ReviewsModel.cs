namespace SFA.DAS.FAT.Domain.Courses;

public sealed class ReviewsModel
{
    public string ReviewPeriod { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public ProviderRating EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public ProviderRating ApprenticeRating { get; set; }
    public string AcademicPeriod => FormatAcademicPeriod();

    public int ConvertedEmployerStars
    {
        get
        {
            if (int.TryParse(EmployerStars, out int value))
            {
                return value;
            }

            return 0;
        }
    }

    public int ConvertedApprenticeStars
    {
        get
        {
            if (int.TryParse(ApprenticeStars, out int value))
            {
                return value;
            }

            return 0;
        }
    }
    private string FormatAcademicPeriod()
    {
        if (string.IsNullOrWhiteSpace(ReviewPeriod) || ReviewPeriod.Length != 4 || !int.TryParse(ReviewPeriod, out _))
        {
            return ReviewPeriod;
        }

        var startYear = int.Parse("20" + ReviewPeriod.Substring(0, 2));
        var endYear = int.Parse("20" + ReviewPeriod.Substring(2, 2));

        return $"{startYear} to {endYear}";
    }
}
