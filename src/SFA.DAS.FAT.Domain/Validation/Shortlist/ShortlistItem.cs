using System;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Domain.Shortlist
{
    public class ShortlistItem
    {
        public Guid Id { get; set; }
        public Guid ShortlistUserId { get; set; }
        public Provider Provider { get; set; }
        public Course Course { get; set; }
        public string LocationDescription { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
