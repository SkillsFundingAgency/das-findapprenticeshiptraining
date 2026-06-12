using System.Collections.Generic;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class CourseProvidersFiltersSubmitModel
{
    public string LarsCode { get; set; }
    public ProviderOrderBy OrderBy { get; set; } = ProviderOrderBy.Distance;
    public List<ProviderDeliveryMode> DeliveryModes { get; set; } = [];
    public List<ProviderRating> EmployerProviderRatings { get; set; } = [];
    public List<ProviderRating> ApprenticeProviderRatings { get; set; } = [];
    public List<QarRating> QarRatings { get; set; } = [];
    public int PageNumber { get; set; } = 1;
    public string Location { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;
}
