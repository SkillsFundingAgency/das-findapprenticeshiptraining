using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

public class GetCourseProviderQueryResult
{
    public long Ukprn { get; set; }
    public string ProviderName { get; set; }
    public ShortProviderAddressModel ProviderAddress { get; set; }
    public ContactModel Contact { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string IFateReferenceNumber { get; set; }
    public QarModel Qar { get; set; }
    public ReviewsModel Reviews { get; set; }
    public EndpointAssessmentModel EndpointAssessments { get; set; }
    public int TotalProvidersCount { get; set; }
    public Guid? ShortlistId { get; set; }
    public IEnumerable<LocationModel> Locations { get; set; }
    public IEnumerable<ProviderCourseModel> Courses { get; set; } = [];
    public List<EmployerFeedbackAnnualSummaries> AnnualEmployerFeedbackDetails { get; set; } = [];
    public List<ApprenticeFeedbackAnnualSummaries> AnnualApprenticeFeedbackDetails { get; set; } = [];

    public static implicit operator GetCourseProviderQueryResult(CourseProviderDetailsModel source)
    {
        if (source is null)
        {
            return null;
        }

        return new GetCourseProviderQueryResult
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = source.ProviderAddress,
            Contact = source.Contact,
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = source.LarsCode,
            IFateReferenceNumber = source.IFateReferenceNumber,
            Qar = source.Qar,
            Reviews = source.Reviews,
            EndpointAssessments = source.EndpointAssessments,
            TotalProvidersCount = source.TotalProvidersCount,
            ShortlistId = source.ShortlistId,
            Locations = source.Locations,
            Courses = source.Courses,
            AnnualEmployerFeedbackDetails = source.AnnualEmployerFeedbackDetails,
            AnnualApprenticeFeedbackDetails = source.AnnualApprenticeFeedbackDetails
        };
    }
}
