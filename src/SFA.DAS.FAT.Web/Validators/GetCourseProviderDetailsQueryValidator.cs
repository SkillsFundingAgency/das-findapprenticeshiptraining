using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;

namespace SFA.DAS.FAT.Web.Validators;

public class GetCourseProviderDetailsQueryValidator : AbstractValidator<GetCourseProviderDetailsQuery>
{
    public const string InvalidUkprnErrorMessage = "Invalid ukprn";
    public GetCourseProviderDetailsQueryValidator()
    {
        RuleFor(x => x.Ukprn)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(9999999).WithMessage(InvalidUkprnErrorMessage)
            .LessThan(20000000).WithMessage(InvalidUkprnErrorMessage);
    }
}
