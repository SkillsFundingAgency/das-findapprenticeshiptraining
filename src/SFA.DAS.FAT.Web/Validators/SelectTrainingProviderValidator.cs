using FluentValidation;
using SFA.DAS.FAT.Web.Models;

namespace SFA.DAS.FAT.Web.Validators;

public class SelectTrainingProviderValidator : AbstractValidator<SelectTrainingProviderSubmitViewModel>
{
    public const string NoTrainingProviderSelectedErrorMessage = "Type a name or UKPRN and select a provider";

    public SelectTrainingProviderValidator()
    {
        RuleFor(s => s.SearchTerm)
            .NotEmpty()
            .WithMessage(NoTrainingProviderSelectedErrorMessage);
    }
}
