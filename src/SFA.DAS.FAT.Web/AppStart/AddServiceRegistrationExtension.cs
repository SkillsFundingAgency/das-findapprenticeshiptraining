﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.FAT.Application.Courses.Services;
using SFA.DAS.FAT.Application.Locations.Services;
using SFA.DAS.FAT.Application.Shortlist.Services;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Infrastructure.Api;
using SFA.DAS.FAT.Infrastructure.Services;

namespace SFA.DAS.FAT.Web.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpClient<IApiClient, ApiClient>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IShortlistService, ShortlistService>();
            services.AddHttpContextAccessor();
            services.AddSingleton(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));
        }
    }
}
