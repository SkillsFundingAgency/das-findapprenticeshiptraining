using MediatR;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse;

public class GetCourseQuery : IRequest<GetCourseQueryResult>
{
    public string LarsCode { get; set; }
    public string LocationName { get; set; }
    public int? Distance { get; set; }
}
