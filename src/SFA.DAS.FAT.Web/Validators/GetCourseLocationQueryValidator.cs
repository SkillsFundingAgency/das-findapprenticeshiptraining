using FluentValidation;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Web.Validators;

public class GetCourseLocationQueryValidator : AbstractValidator<GetCourseLocationQuery>
{
    public const string LocationErrorMessage = "Enter the first 3 letters of a city or postcode and select a location.";

    public GetCourseLocationQueryValidator(ILocationService locationService)
    {
        RuleFor(s => s.Location)
            .Cascade(CascadeMode.Stop)
            .MustAsync(async (location, cancellation) =>
            {
                if (string.IsNullOrEmpty(location?.Trim())) return true;

                return await locationService.IsLocationValid(location);
            })
            .WithMessage(LocationErrorMessage);

    }
}
