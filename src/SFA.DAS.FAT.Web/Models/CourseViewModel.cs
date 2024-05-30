﻿using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Extensions;

namespace SFA.DAS.FAT.Web.Models
{
    public class CourseViewModel : IGetHelpFindingCourseViewModel
    {
        public int Id { get ; private set ; }
        public string Title { get ; set ; }
        public string TitleAndLevel { get; private set; }
        public string Sector { get; private set; }
        public string IntegratedDegree { get ; private set ; }
        public string OverviewOfRole { get ; private set ; }
        public List<string> CoreSkills { get ; private set ; }
        public List<string> TypicalJobTitles { get ; private set ; }
        public string ExternalCourseUrl { get ; private set ; }
        public int TypicalDuration { get ; private set ; }
        public int Level { get ; set; }
        public string LevelEquivalent { get; set; }
        public string MaximumFunding { get ; set ; }
        public int? TotalProvidersCount { get; set; }
        public int? ProvidersAtLocationCount { get; set; }
        public bool OtherBodyApprovalRequired { get; set; }
        public string ApprovalBody { get; set; }
        public DateTime? LastDateStarts { get; set; }
        public bool AfterLastStartDate { get; set; }
        public string LocationName { get; set; }
        public int ShortlistItemCount { get ; set ; }

        public bool CanGetHelpFindingCourse(FindApprenticeshipTrainingWeb config)
        {
            switch (config.EmployerDemandFeatureToggle)
            {
                case true:
                    return !string.IsNullOrEmpty(config.EmployerDemandUrl);

                case false:
                    return !string.IsNullOrEmpty(config.EmployerAccountsUrl) && !string.IsNullOrEmpty(config.RequestApprenticeshipTrainingUrl);
            }
        }

        public string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config)
        {
            return GetHelpFindingCourseUrl(config, EntryPoint.CourseDetail);
        }

        public string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config, EntryPoint entryPoint)
        {
            if (config.EmployerDemandFeatureToggle)
            {
                return $"{config.EmployerDemandUrl}/registerdemand/course/{Id}/share-interest?entrypoint={(short)entryPoint}";
            }

            return $"{config.EmployerAccountsUrl}/service/?redirectUri={config.RequestApprenticeshipTrainingUrl}/accounts/{{hashedAccountId}}/employer-requests/overview?standardId={Id}&requestType={entryPoint}&location={LocationName}";
        }

        public static implicit operator CourseViewModel(Course course)
        {
            return new CourseViewModel
            {
                Id = course.Id,
                Sector = course.Route,
                CoreSkills = course.CoreSkills ?? new List<string>(),
                Title = course.Title,
                TitleAndLevel = $"{course.Title} (level {course.Level})",
                Level = course.Level,
                LevelEquivalent = course.LevelEquivalent,
                IntegratedDegree = course.IntegratedDegree,
                ExternalCourseUrl = course.StandardPageUrl,
                OverviewOfRole = course.OverviewOfRole,
                TypicalJobTitles = course.TypicalJobTitles,
                TypicalDuration = course.TypicalDuration,
                MaximumFunding = course.MaxFunding.ToGdsCostFormat(),
                OtherBodyApprovalRequired = course.OtherBodyApprovalRequired,
                ApprovalBody = string.IsNullOrEmpty(course.ApprovalBody) ? null : course.ApprovalBody,
                LastDateStarts = course.StandardDates?.LastDateStarts,
                AfterLastStartDate = DateTime.UtcNow > course.StandardDates?.LastDateStarts,
            };
        }

        public bool HasLocation => !string.IsNullOrWhiteSpace(LocationName);
    }
}
