using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourses;

public class GetCoursesQuery : IRequest<GetCoursesQueryResult>
{
    public string Keyword { get; set; }
    public string Location { get; set; }
    public int? Distance { get; set; }
    public int Page { get; set; } = 1;
    public List<string> Routes { get; set; }
    public List<int> Levels { get; set; }
    public OrderBy OrderBy { get; set; }

    public List<string> Trainings { get; set; }
    public Guid? ShortlistUserId { get; set; }
}
