using System;
using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class RouteViewModel
{
    public RouteViewModel()
    {
        
    }

    public RouteViewModel (Route route, ICollection<string> selectedSectors)
    {
        Selected =selectedSectors?.Contains(route.Name) ?? false;
        Id = Guid.NewGuid();
        Name = route.Name;
    }

    public bool Selected { get;  }
    public string Name { get ;  }
    public Guid Id { get ;  }
}
