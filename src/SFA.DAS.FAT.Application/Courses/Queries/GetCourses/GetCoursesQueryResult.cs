using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api.Responses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public sealed class GetCoursesQueryResult
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Constants.DefaultPageSize;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public List<StandardModel> Standards { get; set; } = [];
    public IEnumerable<Level> Levels { get; set; } = [];
    public IEnumerable<Route> Routes { get; set; } = [];
}
