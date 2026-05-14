using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models.CourseProviders;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

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

    [TestCase("2223", "2022")]
    [TestCase("2324", "2023")]
    [TestCase("2425", "2024")]
    public void QarPeriodStartYear_WithValidQarPeriod_ReturnsCorrectYear(string qarPeriod, string expected)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = qarPeriod
        };

        sut.QarPeriodStartYear.Should().Be(expected);
    }

    [TestCase("2223", "2023")]
    [TestCase("2324", "2024")]
    [TestCase("2425", "2025")]
    public void QarPeriodEndYear_WithValidQarPeriod_ReturnsCorrectYear(string qarPeriod, string expected)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = qarPeriod
        };

        sut.QarPeriodEndYear.Should().Be(expected);
    }

    [TestCase("2223", "2022")]
    [TestCase("2324", "2023")]
    [TestCase("2425", "2024")]
    public void ReviewPeriodStartYear_WithValidReviewPeriod_ReturnsCorrectYear(string reviewPeriod, string expected)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ReviewPeriod = reviewPeriod
        };

        sut.ReviewPeriodStartYear.Should().Be(expected);
    }

    [TestCase("2223", "2023")]
    [TestCase("2324", "2024")]
    [TestCase("2425", "2025")]
    public void ReviewPeriodEndYear_WithValidReviewPeriod_ReturnsCorrectYear(string reviewPeriod, string expected)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            ReviewPeriod = reviewPeriod
        };

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
            Distance = DistanceService.AcrossEnglandFilterValue
        };

        sut.TotalMessage.Should().Be(expectedMessage);
    }

    [RecursiveMoqInlineAutoData("123", "Coventry")]
    [RecursiveMoqInlineAutoData("456", "London")]
    [RecursiveMoqInlineAutoData("789", "")]
    public void GetHelpFindingCourseUrl_WithLarsCode_ReturnsCorrectUrl(string larsCode, string location)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = location
        };

        var result = sut.GetHelpFindingCourseUrl(larsCode);

        using (new AssertionScope())
        {
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(_config.EmployerAccountsUrl);

            // The redirectUri is URL-encoded, so decode it to check the values
            var decodedResult = Uri.UnescapeDataString(result);
            decodedResult.Should().Contain(_config.RequestApprenticeshipTrainingUrl);
            decodedResult.Should().Contain($"standardId={larsCode}");
            decodedResult.Should().Contain("requestType=CourseDetail");

            if (!string.IsNullOrEmpty(location))
            {
                decodedResult.Should().Contain($"location={location}");
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
        var sut = new CourseProvidersViewModel(_config);

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

    [TestCase(CourseType.ShortCourse, ProviderOrderBy.Distance, false)]
    [TestCase(CourseType.ShortCourse, ProviderOrderBy.AchievementRate, true)]
    [TestCase(CourseType.Apprenticeship, ProviderOrderBy.AchievementRate, false)]
    [TestCase(CourseType.Apprenticeship, ProviderOrderBy.Distance, true)]
    public void ToQueryString_WithCourseTypeAndOrderBy_IncludesOrderByWhenNotDefault(CourseType courseType, ProviderOrderBy orderBy, bool shouldIncludeOrderBy)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            OrderBy = orderBy,
            CourseType = courseType,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        if (shouldIncludeOrderBy)
        {
            result.Should().Contain(x => x.Item1 == "OrderBy" && x.Item2 == orderBy.ToString());
        }
        else
        {
            result.Should().NotContain(x => x.Item1 == "OrderBy");
        }
    }

    [TestCase(CourseType.Apprenticeship, ProviderOrderBy.EmployerProviderRating)]
    [TestCase(CourseType.ShortCourse, ProviderOrderBy.AchievementRate)]
    public void ToQueryString_WithMultipleFiltersAndNonDefaultOrderBy_IncludesSelectedFilters(CourseType courseType, ProviderOrderBy providerOrderBy)
    {
        var sut = new CourseProvidersViewModel(_config);

        sut.Location = "Manchester";
        sut.Distance = "15";
        sut.SelectedDeliveryModes = new List<string> { "Online", "Workplace" };
        sut.SelectedApprenticeApprovalRatings = new List<string> { "Good" };
        sut.OrderBy = providerOrderBy;
        sut.CourseType = courseType;
        sut.QarPeriod = "2223";
        sut.ReviewPeriod = "2324";

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "Location" && x.Item2 == "Manchester");
            result.Should().Contain(x => x.Item1 == "Distance" && x.Item2 == "15");
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "Online");
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "Workplace");
            result.Should().Contain(x => x.Item1 == "ApprenticeProviderRatings" && x.Item2 == "Good");
            result.Should().Contain(x => x.Item1 == "OrderBy" && x.Item2 == providerOrderBy.ToString());
        }
    }

    [TestCase(null, "London")]
    [TestCase("   ", "Birmingham")]
    [TestCase("", "Manchester")]
    public void ToQueryString_WithLocationAndInvalidDistance_IncludesAcrossEnglandFilter(string distance, string location)
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = location,
            Distance = distance,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().Contain(x => x.Item1 == "Location" && x.Item2 == location);
            result.Should().Contain(x => x.Item1 == "Distance" && x.Item2 == DistanceService.AcrossEnglandFilterValue);
        }
    }

    [Test]
    public void ToQueryString_WithEmptyFilterLists_DoesNotIncludeEmptyFilters()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Leeds",
            SelectedDeliveryModes = new List<string>(),
            SelectedEmployerApprovalRatings = new List<string>(),
            SelectedApprenticeApprovalRatings = new List<string>(),
            SelectedQarRatings = new List<string>(),
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Should().NotContain(x => x.Item1 == "DeliveryModes");
            result.Should().NotContain(x => x.Item1 == "EmployerProviderRatings");
            result.Should().NotContain(x => x.Item1 == "ApprenticeProviderRatings");
            result.Should().NotContain(x => x.Item1 == "QarRatings");
        }
    }

    [Test]
    public void ToQueryString_WithMultipleDeliveryModes_IncludesAllModes()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Cambridge",
            SelectedDeliveryModes = new List<string> { "Online", "Workplace", "Provider" },
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.ToQueryString();

        using (new AssertionScope())
        {
            result.Where(x => x.Item1 == "DeliveryModes").Should().HaveCount(3);
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "Online");
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "Workplace");
            result.Should().Contain(x => x.Item1 == "DeliveryModes" && x.Item2 == "Provider");
        }
    }

    [Test]
    public void Filters_PropertyAccessedTwice_ReturnsSameInstance()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var first = sut.Filters;
        var second = sut.Filters;

        first.Should().BeSameAs(second);
    }

    [Test]
    public void TotalMessage_LocationMissing_DoesNotIncludeWithinDistanceText()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            TotalCount = 3,
            Location = string.Empty,
            Distance = "10"
        };

        sut.TotalMessage.Should().Be("3 results");
    }

    [Test]
    public void GetHelpFindingCourseUrl_WhitespaceLocation_IncludesEncodedLocationParameter()
    {
        const string larsCode = "123";
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "   "
        };

        var result = sut.GetHelpFindingCourseUrl(larsCode);
        var decodedResult = Uri.UnescapeDataString(result);

        decodedResult.Should().Contain("location=   ");
    }

    [Test]
    public void CreateFilterSections_NoFiltersSelectedAndDistanceEmpty_CreatesNoClearFilterSections()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            CourseType = CourseType.Apprenticeship,
            OrderBy = ProviderOrderBy.AchievementRate,
            Location = null,
            Distance = " ",
            SelectedDeliveryModes = [],
            SelectedEmployerApprovalRatings = [],
            SelectedApprenticeApprovalRatings = [],
            SelectedQarRatings = [],
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.CreateFilterSections();

        result.ClearFilterSections.Should().BeEmpty();
    }

    [Test]
    public void CreateFilterSections_LocationAndDistanceSelected_IncludesLocationClearFilterSection()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            Location = "Leeds",
            Distance = "10",
            CourseType = CourseType.Apprenticeship,
            OrderBy = ProviderOrderBy.AchievementRate,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.CreateFilterSections();

        using (new AssertionScope())
        {
            result.ClearFilterSections.Should().ContainSingle(s => s.FilterType == FilterService.FilterType.Location);
            var locationSection = result.ClearFilterSections.Single(s => s.FilterType == FilterService.FilterType.Location);
            locationSection.Items.Should().ContainSingle();
            locationSection.Items[0].DisplayText.Should().Be("Leeds (within 10 miles)");
        }
    }

    [Test]
    public void CreateFilterSections_SelectedDeliveryModesForShortCourse_IncludesDeliveryModeDisplayText()
    {
        var sut = new CourseProvidersViewModel(_config)
        {
            CourseType = CourseType.ShortCourse,
            SelectedDeliveryModes = new[] { ProviderDeliveryMode.Workplace.ToString() },
            OrderBy = ProviderOrderBy.Distance,
            QarPeriod = "2223",
            ReviewPeriod = "2324"
        };

        var result = sut.CreateFilterSections();

        using (new AssertionScope())
        {
            result.ClearFilterSections.Should().ContainSingle(s => s.FilterType == FilterService.FilterType.DeliveryModes);
            var section = result.ClearFilterSections.Single(s => s.FilterType == FilterService.FilterType.DeliveryModes);
            section.Items.Should().ContainSingle(i => i.DisplayText == "At learner's workplace");
        }
    }
}
