﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.FAT.Web.Models
{
    public class GetCoursesRequest
    {
        [FromQuery]
        public string Keyword { get; set; }
        [FromQuery]
        public List<Guid> Sectors { get; set; }

        [FromQuery]
        public List<int> Levels { get; set; }

        [FromQuery]
        public string OrderBy { get; set; }

    }
}