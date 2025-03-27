using System;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Domain.Shortlist
{
    [Obsolete("This class is no longer used and will be removed in a future release. Please use SFA.DAS.FAT.Domain.Models.Shortlist.ShortlistForUser instead.")]
    public class ShortlistForUser
    {
        public IEnumerable<ShortlistItem> Shortlist { get; set; }
    }
}
