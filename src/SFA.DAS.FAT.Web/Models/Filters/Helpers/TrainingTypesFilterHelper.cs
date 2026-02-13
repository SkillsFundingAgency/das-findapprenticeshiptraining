using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models.Filters.Helpers;

public static class TrainingTypesFilterHelper
{
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION = "Short training courses based on existing apprenticeships, levels 2 to 7";
    public const string APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION = "Apprenticeships that qualify learners for a job, levels 2 to 7";
    public static List<FilterItemViewModel> BuildItems(List<string> selectedTrainingTypes)
    {
        var allTrainingTypes = new[]
        {
            TrainingType.ApprenticeshipUnit,
            TrainingType.FoundationApprenticeship,
            TrainingType.Apprenticeship
        };

        return allTrainingTypes
            .Select(trainingType => new FilterItemViewModel
            {
                Value = trainingType.GetDescription(),
                DisplayText = trainingType.GetDescription(),
                DisplayDescription = trainingType switch
                {
                    TrainingType.ApprenticeshipUnit => APPRENTICESHIP_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION,
                    TrainingType.FoundationApprenticeship => APPRENTICESHIP_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION,
                    _ => APPRENTICESHIP_TYPE_APPRENTICESHIP_DESCRIPTION
                },
                IsSelected = selectedTrainingTypes?.Contains(trainingType.GetDescription()) ?? false
            })
            .ToList();
    }
}
