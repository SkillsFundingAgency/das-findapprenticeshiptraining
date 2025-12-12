using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Domain.Courses;

public sealed class CourseProviderDetailsModel
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
}
