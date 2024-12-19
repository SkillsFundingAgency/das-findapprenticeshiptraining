using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourse;
using SFA.DAS.FAT.Application.Courses.Queries.GetProvider;
using SFA.DAS.FAT.Domain.Validation;
using GetCourseQueryValidator = SFA.DAS.FAT.Web.Validators.GetCourseQueryValidator;

namespace SFA.DAS.FAT.Web.AppStart
{
    public static class MediatRExtensions
    {
        public static void AddMediatRValidation(this IServiceCollection services)
        {
            services.AddScoped(typeof(IValidator<GetCourseQuery>), typeof(GetCourseQueryValidator));
            services.AddScoped(typeof(IValidator<GetCourseProviderQuery>), typeof(GetCourseProviderDetailsQueryValidator));
        }
    }
}
