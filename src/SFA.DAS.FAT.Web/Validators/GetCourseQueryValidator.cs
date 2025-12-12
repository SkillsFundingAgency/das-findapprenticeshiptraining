using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

namespace SFA.DAS.FAT.Web.Validators
{
    public class GetCourseQueryValidator : AbstractValidator<GetCourseQuery>
    {
        public const string CourseIdErrorMessage = "CourseId must be greater than zero";

        public GetCourseQueryValidator()
        {
            RuleFor(s => int.Parse(s.LarsCode))
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage(CourseIdErrorMessage);
        }
    }
}
