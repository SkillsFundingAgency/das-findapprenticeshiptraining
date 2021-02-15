﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.FAT.Web.Models
{
    public class DeleteShortlistItemRequest
    {
        [FromRoute(Name="id")]
        public Guid ShortlistId { get; set; }
        public int Ukprn { get ; set ; }
        public int TrainingCode { get ; set ; }
        public string RouteName { get; set; }
    }
}