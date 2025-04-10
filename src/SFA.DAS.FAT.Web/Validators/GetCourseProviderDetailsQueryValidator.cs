using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

namespace SFA.DAS.FAT.Web.Validators;

public class GetCourseProviderDetailsQueryValidator : AbstractValidator<GetCourseProviderDetailsQuery>
{
    public const string ProviderIdErrorMessage = "ProviderId must be greater than zero";
    public GetCourseProviderDetailsQueryValidator()
    {
        RuleFor(s => s.Ukprn)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithMessage(ProviderIdErrorMessage);
    }
}
