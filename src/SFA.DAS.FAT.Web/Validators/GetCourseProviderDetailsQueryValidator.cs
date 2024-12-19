using System.Threading.Tasks;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Domain.Validation;

namespace SFA.DAS.FAT.Web.Validators
{
    public class GetCourseProviderDetailsQueryValidator : IValidator<GetCourseProviderQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetCourseProviderQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.ProviderId < 1)
            {
                validationResult.AddError(nameof(item.ProviderId));
            }

            return Task.FromResult(validationResult);
        }
    }
}
