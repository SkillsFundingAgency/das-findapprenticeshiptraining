using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProvidersRequest
{
    [FromRoute]
    public string LarsCode { get; set; }

    [FromQuery]
    public ProviderOrderBy OrderBy { get; set; } = ProviderOrderBy.Distance;

    [FromQuery]
    public string Location { get; set; } = string.Empty;

    [FromQuery]
    public IReadOnlyList<ProviderDeliveryMode> DeliveryModes { get; set; } = new List<ProviderDeliveryMode>();
    [FromQuery]
    public IReadOnlyList<ProviderRating> EmployerProviderRatings { get; set; } = new List<ProviderRating>();
    [FromQuery]
    public IReadOnlyList<ProviderRating> ApprenticeProviderRatings { get; set; } = new List<ProviderRating>();

    [FromQuery]
    public IReadOnlyList<QarRating> QarRatings { get; set; } = new List<QarRating>();

    [FromQuery]
    public string Distance { get; set; } = string.Empty;

    [FromQuery]
    public int PageNumber { get; set; } = 1;
}
