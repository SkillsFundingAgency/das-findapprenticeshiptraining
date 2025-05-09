using MediatR;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public class GetCourseQuery : IRequest<GetCourseQueryResult>
{
    public int LarsCode { get; set; }
    public string Location { get; set; }
    public int? Distance { get; set; }
}
