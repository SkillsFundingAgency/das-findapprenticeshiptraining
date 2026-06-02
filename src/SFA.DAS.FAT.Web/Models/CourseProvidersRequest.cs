using System.Collections.Generic;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProvidersRequest
{
    public string LarsCode { get; set; }

    public ProviderOrderBy OrderBy { get; set; } = ProviderOrderBy.Distance;

    public IReadOnlyList<ProviderDeliveryMode> DeliveryModes { get; set; } = [];
    public IReadOnlyList<ProviderRating> EmployerProviderRatings { get; set; } = [];
    public IReadOnlyList<ProviderRating> ApprenticeProviderRatings { get; set; } = [];
    public IReadOnlyList<QarRating> QarRatings { get; set; } = [];

    public int PageNumber { get; set; } = 1;

    // Location and Distance are posted via forms or persisted to cookies/session.
    public string Location { get; set; } = string.Empty;

    public string Distance { get; set; } = string.Empty;
}
