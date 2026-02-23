using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;

namespace SFA.DAS.FAT.Web.UnitTests.Models.CourseProvidersViewModelTests;

public class WhenCreatingCourseProvidersViewModel
{
    private FindApprenticeshipTrainingWeb _config;

    [SetUp]
    public void Setup()
    {
        _config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://request.test.com",
            EmployerAccountsUrl = "https://accounts.test.com"
        };
    }

    [TestCase("2223")]
    [TestCase("2324")]
    [TestCase("2425")]
    public void QarPeriodStartYear_WithValidQarPeriod_ReturnsCorrectYear(string qarPeriod)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = qarPeriod
        };

        var expected = $"20{qarPeriod.Substring(0, 2)}";
        sut.QarPeriodStartYear.Should().Be(expected);
    }

    [TestCase("2223")]
    [TestCase("2324")]
    [TestCase("2425")]
    public void QarPeriodEndYear_WithValidQarPeriod_ReturnsCorrectYear(string qarPeriod)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = qarPeriod
        };

        var expected = $"20{qarPeriod.Substring(2, 2)}";
        sut.QarPeriodEndYear.Should().Be(expected);
    }

    [TestCase("2223")]
    [TestCase("2324")]
    [TestCase("2425")]
    public void ReviewPeriodStartYear_WithValidReviewPeriod_ReturnsCorrectYear(string reviewPeriod)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ReviewPeriod = reviewPeriod
        };

        var expected = $"20{reviewPeriod.Substring(0, 2)}";
        sut.ReviewPeriodStartYear.Should().Be(expected);
    }

    [TestCase("2223")]
    [TestCase("2324")]
    [TestCase("2425")]
    public void ReviewPeriodEndYear_WithValidReviewPeriod_ReturnsCorrectYear(string reviewPeriod)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ReviewPeriod = reviewPeriod
        };

        var expected = $"20{reviewPeriod.Substring(2, 2)}";
        sut.ReviewPeriodEndYear.Should().Be(expected);
    }

    [TestCase("2223", "2022", "2023")]
    [TestCase("2324", "2023", "2024")]
    [TestCase("2425", "2024", "2025")]
    public void ProviderReviewsHeading_WithValidReviewPeriod_ReturnsCorrectHeading(string reviewPeriod, string startYear, string endYear)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ReviewPeriod = reviewPeriod
        };

        sut.ProviderReviewsHeading.Should().Be($"Provider reviews in {startYear} to {endYear}");
    }

    [TestCase("2223", "2022", "2023")]
    [TestCase("2324", "2023", "2024")]
    [TestCase("2425", "2024", "2025")]
    public void CourseAchievementRateHeading_WithValidQarPeriod_ReturnsCorrectHeading(string qarPeriod, string startYear, string endYear)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = qarPeriod
        };

        sut.CourseAchievementRateHeading.Should().Be($"Course achievement rate in {startYear} to {endYear}");
    }

    [TestCase(0, "No results")]
    [TestCase(1, "1 result")]
    [TestCase(2, "2 results")]
    [TestCase(10, "10 results")]
    [TestCase(-1, "No results")]
    public void TotalMessage_WithVariousTotalCounts_ReturnsCorrectMessage(int totalCount, string expectedMessage)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            TotalCount = totalCount
        };

        sut.TotalMessage.Should().Be(expectedMessage);
    }

    [TestCase(1, "Coventry", "10", "1 result within 10 miles")]
    [TestCase(2, "Coventry", "20", "2 results within 20 miles")]
    [TestCase(5, "London", "5", "5 results within 5 miles")]
    [TestCase(10, "Manchester", "50", "10 results within 50 miles")]
    public void TotalMessage_WithLocationAndDistance_IncludesDistanceInMessage(int totalCount, string location, string distance, string expectedMessage)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            TotalCount = totalCount,
            Location = location,
            Distance = distance
        };

        sut.TotalMessage.Should().Be(expectedMessage);
    }

    [TestCase(1, "Coventry", "1 result")]
    [TestCase(2, "London", "2 results")]
    public void TotalMessage_WithLocationAndAcrossEnglandFilter_DoesNotIncludeDistance(int totalCount, string location, string expectedMessage)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            TotalCount = totalCount,
            Location = location,
            Distance = DistanceService.ACROSS_ENGLAND_FILTER_VALUE
        };

        sut.TotalMessage.Should().Be(expectedMessage);
    }

    [TestCase("123", "Coventry")]
    [TestCase("456", "London")]
    [TestCase("789", "")]
    public void GetHelpFindingCourseUrl_WithLarsCode_ReturnsCorrectUrl(string larsCode, string location)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = location
        };

        var result = sut.GetHelpFindingCourseUrl(larsCode);

        using (new AssertionScope())
        {
            result.Should().Contain(_config.EmployerAccountsUrl);

            var uri = new Uri(result);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var redirectUri = queryParams["redirectUri"];
            redirectUri.Should().NotBeNullOrEmpty();

            var decodedRedirectUri = Uri.UnescapeDataString(redirectUri);
            decodedRedirectUri.Should().Contain(_config.RequestApprenticeshipTrainingUrl);
            decodedRedirectUri.Should().Contain($"standardId={larsCode}");
            decodedRedirectUri.Should().Contain("requestType=CourseDetail");

            if (!string.IsNullOrEmpty(location))
            {
                decodedRedirectUri.Should().Contain($"location={location}");
            }
        }
    }

    [Test]
    public void ToQueryString_WithNoFilters_ReturnsEmptyList()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            OrderBy = ProviderOrderBy.AchievementRate,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        result.Should().BeEmpty();
    }

    [Test]
    public void ToQueryString_WithLocationFilter_IncludesLocationAndDistance()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            Distance = "10",
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "Location" && x.Item2 == "Coventry");
            result.Should().Contain(x => x.Item1 == "Distance" && x.Item2 == "10");
        }
    }

    [Test]
    public void ToQueryString_WithLocationAndNoDistance_IncludesAcrossEnglandFilter()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            Distance = "",
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "Location" && x.Item2 == "Coventry");
            result.Should().Contain(x => x.Item1 == "Distance" && x.Item2 == DistanceService.ACROSS_ENGLAND_FILTER_VALUE);
        }
    }

    [Test]
    public void ToQueryString_WithDeliveryModes_IncludesAllSelectedModes()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            SelectedDeliveryModes = new List<string> { "DayRelease", "BlockRelease" },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "DayRelease");
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "BlockRelease");
        }
    }

    [Test]
    public void ToQueryString_WithEmployerRatings_IncludesAllSelectedRatings()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            SelectedEmployerApprovalRatings = new List<string> { "Excellent", "Good" },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "EmployerProviderRatings" && x.Item2 == "Excellent");
            result.Should().Contain(x => x.Item1 == "EmployerProviderRatings" && x.Item2 == "Good");
        }
    }

    [Test]
    public void ToQueryString_WithApprenticeRatings_IncludesAllSelectedRatings()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            SelectedApprenticeApprovalRatings = new List<string> { "Excellent", "Good" },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "ApprenticeProviderRatings" && x.Item2 == "Excellent");
            result.Should().Contain(x => x.Item1 == "ApprenticeProviderRatings" && x.Item2 == "Good");
        }
    }

    [Test]
    public void ToQueryString_WithQarRatings_IncludesAllSelectedRatings()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Coventry",
            SelectedQarRatings = new List<string> { "Excellent", "Good" },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "QarRatings" && x.Item2 == "Excellent");
            result.Should().Contain(x => x.Item1 == "QarRatings" && x.Item2 == "Good");
        }
    }

    [TestCase(ProviderOrderBy.Distance)]
    [TestCase(ProviderOrderBy.EmployerProviderRating)]
    [TestCase(ProviderOrderBy.ApprenticeProviderRating)]
    public void ToQueryString_WithNonDefaultOrderBy_IncludesOrderBy(ProviderOrderBy orderBy)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            OrderBy = orderBy,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        result.Should().Contain(x => x.Item1 == "OrderBy" && x.Item2 == orderBy.ToString());
    }

    [Test]
    public void ToQueryString_WithDefaultOrderBy_DoesNotIncludeOrderBy()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            OrderBy = ProviderOrderBy.AchievementRate,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        result.Should().NotContain(x => x.Item1 == "OrderBy");
    }

    [Test]
    public void Constructor_WithValidConfiguration_InitializesCorrectly()
    {
        var config = new FindApprenticeshipTrainingWeb
        {
            RequestApprenticeshipTrainingUrl = "https://test.com",
            EmployerAccountsUrl = "https://accounts.com"
        };

        var sut = new CourseProvidersViewModel(config);

        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            sut.SelectedDeliveryModes.Should().NotBeNull().And.BeEmpty();
            sut.SelectedEmployerApprovalRatings.Should().NotBeNull().And.BeEmpty();
            sut.SelectedApprenticeApprovalRatings.Should().NotBeNull().And.BeEmpty();
            sut.SelectedQarRatings.Should().NotBeNull().And.BeEmpty();
            sut.ProviderOrderOptions.Should().NotBeNull().And.BeEmpty();
        }
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(100)]
    public void ShortlistCount_WhenSet_StoresCorrectValue(int count)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ShortlistCount = count
        };

        sut.ShortlistCount.Should().Be(count);
    }

    [TestCase("Software Developer Level 3")]
    [TestCase("Data Analyst Level 4")]
    public void CourseTitleAndLevel_WhenSet_StoresCorrectValue(string title)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            CourseTitleAndLevel = title
        };

        sut.CourseTitleAndLevel.Should().Be(title);
    }
}
