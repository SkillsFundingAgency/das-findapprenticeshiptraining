using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

namespace SFA.DAS.FAT.Web.Validators
{
    public class GetCourseQueryValidator : AbstractValidator<GetCourseQuery>
    {
        public const string CourseIdErrorMessage = "LarsCode must not be empty";

        public GetCourseQueryValidator()
        {
            RuleFor(s => s.LarsCode)
                .NotEmpty()
                .WithMessage(CourseIdErrorMessage);
        }
    }
}
