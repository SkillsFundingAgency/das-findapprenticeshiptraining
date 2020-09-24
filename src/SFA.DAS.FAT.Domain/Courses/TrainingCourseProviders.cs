﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class TrainingCourseProviders
    {
        [JsonProperty("trainingCourse")]
        public Course Course { get; set; }
        [JsonProperty("trainingCourseProviders")]
        public IEnumerable<Provider> CourseProviders { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("location")]
        public Locations.Locations.LocationItem Location { get; set; }
    }
}
