﻿using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;
using SFA.DAS.FAT.Web.Models.FeedbackSurvey;

namespace SFA.DAS.FAT.Web.Models.Providers;

public class ProviderDetailsViewModel : PageLinksViewModelBase
{


    public int Ukprn { get; set; }
    public string ProviderName { get; set; }

    public string ProviderAddress { get; set; }
    public ContactDetailsModel Contact { get; set; }
    public ProviderCoursesModel ProviderCoursesDetails { get; set; }

    public ProviderQarModel Qar { get; set; }

    public EndpointAssessmentModel EndpointAssessments { get; set; }

    public ReviewsModel Reviews { get; set; }

    public bool ShowCourses => ProviderCoursesDetails.CourseCount > 0;

    public bool EmployerReviewed => Reviews.EmployerRating != ProviderRating.NotYetReviewed.ToString();
    public bool ApprenticeReviewed => Reviews.ApprenticeRating != ProviderRating.NotYetReviewed.ToString();


    public FeedbackSurveyViewModel FeedbackSurvey { get; set; }

    public static implicit operator ProviderDetailsViewModel(GetProviderQueryResponse source)
    {
        var vm = new ProviderDetailsViewModel
        {
            Ukprn = source.Ukprn,
            ProviderName = source.ProviderName,
            ProviderAddress = ((GetProviderAddress)source.ProviderAddress).GetComposedAddress(source.ProviderName),
            Contact = source.Contact,
            ProviderCoursesDetails = source.Courses,
            Qar = source.Qar,
            Reviews = source.Reviews,
            EndpointAssessments = source.EndpointAssessments,
            ShowSearchCrumb = true,
            ShowShortListLink = true
        };

        if (vm.ProviderCoursesDetails is { Courses: { } })
        {
            foreach (var course in vm.ProviderCoursesDetails.Courses)
            {
                var routeData = new Dictionary<string, string>
                {
                    { "id", course.LarsCode.ToString() },
                    { "providerId", vm.Ukprn.ToString() }
                };

                course.RouteData = routeData;
            }
        }

        return vm;
    }


}
