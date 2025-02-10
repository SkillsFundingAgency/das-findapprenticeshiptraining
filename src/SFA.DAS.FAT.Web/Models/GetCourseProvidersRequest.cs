using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class GetCourseProvidersRequest
    {
        public int Id { get; set; }

        [FromQuery]
        public ProviderOrderBy? OrderBy { get; set; }

        [FromQuery]
        public string Location { get; set; }
        [FromQuery]
        public IReadOnlyList<ProviderDeliveryMode> DeliveryModes { get; set; } = new List<ProviderDeliveryMode>();
        [FromQuery]
        public IReadOnlyList<ProviderRating> EmployerProviderRatings { get; set; } = new List<ProviderRating>();
        [FromQuery]
        public IReadOnlyList<ProviderRating> ApprenticeProviderRatings { get; set; } = new List<ProviderRating>();

        [FromQuery]
        public IReadOnlyList<QarRating> QarRatings { get; set; } = new List<QarRating>();

        [FromQuery]
        public int? Distance { get; set; }

        [FromQuery]
        public int? Page { get; set; }

        [FromQuery]
        public int? PageSize { get; set; }



        // [FromQuery]
        // public string Removed { get; set; }
        // [FromQuery]
        // public string Added { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>
            {
                {nameof(Id), Id.ToString()},
                {nameof(Location), Location}
            };

            for (var i = 0; i < DeliveryModes.Count; i++)
            {
                var deliveryModeType = DeliveryModes[i];
                result.Add($"{nameof(DeliveryModes)}[{i}]", deliveryModeType.ToString());
            }

            for (var i = 0; i < EmployerProviderRatings.Count; i++)
            {
                var employerProviderRating = EmployerProviderRatings[i];
                result.Add($"{nameof(EmployerProviderRatings)}[{i}]", employerProviderRating.ToString());
            }

            for (var i = 0; i < ApprenticeProviderRatings.Count; i++)
            {
                var apprenticeProviderRating = ApprenticeProviderRatings[i];
                result.Add($"{nameof(ApprenticeProviderRatings)}[{i}]", apprenticeProviderRating.ToString());
            }

            return result;
        }
    }
}
