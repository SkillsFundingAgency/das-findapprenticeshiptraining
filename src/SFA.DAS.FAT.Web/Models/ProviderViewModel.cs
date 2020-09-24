﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class ProviderViewModel
    {
        public decimal? OverallAchievementRate { get ; set ; }

        public int? OverallCohort { get ; set ; }

        public uint ProviderId { get ; set ; }

        public string Website { get ; set ; }

        public string Phone { get ; set ; }

        public string Email { get ; set ; }

        public string Name { get ; set ; }
        public string OverallAchievementRatePercentage { get ; set ; }
        public string NationalOverallAchievementRatePercentage { get ; set ; }
        public IEnumerable<DeliveryModeViewModel> DeliveryModes { get; set; }

        public int TotalEmployerResponses { get ; set ; }

        public int TotalFeedbackRating { get ; set ; }
        public string TotalFeedbackRatingText { get ; set ; }

        public ProviderRating TotalFeedbackText { get ; set ; }

        public static implicit operator ProviderViewModel(Provider source)
        {
            return new ProviderViewModel
            {
                Name = source.Name,
                Email = source.Email,
                Phone = source.Phone,
                Website = source.Website,    
                ProviderId = source.ProviderId,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                OverallAchievementRatePercentage = source.OverallAchievementRate.HasValue ? $"{Math.Round(source.OverallAchievementRate.Value)/100:0%}" : "",
                NationalOverallAchievementRatePercentage = source.NationalOverallAchievementRate.HasValue ? $"{Math.Round(source.NationalOverallAchievementRate.Value)/100:0%}" : "",
                DeliveryModes = source.DeliveryModes!=null ? BuildDeliveryModes(source.DeliveryModes.ToList()) : new List<DeliveryModeViewModel>(),
                TotalFeedbackRating = source.Feedback.TotalFeedbackRating,
                TotalEmployerResponses = source.Feedback.TotalEmployerResponses,
                TotalFeedbackRatingText = GetFeedbackRatingText(source),
                TotalFeedbackText = (ProviderRating)source.Feedback.TotalFeedbackRating
            };
        }

        private static string GetFeedbackRatingText(Provider source)
        {
            switch (source.Feedback.TotalEmployerResponses)
            {
                case 0:
                    return "Not yet reviewed (employer reviews)";
                case 1:
                    return "(1 employer review)";
            }

            return source.Feedback.TotalEmployerResponses > 50 ? "(50+ employer reviews)" 
                : $"({source.Feedback.TotalEmployerResponses} employer reviews)";
        }

        private static IEnumerable<DeliveryModeViewModel> BuildDeliveryModes(List<DeliveryMode> source)
        {
            if (source.Count == 0)
            {
                return new List<DeliveryModeViewModel>();
            }
            
            var dayRelease = source.SingleOrDefault(mode =>
                mode.DeliveryModeType == Domain.Courses.DeliveryModeType.DayRelease);
            var blockRelease = source.SingleOrDefault(mode => 
                mode.DeliveryModeType == Domain.Courses.DeliveryModeType.BlockRelease);
            var workPlace =
                source.SingleOrDefault(mode => mode.DeliveryModeType == Domain.Courses.DeliveryModeType.Workplace);
            var returnList = new List<DeliveryModeViewModel>
            {
                new DeliveryModeViewModel().Map(workPlace, DeliveryModeType.Workplace),
                new DeliveryModeViewModel().Map(dayRelease, DeliveryModeType.DayRelease),
                new DeliveryModeViewModel().Map(blockRelease, DeliveryModeType.BlockRelease)
            };
            
            return returnList;
        }
    }

    public class DeliveryModeViewModel
    {
        public DeliveryModeType DeliveryModeType { get; set; }
        public string FormattedDistanceInMiles { get; set; }
        public bool IsAvailable { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public string AddressFormatted { get; set; }

        public DeliveryModeViewModel Map(DeliveryMode source, DeliveryModeType deliveryModeType)
        {
            var viewModel = source ?? new DeliveryModeViewModel();
            viewModel.DeliveryModeType = deliveryModeType;
            viewModel.IsAvailable = source != default;
            viewModel.FormattedDistanceInMiles = source != default && deliveryModeType != DeliveryModeType.Workplace
                ? $"({source.DistanceInMiles:##.#} miles away)"
                : null;
            viewModel.AddressFormatted = source != default ? 
                BuildFormattedAddress(source) 
                : "";
            return viewModel;
        }

        private static string BuildFormattedAddress(DeliveryMode source)
        {
            var returnString = "";
            if (!string.IsNullOrEmpty(source.Address1))
            {
                returnString += $"{source.Address1}, ";
            }
            if (!string.IsNullOrEmpty(source.Address2))
            {
                returnString += $"{source.Address2}, ";
            }
            if (!string.IsNullOrEmpty(source.Town))
            {
                returnString += $"{source.Town}, ";
            }
            if (!string.IsNullOrEmpty(source.County))
            {
                returnString += $"{source.County}, ";
            }
            if (!string.IsNullOrEmpty(source.Postcode))
            {
                returnString += $"{source.Postcode},";
            }
            
            return returnString.TrimEnd().TrimEnd(',');
        }

        public static implicit operator DeliveryModeViewModel(DeliveryMode source)
        {
            return new DeliveryModeViewModel
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                Town = source.Town,
                County = source.County,
                Postcode = source.Postcode
            };
        }
    }

    public enum DeliveryModeType
    {
        [Description("At apprentice’s workplace")]
        Workplace = 0,
        [Description("Day release")]
        DayRelease = 1,
        [Description("Block release")]
        BlockRelease = 2
    }

    public enum ProviderRating
    {
        [Description("Very poor")]
        VeryPoor = 1,
        [Description("Poor")]
        Poor = 2,
        [Description("Good")]
        Good = 3,
        [Description("Excellent")]
        Excellent = 4
    }
}
