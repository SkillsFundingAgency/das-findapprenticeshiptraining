﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class TrainingCourseProviderDetails
    {
        [JsonProperty("trainingCourseProvider")]
        public Provider CourseProviderDetails { get; set; }
        
        [JsonProperty("trainingCourse")]
        public Course TrainingCourse { get; set; }

        [JsonProperty("additionalCourses")]
        public AdditionalCourses AdditionalCourses { get; set; }
        [JsonProperty("location")]
        public Locations.Locations.LocationItem Location { get; set; }
        [JsonProperty("providersCount")] 
        public ProvidersCount ProvidersCount { get; set; }
    }

    public class AdditionalCourses
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("courses")]
        public IEnumerable<AdditionalCourse> Courses { get; set; }
    }

    public class AdditionalCourse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
