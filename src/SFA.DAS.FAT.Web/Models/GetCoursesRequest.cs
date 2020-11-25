﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class GetCoursesRequest
    {
        [FromQuery]
        public string Keyword { get; set; }
        [FromQuery]
        public List<string> Sectors { get; set; }

        [FromQuery]
        public List<int> Levels { get; set; }

        [FromQuery]
        public OrderBy OrderBy { get; set; }

    }
}
