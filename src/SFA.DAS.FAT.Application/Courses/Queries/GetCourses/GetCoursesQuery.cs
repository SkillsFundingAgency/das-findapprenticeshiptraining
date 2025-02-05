using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQuery : IRequest<GetCoursesResult>
{
    public string Keyword { get; set; }
    public List<string> RouteIds { get; set; }
    public List<int> Levels { get; set; }
    public OrderBy OrderBy { get; set; }
    public Guid? ShortlistUserId { get ; set ; }
}
