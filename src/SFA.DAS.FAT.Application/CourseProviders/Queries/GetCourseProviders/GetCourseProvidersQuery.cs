using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.CourseProviders.Queries.GetCourseProviders
{
    public class GetCourseProvidersQuery : IRequest<GetCourseProvidersResult>
    {
        public int Id { get; set; }
        public ProviderOrderBy? OrderBy { get; set; }
        public int? Distance { get; set; }
        public string Location { get; set; }
        public List<ProviderDeliveryMode> DeliveryModes { get; set; }

        public List<ProviderRating> EmployerProviderRatings { get; set; }

        public List<ProviderRating> ApprenticeProviderRatings { get; set; }
        public List<QarRating> Qar { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public Guid? ShortlistUserId { get; set; }
    }
}
