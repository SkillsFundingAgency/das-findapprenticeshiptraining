using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Web.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Models.Shared;
using SFA.DAS.FAT.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests;
public class WhenBuildingPaginationViewModelTests
{
    private readonly Mock<IUrlHelper> _urlHelperMock = new();

    [SetUp]
    public void Setup()
    {
        _urlHelperMock
            .Setup(x =>
                x.RouteUrl(
                    It.Is<UrlRouteContext>(c =>
                        c.RouteName!.Equals(RouteNames.CourseProviders)
                    )
                )
            )
            .Returns(TestConstants.DefaultUrl);
    }

    [Test]
    [MoqInlineAutoData(1)]
    [MoqInlineAutoData(10)]
    public void Then_Pagination_Single_Pages_Is_Set(
        int numberOfResults,
        [Frozen] Mock<FindApprenticeshipTrainingWeb> config,
        CourseProvidersRequest request)
    {
        //Act
        var vm = new CourseProvidersViewModel(config.Object)
        {
            Location = request.Location,
            Distance = request.Distance,
            SelectedDeliveryModes = request.DeliveryModes.Select(x => x.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(x => x.ToString()).ToList(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        vm.Pagination = new PaginationViewModel(
            1,
            numberOfResults,
            Constants.DefaultPageSize,
            _urlHelperMock.Object,
            RouteNames.CourseProviders,
            vm.ToQueryString()
        );

        //Assert
        using (new AssertionScope())
        {
            var sut = vm!.Pagination;
            sut.Should().NotBeNull();
            sut.Pages.Count.Should().Be(0);
            sut.PageNumber.Should().Be(1);
        }
    }

    [Test]
    [MoqInlineAutoData(11)]
    [MoqInlineAutoData(15)]
    [MoqInlineAutoData(20)]
    public void Then_Pagination_2_Pages_Page_1_Is_Set(
          int numberOfResults,
          CourseProvidersRequest request,
          [Frozen] Mock<FindApprenticeshipTrainingWeb> config)
    {
        //Act
        var vm = new CourseProvidersViewModel(config.Object)
        {
            Location = request.Location,
            Distance = request.Distance,
            SelectedDeliveryModes = request.DeliveryModes.Select(x => x.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(x => x.ToString()).ToList(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        vm.Pagination = new PaginationViewModel(
            1,
        numberOfResults,
           Constants.DefaultPageSize,
            _urlHelperMock.Object,
            RouteNames.CourseProviders,
            vm.ToQueryString()
            );


        var sut = vm!.Pagination;

        //Assert
        using (new AssertionScope())
        {

            sut.Should().NotBeNull();
            sut.Pages.Should().HaveCount(3);
            sut.Pages[0].Title.Should().Be("1");
            sut.Pages[0].HasLink.Should().BeFalse();
            sut.Pages[0].Url.Should().BeNull();
            sut.Pages[1].Title.Should().Be("2");
            sut.Pages[1].HasLink.Should().BeTrue();
            sut.Pages[1].Url.Should().Contain("PageNumber=2");
            sut.Pages[2].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[2].HasLink.Should().BeTrue();
            sut.Pages[2].Url.Should().Contain("PageNumber=2");

            var urlCheck = sut.Pages[2].Url;

            urlCheck.Should().Contain(TestConstants.DefaultUrl);
            urlCheck.Should().Contain($"OrderBy={request.OrderBy.ToString()}");
            urlCheck.Should().Contain($"Location={request.Location}");
            urlCheck.Should().Contain($"DeliveryModes={string.Join("&DeliveryModes=", request.DeliveryModes)}");
            urlCheck.Should().Contain($"EmployerProviderRatings={string.Join("&EmployerProviderRatings=", request.EmployerProviderRatings)}");
            urlCheck.Should().Contain($"ApprenticeProviderRatings={string.Join("&ApprenticeProviderRatings=", request.ApprenticeProviderRatings)}");
            urlCheck.Should().Contain($"QarRatings={string.Join("&QarRatings=", request.QarRatings)}");
        }
    }

    [Test]
    [MoqInlineAutoData(11)]
    [MoqInlineAutoData(15)]
    [MoqInlineAutoData(20)]
    public void Then_Pagination_2_Pages_Page_2_Is_Set(
     int numberOfResults,
     CourseProvidersRequest request,
     [Frozen] Mock<FindApprenticeshipTrainingWeb> config)
    {
        int pagesExpected = 3;

        var vm = new CourseProvidersViewModel(config.Object)
        {
            Location = request.Location,
            Distance = request.Distance,
            SelectedDeliveryModes = request.DeliveryModes.Select(x => x.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(x => x.ToString()).ToList(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        vm.Pagination = new PaginationViewModel(
            2,
            numberOfResults,
            Constants.DefaultPageSize,
            _urlHelperMock.Object,
            RouteNames.CourseProviders,
            vm.ToQueryString()
        );

        //Act
        var sut = vm!.Pagination;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            sut.Pages.Should().HaveCount(pagesExpected);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].HasLink.Should().BeTrue();
            sut.Pages[0].Url.Should().Contain("PageNumber=1");
            sut.Pages[1].Title.Should().Be("1");
            sut.Pages[1].HasLink.Should().BeTrue();
            sut.Pages[1].Url.Should().Contain("PageNumber=1");
            sut.Pages[2].Title.Should().Be("2");
            sut.Pages[2].HasLink.Should().BeFalse();
            sut.Pages[2].Url.Should().BeNull();
        }
    }

    [Test]
    [MoqInlineAutoData(71)]
    public void Then_Pagination_7_Pages_Page_5_Is_Set(
        int numberOfResults,
     CourseProvidersRequest request,
     [Frozen] Mock<FindApprenticeshipTrainingWeb> config)
    {
        int currentPage = 5;
        int pagesExpected = 8;

        var vm = new CourseProvidersViewModel(config.Object)
        {
            Location = request.Location,
            Distance = request.Distance,
            SelectedDeliveryModes = request.DeliveryModes.Select(x => x.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(x => x.ToString()).ToList(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        vm.Pagination = new PaginationViewModel(
            currentPage,
            numberOfResults,
            Constants.DefaultPageSize,
            _urlHelperMock.Object,
            RouteNames.CourseProviders,
            vm.ToQueryString()
        );

        var sut = vm!.Pagination;

        //Assert
        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            sut.Pages.Should().HaveCount(pagesExpected);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].HasLink.Should().BeTrue();
            sut.Pages[0].Url.Should().Contain("PageNumber=4");
            sut.Pages[1].Title.Should().Be("3");
            sut.Pages[1].HasLink.Should().BeTrue();
            sut.Pages[1].Url.Should().Contain("PageNumber=3");
            sut.Pages[2].Title.Should().Be("4");
            sut.Pages[2].HasLink.Should().BeTrue();
            sut.Pages[2].Url.Should().Contain("PageNumber=4");

            sut.Pages[3].Title.Should().Be("5");
            sut.Pages[3].HasLink.Should().BeFalse();
            sut.Pages[3].Url.Should().BeNull();

            sut.Pages[4].Title.Should().Be("6");
            sut.Pages[4].HasLink.Should().BeTrue();
            sut.Pages[4].Url.Should().Contain("PageNumber=6");
            sut.Pages[5].Title.Should().Be("7");
            sut.Pages[5].HasLink.Should().BeTrue();
            sut.Pages[5].Url.Should().Contain("PageNumber=7");
            sut.Pages[6].Title.Should().Be("8");
            sut.Pages[6].HasLink.Should().BeTrue();
            sut.Pages[6].Url.Should().Contain("PageNumber=8");
            sut.Pages[7].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[7].HasLink.Should().BeTrue();
            sut.Pages[7].Url.Should().Contain("PageNumber=6");
        }
    }

    [Test]
    [MoqInlineAutoData("Location 1", "5", true)]
    [MoqInlineAutoData("location 1", "100", true)]
    [MoqInlineAutoData("location 3", "All", true)]
    [MoqInlineAutoData(null, "5", false)]
    [MoqInlineAutoData(null, "100", false)]
    [MoqInlineAutoData(null, "All", false)]
    public void Then_Pagination_2_Pages_Page_1_Distance_Is_Set(
        string location,
        string distance,
        bool isDistanceInPaginationUrl,
        CourseProvidersRequest request,
        [Frozen] Mock<FindApprenticeshipTrainingWeb> config)
    {
        var numberOfResults = 11;
        request.Distance = distance;
        request.Location = location;

        //Act
        var vm = new CourseProvidersViewModel(config.Object)
        {
            Location = request.Location,
            Distance = request.Distance,
            SelectedDeliveryModes = request.DeliveryModes.Select(x => x.ToString()).ToList(),
            SelectedEmployerApprovalRatings = request.EmployerProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedApprenticeApprovalRatings = request.ApprenticeProviderRatings.Select(x => x.ToString()).ToList(),
            SelectedQarRatings = request.QarRatings.Select(x => x.ToString()).ToList(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        vm.Pagination = new PaginationViewModel(
            1,
        numberOfResults,
           Constants.DefaultPageSize,
            _urlHelperMock.Object,
            RouteNames.CourseProviders,
            vm.ToQueryString()
            );


        var sut = vm!.Pagination;

        //Assert
        using (new AssertionScope())
        {

            sut.Should().NotBeNull();
            sut.Pages.Should().HaveCount(3);
            sut.Pages[0].Title.Should().Be("1");
            sut.Pages[0].HasLink.Should().BeFalse();
            sut.Pages[0].Url.Should().BeNull();
            sut.Pages[1].Title.Should().Be("2");
            sut.Pages[1].HasLink.Should().BeTrue();
            sut.Pages[1].Url.Should().Contain("PageNumber=2");
            sut.Pages[2].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[2].HasLink.Should().BeTrue();
            sut.Pages[2].Url.Should().Contain("PageNumber=2");

            var urlCheck = sut.Pages[2].Url;

            urlCheck.Should().Contain(TestConstants.DefaultUrl);
            switch (isDistanceInPaginationUrl)
            {
                case true:
                    urlCheck.Should().Contain($"Distance={distance}");
                    break;
                default:
                    urlCheck.Should().NotContain($"Distance={distance}");
                    break;
            }

        }
    }
}
