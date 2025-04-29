using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderCourseDetails
{
    public string CourseName { get; init; }
    public int Level { get; init; }

    public int LarsCode { get; init; }

    public Dictionary<string, string> RouteData { get; set; } = new();

    public static implicit operator ProviderCourseDetails(GetProviderCourseDetails source)
    {
        return new ProviderCourseDetails
        {
            CourseName = source.CourseName,
            Level = source.Level,
            LarsCode = source.LarsCode
        };
    }
}
