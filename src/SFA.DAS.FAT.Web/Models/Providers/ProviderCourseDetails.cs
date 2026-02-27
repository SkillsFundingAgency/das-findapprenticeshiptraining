using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCourseDetails
{
    public string CourseName { get; init; }
    public ApprenticeshipType ApprenticeshipType { get; init; }
    public int Level { get; init; }

    public string LarsCode { get; init; }

    public static implicit operator ProviderCourseDetails(GetProviderCourseDetails source)
    {
        return new ProviderCourseDetails
        {
            CourseName = source.CourseName,
            ApprenticeshipType = source.ApprenticeshipType,
            Level = source.Level,
            LarsCode = source.LarsCode
        };
    }
}
