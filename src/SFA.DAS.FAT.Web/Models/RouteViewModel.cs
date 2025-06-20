using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models;

public class RouteViewModel
{
    public RouteViewModel(Route route, ICollection<string> selectedRoutes)
    {
        Selected = selectedRoutes?.Contains(route.Name) ?? false;
        Id = route.Id;
        Name = route.Name;
    }

    public bool Selected { get; }
    public string Name { get; }
    public int Id { get; }
}
