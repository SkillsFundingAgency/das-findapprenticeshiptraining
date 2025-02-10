using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.FAT.Application.CourseProviders.Queries.GetCourseProviders;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.BreadCrumbs;

namespace SFA.DAS.FAT.Web.Models
{
    public class CourseProvidersViewModel : PageLinksViewModelBase
    {
        public CourseProvidersViewModel(GetCourseProvidersRequest request, GetCourseProvidersResult result, Dictionary<uint, string> providerOrder)
        {
            ProviderOrder = providerOrder;
            CourseId = request.Id;
            CourseTitleAndLevel = result.StandardName;

            // MFCMFC get this from const??
            var defaultDistance = 10;

            Distance = request.Distance switch
            {
                null when request.Location != null => defaultDistance,
                null => null,
                _ => request.Distance
            };


            LocationFilterDescription = string.Empty;
            if (Distance != null && request.Location != null)
            {
                LocationFilterDescription = Distance == MaximumDistance
                    ? $"{request.Location} (across England)"
                    : $"{request.Location} (within {Distance} miles)";
            }

            // MFCMFC holding pattern
            Providers = new List<ProviderViewModel>();
            // Providers = result.Providers.Select(c => (ProviderViewModel)c);



            Location = request.Location;
            Total = result.TotalCount;
            TotalFiltered = Providers.Count();

            DeliveryModes = BuildDeliveryModeOptionViewModel(request.DeliveryModes);
            EmployerProviderRatings = BuildEmployerProviderRatingOptionViewModel(request.EmployerProviderRatings);
            ApprenticeProviderRatings = BuildApprenticeProviderRatingOptionViewModel(request.ApprenticeProviderRatings);
            QarRatings = BuildQarOptionViewModel(request.QarRatings);
            QarPeriod = result.QarPeriod;
            ReviewPeriod = result.ReviewPeriod;
        }

        public int MaximumDistance => 500;
        public IEnumerable<ProviderViewModel> Providers { get; set; }

        public string CourseTitleAndLevel { get; set; }


        public string LocationFilterDescription { get; set; }

        public int Total { get; set; }
        public int TotalFiltered { get; set; }
        public string TotalMessage => GetTotalMessage();

        public string ClearLocationLink => BuildClearLocationFilterLink();
        public Dictionary<string, string> ClearDeliveryModeLinks => BuildClearDeliveryModeLinks();
        public Dictionary<string, string> ClearEmployerProviderRatingLinks => BuildClearEmployerProviderRatingLinks();
        public Dictionary<string, string> ClearApprenticeProviderRatingLinks => BuildClearApprenticeProviderRatingLinks();

        public bool HasLocation => !string.IsNullOrWhiteSpace(Location);
        public bool HasEmployerProviderRatings => EmployerProviderRatings != null && EmployerProviderRatings.Any(model => model.Selected);
        public bool HasApprenticeProviderRatings => ApprenticeProviderRatings != null && ApprenticeProviderRatings.Any(model => model.Selected);
        public bool HasQarRatings => QarRatings != null && QarRatings.Any(model => model.Selected);

        public bool HasDeliveryModes => DeliveryModes != null && DeliveryModes.Any(model => model.Selected);
        public bool ShowSelectedFilters => ShouldShowFilters();

        public IEnumerable<DeliveryModeOptionViewModel> DeliveryModes { get; set; }
        public IEnumerable<EmployerProviderRatingOptionViewModel> EmployerProviderRatings { get; set; }
        public IEnumerable<ApprenticeProviderRatingOptionViewModel> ApprenticeProviderRatings { get; set; }

        public IEnumerable<QarOptionViewModel> QarRatings { get; set; }
        public Dictionary<uint, string> ProviderOrder { get; }
        public string BannerUpdateMessage { get; set; }

        public string QarPeriod { get; set; }
        public string ReviewPeriod { get; set; }


        public int? Distance { get; set; }

        public string QarPeriodStartYear { get => $"20{QarPeriod.AsSpan(0, 2)}"; }

        public string QarPeriodEndYear { get => $"20{QarPeriod.AsSpan(2, 2)}"; }

        public string ReviewPeriodStartYear { get => $"20{ReviewPeriod.AsSpan(0, 2)}"; }
        public string ReviewPeriodEndYear { get => $"20{ReviewPeriod.AsSpan(2, 2)}"; }

        //public CourseViewModel Course { get; set; }
        public bool CanGetHelpFindingCourse(FindApprenticeshipTrainingWeb config)
        {
            return true;
            // MFCMFC return Course.CanGetHelpFindingCourse(config);
        }

        public string GetHelpFindingCourseUrl(FindApprenticeshipTrainingWeb config)
        {
            return "";
            // return Course.GetHelpFindingCourseUrl(config, EntryPoint.Providers, Location);
        }

        private bool ShouldShowFilters()
        {
            var result = HasLocation ||
                         HasDeliveryModes ||
                         HasEmployerProviderRatings ||
                         HasApprenticeProviderRatings ||
                         HasQarRatings;
            return result;
        }

        private Dictionary<string, string> BuildClearDeliveryModeLinks()
        {
            var clearDeliveryModeLinks = new Dictionary<string, string>();

            if (DeliveryModes == null)
            {
                return clearDeliveryModeLinks;
            }

            var location = BuildLocationLink();
            var providerRatings = BuildEmployerProviderRatingLinks(location);
            var apprenticeProviderRating = BuildApprenticeProviderRatingLinks(location);

            foreach (var deliveryMode in DeliveryModes.Where(model => model.Selected))
            {
                var otherSelected = DeliveryModes
                    .Where(viewModel =>
                        viewModel.Selected &&
                        viewModel.DeliveryModeChoice != deliveryMode.DeliveryModeChoice)
                    .Select(viewModel => viewModel.DeliveryModeChoice);

                var link = $"{location}&deliveryModes={string.Join("&deliveryModes=", otherSelected)}{providerRatings}{apprenticeProviderRating}";

                clearDeliveryModeLinks.Add(deliveryMode.Description, link);
            }

            return clearDeliveryModeLinks;
        }
        private Dictionary<string, string> BuildClearEmployerProviderRatingLinks()
        {
            var providerRatingLinks = new Dictionary<string, string>();

            if (EmployerProviderRatings == null)
            {
                return providerRatingLinks;
            }

            var location = BuildLocationLink();
            var deliveryModes = BuildDeliveryModeLinks(location);
            var apprenticeProviderRating = BuildApprenticeProviderRatingLinks(location);

            foreach (var providerRating in EmployerProviderRatings.Where(model => model.Selected).OrderByDescending(c => c.ProviderRatingType))
            {
                var otherSelected = EmployerProviderRatings
                    .Where(viewModel =>
                        viewModel.Selected &&
                        viewModel.ProviderRatingType != providerRating.ProviderRatingType)
                    .Select(viewModel => viewModel.ProviderRatingType);
                var link = $"{location}{deliveryModes}&employerProviderRatings={string.Join("&employerProviderRatings=", otherSelected)}{apprenticeProviderRating}";

                providerRatingLinks.Add(providerRating.Description, link);
            }
            return providerRatingLinks;
        }
        private Dictionary<string, string> BuildClearApprenticeProviderRatingLinks()
        {
            var apprenticeProviderRatingLinks = new Dictionary<string, string>();

            if (ApprenticeProviderRatings == null)
            {
                return apprenticeProviderRatingLinks;
            }

            var location = BuildLocationLink();
            var deliveryModes = BuildDeliveryModeLinks(location);
            var employerProviderRatings = BuildEmployerProviderRatingLinks(location);

            foreach (var apprenticeProviderRating in ApprenticeProviderRatings.Where(model => model.Selected).OrderByDescending(c => c.ProviderRatingType))
            {
                var otherSelected = ApprenticeProviderRatings
                    .Where(viewModel =>
                        viewModel.Selected &&
                        viewModel.ProviderRatingType != apprenticeProviderRating.ProviderRatingType)
                    .Select(viewModel => viewModel.ProviderRatingType);
                var link = $"{location}{deliveryModes}{employerProviderRatings}&apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", otherSelected)}";

                apprenticeProviderRatingLinks.Add(apprenticeProviderRating.Description, link);
            }
            return apprenticeProviderRatingLinks;
        }

        private string BuildClearLocationFilterLink()
        {
            var location = "?location="; //"?location=-1";
            var employerProviderRatings = BuildEmployerProviderRatingLinks(location);
            var apprenticeProviderRating = BuildApprenticeProviderRatingLinks(location);
            var deliveryModeLinks = BuildDeliveryModeLinks(location);

            var link = $"{location}{employerProviderRatings}{deliveryModeLinks}{apprenticeProviderRating}";

            return link;
        }

        private string BuildDeliveryModeLinks(string linkToAppendTo)
        {
            if (HasDeliveryModes)
            {
                var deliveryModes = DeliveryModes.Where(dm => dm.Selected).Select(dm => dm.DeliveryModeChoice);
                return $"{GetSeparator(linkToAppendTo)}deliveryModes={string.Join("&deliveryModes=", deliveryModes)}";
            }
            return null;
        }

        private string BuildLocationLink()
        {
            return $"?location={HttpUtility.UrlEncode(Location)}";
        }

        private string BuildEmployerProviderRatingLinks(string linkToAppendTo)
        {
            if (HasEmployerProviderRatings)
            {
                var employerProviderRatings = EmployerProviderRatings.Where(pr => pr.Selected).Select(pr => pr.ProviderRatingType);
                return $"{GetSeparator(linkToAppendTo)}employerProviderRatings={string.Join("&employerProviderRatings=", employerProviderRatings)}";
            }
            return null;
        }
        private string BuildApprenticeProviderRatingLinks(string linkToAppendTo)
        {
            if (HasApprenticeProviderRatings)
            {
                var apprenticeProviderRatings = ApprenticeProviderRatings.Where(pr => pr.Selected).Select(pr => pr.ProviderRatingType);
                return $"{GetSeparator(linkToAppendTo)}apprenticeProviderRatings={string.Join("&apprenticeProviderRatings=", apprenticeProviderRatings)}";
            }
            return null;
        }

        private string GetTotalMessage()
        {
            var totalToUse = !HasDeliveryModes && !HasEmployerProviderRatings && !HasApprenticeProviderRatings
                ? Total
                : TotalFiltered;

            var totalMessage = $"{totalToUse} result{(totalToUse != 1 ? "s" : "")}";

            if (HasLocation && totalToUse == 0)
            {
                totalMessage += $" at {Location}";
            }

            return totalMessage;
        }

        private static IEnumerable<DeliveryModeOptionViewModel> BuildDeliveryModeOptionViewModel(IReadOnlyList<ProviderDeliveryMode> selectedDeliveryModeChoices)
        {
            var deliveryModeOptionViewModels = new List<DeliveryModeOptionViewModel>();

            foreach (ProviderDeliveryMode deliveryModeChoice in Enum.GetValues(typeof(ProviderDeliveryMode)))
            {
                deliveryModeOptionViewModels.Add(new DeliveryModeOptionViewModel
                {
                    DeliveryModeChoice = deliveryModeChoice,
                    Description = deliveryModeChoice.GetDescription(),
                    Selected = selectedDeliveryModeChoices.Any(type => type == deliveryModeChoice)
                });
            }

            // if (deliveryModeOptionViewModels.FirstOrDefault(c =>
            //     c.Selected && c.DeliveryModeChoice == DeliveryModeType.National) != null)
            // {
            //     deliveryModeOptionViewModels.First(c => c.DeliveryModeChoice == DeliveryModeType.Workplace).Selected =
            //         true;
            // }

            return deliveryModeOptionViewModels;
        }

        private static IEnumerable<EmployerProviderRatingOptionViewModel> BuildEmployerProviderRatingOptionViewModel(IReadOnlyList<ProviderRating> selectedProviderRatingTypes)
        {
            var employerProviderRatingOptionViewModel = new List<EmployerProviderRatingOptionViewModel>();

            foreach (ProviderRating employerProviderRatingType in Enum.GetValues(typeof(ProviderRating)))
            {
                employerProviderRatingOptionViewModel.Add(new EmployerProviderRatingOptionViewModel
                {
                    ProviderRatingType = employerProviderRatingType,
                    Description = employerProviderRatingType.GetDescription(),
                    Selected = selectedProviderRatingTypes.Any(type => type == employerProviderRatingType)
                });
            }

            return employerProviderRatingOptionViewModel;
        }

        private static IEnumerable<ApprenticeProviderRatingOptionViewModel> BuildApprenticeProviderRatingOptionViewModel(IReadOnlyList<ProviderRating> selectedProviderRatingTypes)
        {
            var apprenticeProviderRatingOptionViewModel = new List<ApprenticeProviderRatingOptionViewModel>();

            foreach (ProviderRating apprenticeProviderRatingType in Enum.GetValues(typeof(ProviderRating)))
            {
                apprenticeProviderRatingOptionViewModel.Add(new ApprenticeProviderRatingOptionViewModel
                {
                    ProviderRatingType = apprenticeProviderRatingType,
                    Description = apprenticeProviderRatingType.GetDescription(),
                    Selected = selectedProviderRatingTypes.Any(type => type == apprenticeProviderRatingType)
                });
            }

            return apprenticeProviderRatingOptionViewModel;
        }

        private static IEnumerable<QarOptionViewModel> BuildQarOptionViewModel(IReadOnlyList<QarRating> selectedQarTypes)
        {
            var vm = new List<QarOptionViewModel>();

            foreach (QarRating qarRatingType in Enum.GetValues(typeof(QarRating)))
            {
                vm.Add(new QarOptionViewModel
                {
                    QarRatingType = qarRatingType,
                    Description = qarRatingType.GetDescription(),
                    Selected = selectedQarTypes.Any(type => type == qarRatingType)
                });
            }

            return vm;
        }

        private static string GetSeparator(string url)
        {
            return string.IsNullOrEmpty(url) ? "?" : "&";
        }
    }
}
