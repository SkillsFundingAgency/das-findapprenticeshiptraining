using System;
using System.Collections.Generic;
using System.Globalization;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;
using SFA.DAS.FAT.Web.Extensions;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.Models;

public class StandardViewModel
{
    public int Ordering { get; init; }
    public string StandardUId { get; init; }
    public string IfateReferenceNumber { get; init; }
    public string LarsCode { get; init; }
    public float? SearchScore { get; init; }
    public int ProvidersCount { get; init; }
    public int TotalProvidersCount { get; init; }
    public string Title { get; init; }
    public int Level { get; init; }
    public string LevelName { get; init; }
    public string OverviewOfRole { get; init; }
    public string Keywords { get; init; }
    public string Route { get; init; }
    public int RouteCode { get; init; }
    public string MaxFunding { get; init; }
    public int TypicalDuration { get; init; }
    public CourseType CourseType { get; init; }
    public LearningType LearningType { get; init; }
    public string RequestApprenticeshipTrainingUrl { get; init; }
    public string FindProvidersUrl { get; init; }
    public string FindProvidersUrlDescription { get; init; }
    public string LearningTypeTagClass => LearningType.GetTagClass();
    public bool IsShortCourseType => CourseType == CourseType.ShortCourse;
    public bool IsApprenticeshipType => CourseType == CourseType.Apprenticeship;
    public bool HasProviders => ProvidersCount > 0;
    public bool ShowNoProvidersRunThisCourseApprenticeshipMessage => !HasProviders && TotalProvidersCount == 0 && IsApprenticeshipType;
    public bool ShowNoProvidersBasedOnSearchApprenticeshipMessage => !HasProviders && TotalProvidersCount > 0 && IsApprenticeshipType;
    public bool ShowNoProvidersBasedOnSearchShortCourseMessage => !HasProviders && IsShortCourseType;
    public Dictionary<string, string> CourseDetailsRouteValues { get; init; } = new();

    public StandardViewModel(StandardModel source, string location, string distance, FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration, IUrlHelper urlHelper, List<LevelViewModel> levels)
    {
        Ordering = source.Ordering;
        StandardUId = source.StandardUId;
        IfateReferenceNumber = source.IfateReferenceNumber;
        LarsCode = source.LarsCode;
        SearchScore = source.SearchScore;
        ProvidersCount = source.ProvidersCount;
        TotalProvidersCount = source.TotalProvidersCount;
        Title = source.Title;
        Level = source.Level;
        OverviewOfRole = source.OverviewOfRole;
        Keywords = source.Keywords;
        Route = source.Route;
        RouteCode = source.RouteCode;
        TypicalDuration = source.TypicalDuration;
        LearningType = source.LearningType;
        CourseType = source.CourseType;
        MaxFunding = source.MaxFunding.ToString("C0", new CultureInfo("en-GB"));
        LevelName = GetLevelName(levels);
        RequestApprenticeshipTrainingUrl = GetRequestApprenticeshipTrainingUrl(findApprenticeshipTrainingWebConfiguration, location);
        FindProvidersUrl = GetFindProvidersUrl(urlHelper, location, distance);
        FindProvidersUrlDescription = GetFindProvidersUrlDescription(location, distance);
        GenerateStandardRouteValues(location, distance);
    }

    private string GetLevelName(List<LevelViewModel> levels)
    {
        LevelViewModel level = levels.Find(a => a.Code == Level);

        return $"{Level} - equal to {level.Name}";
    }

    private void GenerateStandardRouteValues(string location, string distance)
    {
        CourseDetailsRouteValues.Add("larsCode", LarsCode);

        if (!string.IsNullOrWhiteSpace(location))
        {
            CourseDetailsRouteValues.Add("location", location);
            CourseDetailsRouteValues.Add("distance", distance.ToString());
        }
    }

    private string GetRequestApprenticeshipTrainingUrl(FindApprenticeshipTrainingWeb findApprenticeshipTrainingWebConfiguration, string location)
    {
        if (IsShortCourseType || (IsApprenticeshipType && HasProviders)) return string.Empty;
        var requestApprenticeshipTrainingUrl = findApprenticeshipTrainingWebConfiguration.RequestApprenticeshipTrainingUrl;
        var employerAccountsUrl = findApprenticeshipTrainingWebConfiguration.EmployerAccountsUrl;

        string redirectUri = $"{requestApprenticeshipTrainingUrl}/accounts/{{{{hashedAccountId}}}}/employer-requests/overview?standardId={LarsCode}&requestType={EntryPoint.CourseDetail}";

        var locationQueryParam = !string.IsNullOrEmpty(location) ? $"&location={location}" : string.Empty;

        return $"{employerAccountsUrl}/service/?redirectUri={Uri.EscapeDataString(redirectUri + locationQueryParam)}";
    }

    private string GetFindProvidersUrl(IUrlHelper urlHelper, string location, string distance)
    {
        if (!HasProviders) return string.Empty;
        return urlHelper.RouteUrl(RouteNames.CourseProviders, new { larsCode = LarsCode, location, distance = distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE ? DistanceService.ACROSS_ENGLAND_FILTER_VALUE : distance })!;
    }

    private string GetFindProvidersUrlDescription(string location, string distance)
    {
        if (!HasProviders) return string.Empty;
        bool isNationalSearch = string.IsNullOrWhiteSpace(location) || distance == DistanceService.ACROSS_ENGLAND_FILTER_VALUE;

        string providerText = "training provider".ToQuantity(ProvidersCount);

        return isNationalSearch
            ? $"View {providerText} for this course"
            : $"View {providerText} within {distance} miles";
    }
}
