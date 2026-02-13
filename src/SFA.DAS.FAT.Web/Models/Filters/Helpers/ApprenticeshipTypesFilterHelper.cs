using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models.Filters.Helpers;

public static class ApprenticeshipTypesFilterHelper
{
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION = "Short training courses based on existing apprenticeships, levels 2 to 7";
    public const string APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION = "Apprenticeships that qualify learners for a job, levels 2 to 7";
    public static List<FilterItemViewModel> BuildItems(List<string> selectedTrainingTypes, bool isBoldDisplayText = false)
    {
        var allTrainingTypes = new[]
        {
            ApprenticeshipType.ApprenticeshipUnit,
            ApprenticeshipType.FoundationApprenticeship,
            ApprenticeshipType.Apprenticeship
        };

        return allTrainingTypes
            .Select(trainingType => new FilterItemViewModel
            {
                Value = trainingType.GetDescription(),
                DisplayText = trainingType.GetDescription(),
                IsBoldDisplayText = isBoldDisplayText,
                DisplayDescription = trainingType switch
                {
                    ApprenticeshipType.ApprenticeshipUnit => APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION,
                    ApprenticeshipType.FoundationApprenticeship => APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION,
                    _ => APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION
                },
                IsSelected = selectedTrainingTypes?.Contains(trainingType.GetDescription()) ?? false
            })
            .ToList();
    }
}
