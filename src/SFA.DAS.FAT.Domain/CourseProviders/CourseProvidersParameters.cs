using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.CourseProviders;

public class CourseProvidersParameters
{
    public string LarsCode { get; set; }
    public ProviderOrderBy OrderBy { get; set; }
    public int? Distance { get; set; }
    public string Location { get; set; }
    public List<ProviderDeliveryMode> DeliveryModeTypes { get; set; } = [];

    public List<ProviderRating> EmployerProviderRatingTypes { get; set; } = [];
    public List<ProviderRating> ApprenticeProviderRatingTypes { get; set; } = [];

    public List<QarRating> QarRatings { get; set; } = [];
    public int Page { get; set; } = 1;
    public int? PageSize { get; set; } = Constants.DefaultPageSize;
    public Guid? ShortlistUserId { get; set; }
}
