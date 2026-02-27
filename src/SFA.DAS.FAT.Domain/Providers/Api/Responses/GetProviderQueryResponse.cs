using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Providers.Api.Responses;

public class GetProviderQueryResponse
{
    public int Ukprn { get; set; }

    public string ProviderName { get; set; }
    public GetProviderAddressModel ProviderAddress { get; set; }
    public GetProviderContactDetails Contact { get; set; }

    public GetProviderQarModel Qar { get; set; }
    public GetProviderReviewsModel Reviews { get; set; }

    public List<GetProviderCourseDetails> Courses { get; set; }

    public GetProviderEndpointAssessmentsDetails EndpointAssessments { get; set; }

    public List<EmployerFeedbackAnnualSummaries> AnnualEmployerFeedbackDetails { get; set; }

    public List<ApprenticeFeedbackAnnualSummaries> AnnualApprenticeFeedbackDetails { get; set; }
}

public sealed class GetProviderReviewsModel
{
    public string ReviewPeriod { get; set; }
    public string EmployerReviews { get; set; }
    public string EmployerStars { get; set; }
    public string EmployerRating { get; set; }
    public string ApprenticeReviews { get; set; }
    public string ApprenticeStars { get; set; }
    public string ApprenticeRating { get; set; }
}

public class GetProviderAddressModel
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string Town { get; set; }
    public string Postcode { get; set; }
}

public class GetProviderQarModel
{
    public string Period { get; set; }

    public string Leavers { get; set; }
    public string AchievementRate { get; set; }
    public string NationalAchievementRate { get; set; }

}

public class GetProviderCourseDetails
{
    public string CourseName { get; set; }
    public ApprenticeshipType ApprenticeshipType { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string IfateReferenceNumber { get; set; }

}

public class GetProviderContactDetails
{
    public string MarketingInfo { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
}

public class GetProviderEndpointAssessmentsDetails
{
    public DateTime? EarliestAssessment { get; set; }
    public int EndpointAssessmentCount { get; set; }
}

public class ApprenticeFeedbackAnnualSummaries
{
    public long Ukprn { get; set; }
    public int Stars { get; set; }
    public int ReviewCount { get; set; }
    public string TimePeriod { get; set; }

    public List<AttributeResultModel> ProviderAttribute { get; set; }
}

public class EmployerFeedbackAnnualSummaries
{
    public long Ukprn { get; set; }
    public int Stars { get; set; }
    public int ReviewCount { get; set; }
    public string TimePeriod { get; set; }


    public List<AnnualSummaryItem> ProviderAttribute { get; set; }
}

public class AnnualSummaryItem
{
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Weakness { get; set; }
}
