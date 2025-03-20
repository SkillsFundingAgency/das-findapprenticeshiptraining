using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Models.Filters.FilterComponents;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests;
public sealed class WhenFilteringCourseProviders
{
    private CourseProvidersViewModel _viewModel;

    private FindApprenticeshipTrainingWeb _findApprenticeshipTrainingWebConfiguration;

    private Mock<IUrlHelper> _urlHelperMock;

    private string _fullQueryString;

    private const string ExpectedReviewsSubHeading = "From 2023 to 2024";
    private const string ExpectedQarSubHeading = "From 2022 to 2023";

    [SetUp]
    public void Setup()
    {
        _urlHelperMock = new Mock<IUrlHelper>();
        _findApprenticeshipTrainingWebConfiguration = new FindApprenticeshipTrainingWeb()
        {
            RequestApprenticeshipTrainingUrl = "https://localhost"
        };

        _viewModel = new CourseProvidersViewModel(_findApprenticeshipTrainingWebConfiguration, _urlHelperMock.Object)
        {
            Location = "M60 7RA",
            Distance = "20",
            SelectedDeliveryModes = new List<string> { ProviderDeliveryMode.Provider.ToString(), ProviderDeliveryMode.DayRelease.ToString(), ProviderDeliveryMode.BlockRelease.ToString() },
            SelectedEmployerApprovalRatings = new List<string>() { ProviderRating.Good.ToString(), ProviderRating.Excellent.ToString() },
            SelectedApprenticeApprovalRatings = new List<string>() { ProviderRating.Poor.ToString(), ProviderRating.VeryPoor.ToString() },
            SelectedQarRatings = new List<string> { QarRating.VeryPoor.ToString(), QarRating.Excellent.ToString() },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        _fullQueryString = "?location=M60 7RA&distance=20&deliverymodes=Provider&deliverymodes=DayRelease&deliverymodes=BlockRelease&employerproviderratings=Excellent&employerproviderratings=Good&apprenticeproviderratings=Poor&apprenticeproviderratings=VeryPoor&qarratings=Excellent&qarratings=VeryPoor";
    }

    [Test]
    public void Then_Filters_Must_Contain_Location_Filter_Section()
    {
        var sut = _viewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(sut.Any(a => a.For == nameof(_viewModel.Location)), Is.True);

            var locationFilterSection = sut.First(a => a.For == nameof(_viewModel.Location));
            Assert.That(locationFilterSection, Is.TypeOf<SearchFilterSectionViewModel>());
            Assert.That(locationFilterSection.Id, Is.EqualTo("search-location"));
            Assert.That(locationFilterSection.For, Is.EqualTo(nameof(_viewModel.Location)));
            Assert.That(locationFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Search));
            Assert.That(((SearchFilterSectionViewModel)locationFilterSection).InputValue, Is.EqualTo(_viewModel.Location));
            Assert.That(locationFilterSection.Heading, Is.EqualTo(FilterService.LOCATION_SECTION_HEADING));
            Assert.That(locationFilterSection.SubHeading, Is.EqualTo(FilterService.LOCATION_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Distance_Filter_Section()
    {
        var sut = _viewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            Assert.That(sut.Any(a => a.For == nameof(_viewModel.Distance)), Is.True);

            var distanceFilterSection = sut.First(a => a.For == nameof(_viewModel.Distance));
            Assert.That(distanceFilterSection, Is.TypeOf<DropdownFilterSectionViewModel>());
            Assert.That(distanceFilterSection.Id, Is.EqualTo("distance-filter"));
            Assert.That(distanceFilterSection.For, Is.EqualTo(nameof(_viewModel.Distance)));
            Assert.That(distanceFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.Dropdown));

            var dropdownFilter = ((DropdownFilterSectionViewModel)distanceFilterSection);

            dropdownFilter.Items.Should().BeEquivalentTo(
                FilterService.GetDistanceFilterValues(_viewModel.Distance),
                options => options.WithStrictOrdering()
            );

            var selectedDistanceValue = dropdownFilter.Items.First(a => a.Selected);
            Assert.That(selectedDistanceValue.Value, Is.EqualTo(_viewModel.Distance));
            Assert.That(distanceFilterSection.Heading, Is.EqualTo(FilterService.DISTANCE_SECTION_HEADING));
            Assert.That(distanceFilterSection.SubHeading, Is.EqualTo(FilterService.DISTANCE_SECTION_SUB_HEADING));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_Location_Clear_Link()
    {
        var sut = _viewModel.Filters.ClearFilterSections;

        var urlWithoutLocation = _fullQueryString.Replace($"location={_viewModel.Location}&distance={_viewModel.Distance}&", "");

        Assert.Multiple(() =>
        {
            var locationClearLink = sut.First(a => a.FilterType == FilterService.FilterType.Location);
            Assert.That(locationClearLink, Is.Not.Null);
            Assert.That(locationClearLink.Title, Is.EqualTo("Apprentice's work location"));
            Assert.That(locationClearLink.Items[0].DisplayText, Is.EqualTo($"{_viewModel.Location} (within {_viewModel.Distance} miles)"));
            Assert.That(locationClearLink.Items[0].ClearLink, Is.EqualTo(urlWithoutLocation));
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_DeliveryModes_Filter_Section()
    {
        var focus = "DeliveryModes";
        var sut = _viewModel.Filters.FilterSections;

        Assert.That(sut.Any(a => a.For == focus), Is.True);

        var deliveryModesFilterSection = sut.First(a => a.For == focus);
        var checkboxList = ((CheckboxListFilterSectionViewModel)deliveryModesFilterSection);

        Assert.Multiple(() =>
        {
            Assert.That(deliveryModesFilterSection.Id, Is.EqualTo("modes-filter"));
            Assert.That(deliveryModesFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(deliveryModesFilterSection.FilterComponentType,
                Is.EqualTo(FilterService.FilterComponentType.CheckboxList));
            Assert.That(checkboxList.Items, Has.Count.EqualTo(Enum.GetNames(typeof(ProviderDeliveryMode)).Length));
            Assert.That(checkboxList.Items.Where(a => a.Selected).ToList(),
                Has.Count.EqualTo(_viewModel.SelectedDeliveryModes.Count));
            Assert.That(checkboxList.Heading, Is.EqualTo(FilterService.DELIVERYMODES_SECTION_HEADING));
            Assert.That(checkboxList.SubHeading, Is.EqualTo(FilterService.DELIVERYMODES_SECTION_SUB_HEADING));
            Assert.That(checkboxList.Link, Is.Null);
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_DeliveryModes_Clear_Links()
    {
        var sut = _viewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var clearLinks = sut.First(a => a.FilterType == FilterService.FilterType.DeliveryModes);
            Assert.That(clearLinks, Is.Not.Null);
            Assert.That(clearLinks.Items, Has.Count.EqualTo(_viewModel.SelectedDeliveryModes.Count));

            var urlWithoutDeliveryModeProvider = _fullQueryString.Replace("&deliverymodes=Provider", "");
            var urlWithoutDeliveryModeDayRelease = _fullQueryString.Replace("&deliverymodes=DayRelease", "");
            var urlWithoutDeliveryModeBlockRelease = _fullQueryString.Replace("&deliverymodes=BlockRelease", "");

            Assert.Multiple(() =>
            {
                var workPlaceLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderDeliveryMode.Provider.GetDescription());
                Assert.That(workPlaceLink, Is.Not.Null);
                Assert.That(workPlaceLink.ClearLink, Is.EqualTo(urlWithoutDeliveryModeProvider));

                var dayReleaseLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderDeliveryMode.DayRelease.GetDescription());
                Assert.That(dayReleaseLink, Is.Not.Null);
                Assert.That(dayReleaseLink.ClearLink, Is.EqualTo(urlWithoutDeliveryModeDayRelease));

                var blockReleaseLink = clearLinks.Items.First(a =>
                    a.DisplayText == ProviderDeliveryMode.BlockRelease.GetDescription());
                Assert.That(blockReleaseLink, Is.Not.Null);
                Assert.That(blockReleaseLink.ClearLink, Is.EqualTo(urlWithoutDeliveryModeBlockRelease));
            });
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Accordion_Filter_Section_With_EmployerProviderReviews()
    {
        var focus = "Reviews";
        var sut = _viewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = sut.First(a => a.Id == "ratings-select");
            Assert.That(accordionFilterSection, Is.TypeOf<AccordionGroupFilterSectionViewModel>());
            Assert.That(accordionFilterSection.For, Is.EqualTo(focus));
            Assert.That(accordionFilterSection.Heading, Is.EqualTo(FilterService.REVIEW_SECTION_HEADING));
            Assert.That(accordionFilterSection.SubHeading, Is.EqualTo(ExpectedReviewsSubHeading));
            Assert.That(accordionFilterSection.Link, Is.Null);
            Assert.That(accordionFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.AccordionGroup));

            var employerProviderRatingsFilterSection = accordionFilterSection.Children.First(a => a.For == "EmployerProviderRatings");
            Assert.That(employerProviderRatingsFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(employerProviderRatingsFilterSection.Id, Is.EqualTo("employer-ratings-filter"));
            Assert.That(employerProviderRatingsFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var checkBoxList = ((CheckboxListFilterSectionViewModel)employerProviderRatingsFilterSection);

            Assert.That(checkBoxList.Items, Has.Count.EqualTo(Enum.GetNames(typeof(ProviderRating)).Length));
            Assert.That(checkBoxList.Items.Where(a => a.Selected).ToList(), Has.Count.EqualTo(_viewModel.SelectedEmployerApprovalRatings.Count));
            Assert.That(checkBoxList.Heading, Is.EqualTo(FilterService.EMPLOYER_REVIEWS_SECTION_HEADING));
            Assert.That(checkBoxList.Link, Is.Null);
            Assert.That(checkBoxList.SubHeading, Is.Null);
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_EmployerProviderReviews_Clear_Links()
    {
        var sut = _viewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var clearLinks = sut.First(a => a.FilterType == FilterService.FilterType.EmployerProviderRatings);
            Assert.That(clearLinks, Is.Not.Null);
            Assert.That(clearLinks.Items, Has.Count.EqualTo(_viewModel.SelectedEmployerApprovalRatings.Count));

            var urlWithoutEmployerProviderReviewsGood = _fullQueryString.Replace("&employerproviderratings=Good", "");
            var urlWithoutEmployerProviderReviewsExcellent = _fullQueryString.Replace("&employerproviderratings=Excellent", "");

            Assert.Multiple(() =>
            {
                var employerProviderReviewsGoodLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderRating.Good.GetDescription());
                Assert.That(employerProviderReviewsGoodLink, Is.Not.Null);
                Assert.That(employerProviderReviewsGoodLink.ClearLink, Is.EqualTo(urlWithoutEmployerProviderReviewsGood));

                var employerProviderReviewsExcellentLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderRating.Excellent.GetDescription());
                Assert.That(employerProviderReviewsExcellentLink, Is.Not.Null);
                Assert.That(employerProviderReviewsExcellentLink.ClearLink, Is.EqualTo(urlWithoutEmployerProviderReviewsExcellent));
            });
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Accordion_Filter_Section_With_ApprenticeProviderReviews()
    {
        var sut = _viewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = sut.First(a => a.Id == "ratings-select");
            var apprenticeProviderRatingsFilterSection = accordionFilterSection.Children.First(a => a.For == "ApprenticeProviderRatings");
            Assert.That(apprenticeProviderRatingsFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(apprenticeProviderRatingsFilterSection.Id, Is.EqualTo("apprentice-ratings-filter"));
            Assert.That(apprenticeProviderRatingsFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var checkBoxList = ((CheckboxListFilterSectionViewModel)apprenticeProviderRatingsFilterSection);

            Assert.That(checkBoxList.Items, Has.Count.EqualTo(Enum.GetNames(typeof(ProviderRating)).Length));
            Assert.That(checkBoxList.Items.Where(a => a.Selected).ToList(), Has.Count.EqualTo(_viewModel.SelectedApprenticeApprovalRatings.Count));
            Assert.That(checkBoxList.Heading, Is.EqualTo(FilterService.APPRENTICE_REVIEWS_SECTION_HEADING));
            Assert.That(checkBoxList.Link, Is.Null);
            Assert.That(checkBoxList.SubHeading, Is.Null);
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_ApprenticeProviderReviews_Clear_Links()
    {
        var sut = _viewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var clearLinks = sut.First(a => a.FilterType == FilterService.FilterType.ApprenticeProviderRatings);
            Assert.That(clearLinks, Is.Not.Null);
            Assert.That(clearLinks.Items, Has.Count.EqualTo(_viewModel.SelectedApprenticeApprovalRatings.Count));

            var urlWithoutApprenticeProviderReviewsVeryPoor = _fullQueryString.Replace("&apprenticeproviderratings=VeryPoor", "");
            var urlWithoutApprenticeProviderReviewsPoor = _fullQueryString.Replace("&apprenticeproviderratings=Poor", "");

            Assert.Multiple(() =>
            {
                var apprenticeProviderReviewsVeryPoorLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderRating.VeryPoor.GetDescription());
                Assert.That(apprenticeProviderReviewsVeryPoorLink, Is.Not.Null);
                Assert.That(apprenticeProviderReviewsVeryPoorLink.ClearLink, Is.EqualTo(urlWithoutApprenticeProviderReviewsVeryPoor));

                var apprenticeProviderReviewsPoorLink =
                    clearLinks.Items.First(a => a.DisplayText == ProviderRating.Poor.GetDescription());
                Assert.That(apprenticeProviderReviewsPoorLink, Is.Not.Null);
                Assert.That(apprenticeProviderReviewsPoorLink.ClearLink, Is.EqualTo(urlWithoutApprenticeProviderReviewsPoor));
            });
        });
    }

    [Test]
    public void Then_Filters_Must_Contain_Accordion_Filter_Section_With_QarRatings()
    {
        var sut = _viewModel.Filters.FilterSections;

        Assert.Multiple(() =>
        {
            var accordionFilterSection = sut.First(a => a.Id == "qar-select");
            var qarRatingsFilterSection = accordionFilterSection.Children.First(a => a.For == "QarRatings");
            Assert.That(qarRatingsFilterSection, Is.TypeOf<CheckboxListFilterSectionViewModel>());
            Assert.That(qarRatingsFilterSection.Id, Is.EqualTo("qar-filter"));
            Assert.That(qarRatingsFilterSection.FilterComponentType, Is.EqualTo(FilterService.FilterComponentType.CheckboxList));

            var checkBoxList = ((CheckboxListFilterSectionViewModel)qarRatingsFilterSection);

            Assert.That(checkBoxList.Items, Has.Count.EqualTo(Enum.GetNames(typeof(QarRating)).Length));
            Assert.That(checkBoxList.Items.Where(a => a.Selected).ToList(), Has.Count.EqualTo(_viewModel.SelectedQarRatings.Count));
            Assert.That(checkBoxList.Heading, Is.EqualTo(FilterService.QAR_SECTION_HEADING));
            Assert.That(checkBoxList.Link, Is.Null);
            Assert.That(checkBoxList.SubHeading, Is.EqualTo(ExpectedQarSubHeading));
        });
    }

    [Test]
    public void Then_Clear_Filter_Sections_Must_Contain_QarReviews_Clear_Links()
    {
        var sut = _viewModel.Filters.ClearFilterSections;

        Assert.Multiple(() =>
        {
            var clearLinks = sut.First(a => a.FilterType == FilterService.FilterType.QarRatings);
            Assert.That(clearLinks, Is.Not.Null);
            Assert.That(clearLinks.Items, Has.Count.EqualTo(_viewModel.SelectedQarRatings.Count));

            var urlWithoutQarRatingsExcellent = _fullQueryString.Replace("&qarratings=Excellent", "");
            var urlWithoutQarRatingsVeryPoor = _fullQueryString.Replace("&qarratings=VeryPoor", "");

            Assert.Multiple(() =>
            {
                var qarRatingsExcellentLink =
                    clearLinks.Items.First(a => a.DisplayText == QarRating.Excellent.GetDescription());
                Assert.That(qarRatingsExcellentLink, Is.Not.Null);
                Assert.That(qarRatingsExcellentLink.ClearLink, Is.EqualTo(urlWithoutQarRatingsExcellent));

                var qarRatingsVeryPoorLink =
                    clearLinks.Items.First(a => a.DisplayText == QarRating.VeryPoor.GetDescription());
                Assert.That(qarRatingsVeryPoorLink, Is.Not.Null);
                Assert.That(qarRatingsVeryPoorLink.ClearLink, Is.EqualTo(urlWithoutQarRatingsVeryPoor));
            });
        });
    }
}
