namespace SFA.DAS.FAT.Web.Models.FeedbackSurvey;

public class FeedbackByYear
{
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string Heading { get; set; }

    public string SubHeading { get; set; }
    public bool IsMostRecentYear { get; set; }
    public string MainText { get; set; }
    public string TimePeriod { get; set; }

    public EmployerFeedBackDetails EmployerFeedbackDetails { get; set; }
    public ApprenticeFeedBackDetails ApprenticeFeedbackDetails { get; set; }

    public bool ShowEmployerFeedbackStars => EmployerFeedbackDetails is { Stars: > 0 };
    public bool ShowApprenticeFeedbackStars => ApprenticeFeedbackDetails is { Stars: > 0 };

    public string NoEmployerReviewsText { get; set; }
    public string NoApprenticeReviewsText { get; set; }
}
