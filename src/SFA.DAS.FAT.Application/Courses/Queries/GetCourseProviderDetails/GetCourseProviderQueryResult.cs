using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

public class GetCourseProviderQueryResult : CourseProviderDetailsBase
{
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
            CourseType = source.CourseType,
            ApprenticeshipType = source.ApprenticeshipType,
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
