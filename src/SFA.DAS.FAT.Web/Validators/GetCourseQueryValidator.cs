using System.Threading.Tasks;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Validation;

namespace SFA.DAS.FAT.Web.Validators
{
    public class GetCourseQueryValidator : IValidator<GetCourseQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetCourseQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.CourseId < 1)
            {
                validationResult.AddError(nameof(item.CourseId));
            }

            return Task.FromResult(validationResult);
        }
    }
}
