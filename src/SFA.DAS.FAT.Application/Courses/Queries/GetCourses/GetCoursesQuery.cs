using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQuery : IRequest<GetCoursesQueryResult>
{
    public string Keyword { get; set; }
    public string Location { get; set; }
    public List<int> RouteIds { get; set; }
    public List<int> Levels { get; set; }
    public OrderBy OrderBy { get; set; }
    public Guid? ShortlistUserId { get ; set ; }
}
