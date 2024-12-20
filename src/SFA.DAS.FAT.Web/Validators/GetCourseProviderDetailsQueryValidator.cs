using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;

namespace SFA.DAS.FAT.Web.Validators
{
    public class GetCourseProviderDetailsQueryValidator : AbstractValidator<GetCourseProviderQuery>
    {
        public const string ProviderIdErrorMessage = "ProviderId must be greater than zero";
        public GetCourseProviderDetailsQueryValidator()
        {
            RuleFor(s => s.ProviderId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage(ProviderIdErrorMessage);
        }
    }
}
