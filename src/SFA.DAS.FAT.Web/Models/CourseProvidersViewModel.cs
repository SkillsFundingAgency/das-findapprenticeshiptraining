﻿using System.Collections.Generic;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class CourseProvidersViewModel
    {
        public IEnumerable<Provider> Providers { get; set; }
        public CourseViewModel Course { get; set; }
        public int Total { get; set; }
        public string TotalMessage => Total == 1 ? $"{Total} result" : $"{Total} results";
    }
}