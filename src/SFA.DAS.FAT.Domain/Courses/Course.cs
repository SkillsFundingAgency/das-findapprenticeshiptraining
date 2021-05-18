using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public string LevelEquivalent { get; set; }
        public string OverviewOfRole { get; set; }
        public string Route { get; set; }
        public string IntegratedDegree { get; set; }
        public List<string> CoreSkills { get; set; }
        public List<string> TypicalJobTitles { get ; set ; }
        public string StandardPageUrl { get ; set ; }
        public int TypicalDuration { get ; set ; }
        public long MaxFunding { get ; set ; }
        public bool OtherBodyApprovalRequired { get; set; }
        public string ApprovalBody { get; set; }
        public StandardDates StandardDates { get; set; }
    }
}
