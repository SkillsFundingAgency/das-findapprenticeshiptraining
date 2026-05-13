using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
namespace SFA.DAS.FAT.Web.Models.Filters.Helpers;

public static class LearningTypesFilterHelper
{
    public const string ApprenticeshipUnitDescription = "Short training courses based on employer skills needs, levels 2 to 5";
    public const string FoundationApprenticeshipDescription = "Introductory apprenticeships, level 2";
    public const string ApprenticeshipDescription = "Qualifications from levels 2 to 7";
    public static List<FilterItemViewModel> BuildItems(List<LearningType> selectedTrainingTypes, bool isLearningTypeEmphasised = false)
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
                    LearningType.ApprenticeshipUnit => ApprenticeshipUnitDescription,
                    LearningType.FoundationApprenticeship => FoundationApprenticeshipDescription,
                    _ => ApprenticeshipDescription
                },
                IsSelected = selectedTrainingTypes.Contains(trainingType)
            })
            .ToList();
    }
}
