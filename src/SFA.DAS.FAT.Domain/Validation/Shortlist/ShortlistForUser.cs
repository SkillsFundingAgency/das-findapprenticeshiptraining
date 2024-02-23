using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Shortlist
{
    public class ShortlistForUser
    {
        public IEnumerable<ShortlistItem> Shortlist { get; set; }
    }
}
