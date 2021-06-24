using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetCourse
{
    public class GetCourseResult
    {
        public Course Course { get ; set ; }
        public ProvidersCount ProvidersCount { get; set; }
        public int ShortlistItemCount { get ; set ; }
    }
}
