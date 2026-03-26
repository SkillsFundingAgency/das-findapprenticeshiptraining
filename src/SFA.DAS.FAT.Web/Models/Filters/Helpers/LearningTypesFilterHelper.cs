using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models.Filters.Helpers;

public static class LearningTypesFilterHelper
{
    public const string LEARNING_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION = "Short training courses based on existing apprenticeships, levels 2 to 7";
    public const string LEARNING_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION = "Introductory apprenticeships for young people, level 2";
    public const string LEARNING_TYPE_APPRENTICESHIP_DESCRIPTION = "Apprenticeships that qualify learners for a job, levels 2 to 7";
    public static List<FilterItemViewModel> BuildItems(List<string> selectedTrainingTypes, bool isLearningTypeEmphasised = false)
    {
        var allTrainingTypes = new[]
        {
            LearningType.ApprenticeshipUnit,
            LearningType.FoundationApprenticeship,
            LearningType.Apprenticeship
        };

        return allTrainingTypes
            .Select(trainingType => new FilterItemViewModel
            {
                Value = trainingType.ToString(),
                DisplayText = trainingType.GetDescription(),
                IsLearningTypeEmphasised = isLearningTypeEmphasised,
                DisplayDescription = trainingType switch
                {
                    LearningType.ApprenticeshipUnit => LEARNING_TYPE_APPRENTICESHIP_UNIT_DESCRIPTION,
                    LearningType.FoundationApprenticeship => LEARNING_TYPE_FOUNDATION_APPRENTICESHIP_DESCRIPTION,
                    _ => LEARNING_TYPE_APPRENTICESHIP_DESCRIPTION
                },
                IsSelected = selectedTrainingTypes.Contains(trainingType.ToString())
            })
            .ToList();
    }
}
