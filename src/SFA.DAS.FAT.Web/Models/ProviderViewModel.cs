using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Extensions;

namespace SFA.DAS.FAT.Web.Models
{
    [Obsolete("FAT25 - Development - Replaced with course provider view model")]
    public class ProviderViewModel
    {
        public decimal? OverallAchievementRate { get; set; }

        public int? OverallCohort { get; set; }

        public uint ProviderId { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
        public string TradingName { get; set; }
        public string MarketingInfo { get; set; }
        public string OverallAchievementRatePercentage { get; set; }
        public string NationalOverallAchievementRatePercentage { get; set; }
        public IEnumerable<DeliveryModeViewModel> DeliveryModes { get; set; }
        public EmployerFeedbackViewModel EmployerFeedback { get; set; }
        public ApprenticeFeedbackViewModel ApprenticeFeedback { get; set; }
        public string ProviderDistance { get; set; }
        public string ProviderDistanceText { get; set; }
        public string ProviderAddress { get; set; }

        public Guid? ShortlistId { get; set; }

        public static implicit operator ProviderViewModel(Provider source)
        {
            if (source == null || source.ProviderId == 0)
            {
                return null;
            }
            return new ProviderViewModel
            {
                Name = source.Name,
                TradingName = !string.IsNullOrEmpty(source.TradingName) && !source.TradingName.Equals(source.Name, StringComparison.CurrentCultureIgnoreCase) ? source.TradingName : null,
                MarketingInfo = source.MarketingInfo,
                Email = source.Email,
                Phone = source.Phone,
                ShortlistId = source.ShortlistId,
                Website = source.Website,
                ProviderId = source.ProviderId,
                OverallCohort = source.OverallCohort,
                OverallAchievementRate = source.OverallAchievementRate,
                OverallAchievementRatePercentage = source.OverallAchievementRate.HasValue ? $"{Math.Round(source.OverallAchievementRate.Value) / 100:0%}" : "",
                NationalOverallAchievementRatePercentage = source.NationalOverallAchievementRate.HasValue ? $"{Math.Round(source.NationalOverallAchievementRate.Value) / 100:0%}" : "",
                DeliveryModes = source.DeliveryModes != null ? BuildDeliveryModes(source.DeliveryModes.ToList()) : new List<DeliveryModeViewModel>(),
                EmployerFeedback = new EmployerFeedbackViewModel(source.EmployerFeedback),
                ApprenticeFeedback = new ApprenticeFeedbackViewModel(source.ApprenticeFeedback),
                ProviderDistance = source.ProviderAddress?.DistanceInMiles != null ? source.ProviderAddress.DistanceInMiles.FormatDistance() : "",
                ProviderDistanceText = source.ProviderAddress != null ? GetProviderDistanceText(source.ProviderAddress.DistanceInMiles.FormatDistance()) : "",
                ProviderAddress = source.ProviderAddress != null ? BuildProviderAddress(source.ProviderAddress) : ""
            };
        }

        private static string GetProviderDistanceText(string distance)
        {
            if (string.IsNullOrEmpty(distance) || distance == "-1")
            {
                return "Head office";
            }

            if (distance == "1")
            {
                return "Head office 1 mile away";
            }

            return $"Head office {distance} miles away";
        }
        private static string BuildProviderAddress(ProviderAddress sourceProviderAddress)
        {
            var returnAddressFields = new List<string>();

            if (!string.IsNullOrEmpty(sourceProviderAddress.Address1))
            {
                returnAddressFields.Add(sourceProviderAddress.Address1);
            }
            if (!string.IsNullOrEmpty(sourceProviderAddress.Address2))
            {
                returnAddressFields.Add(sourceProviderAddress.Address2);
            }
            if (!string.IsNullOrEmpty(sourceProviderAddress.Address3))
            {
                returnAddressFields.Add(sourceProviderAddress.Address3);
            }
            if (!string.IsNullOrEmpty(sourceProviderAddress.Address4))
            {
                returnAddressFields.Add(sourceProviderAddress.Address4);
            }
            if (!string.IsNullOrEmpty(sourceProviderAddress.Town))
            {
                returnAddressFields.Add(sourceProviderAddress.Town);
            }
            if (!string.IsNullOrEmpty(sourceProviderAddress.Postcode))
            {
                returnAddressFields.Add(sourceProviderAddress.Postcode);
            }
            return string.Join(", ", returnAddressFields);
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
                new DeliveryModeViewModel().Map(blockRelease, DeliveryModeType.BlockRelease),

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
        public bool National { get; set; }
        public string NationalText { get; set; }

        public DeliveryModeViewModel Map(DeliveryMode source, DeliveryModeType deliveryModeType)
        {
            var viewModel = source ?? new DeliveryModeViewModel();
            viewModel.DeliveryModeType = deliveryModeType;
            viewModel.IsAvailable = source != default;
            viewModel.FormattedDistanceInMiles = source != default && deliveryModeType != DeliveryModeType.Workplace
                ? source.DistanceInMiles.FormatDistance() == "1"
                    ? ": 1 mile away"
                    : $": {source.DistanceInMiles.FormatDistance()} miles away"
                : null;
            viewModel.NationalText = source != default &&
                source.National && deliveryModeType == DeliveryModeType.Workplace ? "(national)" : null;
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
                Postcode = source.Postcode,
                National = source.National
            };
        }


    }
}
