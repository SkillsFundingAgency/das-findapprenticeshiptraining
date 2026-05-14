using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingCourseProviderViewModel
{
    [Test, MoqAutoData]
    public void ImplicitOperator_FromGetCourseProviderQueryResult_MapsPropertiesCorrectly(GetCourseProviderQueryResult source)
    {
        source.AnnualEmployerFeedbackDetails = new List<EmployerFeedbackAnnualSummaries>();
        source.AnnualApprenticeFeedbackDetails = new List<ApprenticeFeedbackAnnualSummaries>();
        var sut = (CourseProviderViewModel)source;

        var expectedCoursesAlphabetically = source.Courses.ToList().OrderBy(c => c.CourseName).ThenBy(c => c.Level);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Ukprn, Is.EqualTo(source.Ukprn));
            Assert.That(sut.ProviderName, Is.EqualTo(source.ProviderName));
            Assert.That(sut.ProviderAddress, Is.EqualTo(source.ProviderAddress));
            Assert.That(sut.Contact, Is.EqualTo(source.Contact));
            Assert.That(sut.CourseName, Is.EqualTo(source.CourseName));
            Assert.That(sut.Level, Is.EqualTo(source.Level));
            Assert.That(sut.LarsCode, Is.EqualTo(source.LarsCode));
            Assert.That(sut.IFateReferenceNumber, Is.EqualTo(source.IFateReferenceNumber));
            Assert.That(sut.Qar, Is.EqualTo(source.Qar));
            Assert.That(sut.Reviews, Is.EqualTo(source.Reviews));
            Assert.That(sut.EndpointAssessments, Is.EqualTo(source.EndpointAssessments));
            Assert.That(sut.TotalProvidersCount, Is.EqualTo(source.TotalProvidersCount));
            Assert.That(sut.ShortlistId, Is.EqualTo(source.ShortlistId));
            Assert.That(sut.Locations, Is.EqualTo(source.Locations));
            Assert.That(sut.Courses, Is.EqualTo(expectedCoursesAlphabetically));
        }
    }

    [Test]
    public void TrainingOptions_WhenAccessedForApprenticeship_MapsReleaseAndLearnerWorkplaceValues()
    {
        var closestBlockReleaseLocation = new LocationModel
        {
            LocationType = LocationType.Provider,
            BlockRelease = true,
            CourseDistance = 1.44
        };

        var closestDayReleaseLocation = new LocationModel
        {
            LocationType = LocationType.Provider,
            DayRelease = true,
            CourseDistance = 2.55
        };

        var sut = new CourseProviderViewModel
        {
            CourseType = CourseType.Apprenticeship,
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Regional, AtEmployer = true },
                closestBlockReleaseLocation,
                new LocationModel { LocationType = LocationType.Provider, BlockRelease = true, CourseDistance = 5.1 },
                closestDayReleaseLocation,
                new LocationModel { LocationType = LocationType.Provider, DayRelease = true, CourseDistance = 7.7 }
            }
        };

        var result = sut.TrainingOptions;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ShowOnlineOption, Is.False);
            Assert.That(result.ShowProviderOption, Is.False);
            Assert.That(result.ShowLearnerWorkplaceOption, Is.True);
            Assert.That(result.AtLearnerWorkplaceWithNoLocationDisplayMessage, Is.EqualTo(CourseProviderViewModel.AtLearnerWorkplaceWithNoLocationRegional));
            Assert.That(result.HasMatchingRegionalLocationOrNational, Is.True);
            Assert.That(result.ShowBlockReleaseOption, Is.True);
            Assert.That(result.BlockReleaseLocations, Has.Count.EqualTo(2));
            Assert.That(result.HasMultipleBlockReleaseLocations, Is.True);
            Assert.That(result.ClosestBlockReleaseLocation, Is.EqualTo(closestBlockReleaseLocation));
            Assert.That(result.ClosestBlockReleaseLocationDistanceDisplay, Is.EqualTo("1.4"));
            Assert.That(result.ShowDayReleaseOption, Is.True);
            Assert.That(result.DayReleaseLocations, Has.Count.EqualTo(2));
            Assert.That(result.HasMultipleDayReleaseLocations, Is.True);
            Assert.That(result.ClosestDayReleaseLocation, Is.EqualTo(closestDayReleaseLocation));
            Assert.That(result.ClosestDayReleaseLocationDistanceDisplay, Is.EqualTo("2.6"));
            Assert.That(result.ClosestProviderLocationAddress, Is.Empty);
        }
    }

    [Test]
    public void ShowMultipleProvidersForCourse_WhenTotalProvidersCountIsGreaterThanOne_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            TotalProvidersCount = 2
        };

        Assert.That(sut.ShowMultipleProvidersForCourse, Is.True);
    }

    [Test]
    public void ShowMultipleProvidersForCourse_WhenTotalProvidersCountIsOne_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            TotalProvidersCount = 1
        };

        Assert.That(sut.ShowMultipleProvidersForCourse, Is.False);
    }

    [Test]
    public void EndpointAssessmentDisplayMessage_WhenEndpointAssessmentsIsNull_ReturnsNotEnoughApprenticesMessage()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = null
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("Not enough apprentices have completed a course with this provider to show participation"));
    }

    [Test]
    public void EndpointAssessmentDisplayMessage_WhenEarliestAssessmentIsNull_ReturnsNotEnoughApprenticesMessage()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(null, 0)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("Not enough apprentices have completed a course with this provider to show participation"));
    }

    [Test]
    public void EndpointAssessmentDisplayMessage_WhenAssessmentCountIsZero_ReturnsMessage()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentices have completed a course and taken their end-point assessment with this provider."));
    }

    [Test]
    public void EndpointAssessmentDisplayMessage_WhenAssessmentCountGreaterThanOne_ReturnsMessageWithYearAndPlural()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2019, 5, 1, 0, 0, 0, DateTimeKind.Utc), 5)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentices have completed this course and taken their end-point assessment with this provider since 2019"));
    }

    [Test]
    public void EndpointAssessmentDisplayMessage_WhenAssessmentCountIsOne_ReturnsMessageWithYearAndSingular()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2021, 3, 1, 0, 0, 0, DateTimeKind.Utc), 1)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentice have completed this course and taken their end-point assessment with this provider since 2021"));
    }

    [Test]
    public void ContactAddress_WithNonEmptyProperties_CombinesWithCommas()
    {
        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel
            {
                AddressLine1 = "123 Main St",
                AddressLine2 = "Suite 400",
                AddressLine3 = "",
                AddressLine4 = null,
                Town = "Belfast",
                Postcode = "BT1 2AB"
            }
        };

        Assert.That(sut.ContactAddress, Is.EqualTo("123 Main St, Suite 400, Belfast, BT1 2AB"));
    }

    [Test]
    public void ContactAddress_WhenAllPartsAreEmpty_ReturnsEmpty()
    {
        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel
            {
                AddressLine1 = "   ",
                AddressLine2 = "",
                AddressLine3 = null,
                AddressLine4 = null,
                Town = null,
                Postcode = ""
            }
        };

        Assert.That(sut.ContactAddress, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ContactAddress_WithUntrimmedValues_TrimsValues()
    {
        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel
            {
                AddressLine1 = " 123 Main St ",
                Town = "  Belfast ",
                Postcode = " BT1 2AB "
            }
        };

        Assert.That(sut.ContactAddress, Is.EqualTo("123 Main St, Belfast, BT1 2AB"));
    }

    [Test]
    public void BlockReleaseLocations_WhenBlockReleaseLocationsExist_ReturnsFormattedAddresses()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Building 4",
                    Town = "Belfast",
                    County = "Antrim",
                    Postcode = "BT1 1AA",
                    BlockRelease = true,
                    DayRelease = false
                },
                new LocationModel
                {
                    AddressLine1 = "456 Side St",
                    Town = "Derry",
                    Postcode = "BT2 2BB",
                    BlockRelease = true,
                    DayRelease = false
                },
                new LocationModel
                {
                    AddressLine1 = "789 Side St",
                    BlockRelease = false,
                    DayRelease = true
                }
            }
        };

        var result = sut.BlockReleaseLocations;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].FormatAddress(), Is.EqualTo("123 Main St, Building 4, Belfast, Antrim, BT1 1AA"));
            Assert.That(result[1].FormatAddress(), Is.EqualTo("456 Side St, Derry, BT2 2BB"));
        }
    }

    [Test]
    public void DayReleaseLocations_WhenDayReleaseLocationsExist_ReturnsFormattedAddresses()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel
                {
                    AddressLine1 = "Day Site 1",
                    Town = "Newry",
                    Postcode = "BT3 3CC",
                    DayRelease = true
                },
                new LocationModel
                {
                    AddressLine1 = "Block Site",
                    Town = "Armagh",
                    Postcode = "BT4 4DD",
                    BlockRelease = true,
                    DayRelease = false
                }
            }
        };

        var result = sut.DayReleaseLocations;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].FormatAddress(), Is.EqualTo("Day Site 1, Newry, BT3 3CC"));
        }
    }

    [Test]
    public void BlockReleaseLocations_WhenNoBlockReleaseLocationsExist_ReturnsEmptyList()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AddressLine1 = "None", DayRelease = true, BlockRelease = false }
            }
        };

        Assert.That(sut.BlockReleaseLocations, Is.Empty);
    }

    [Test]
    public void DayReleaseLocations_WhenNoDayReleaseLocationsExist_ReturnsEmptyList()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AddressLine1 = "None", BlockRelease = true, DayRelease = false }
            }
        };

        Assert.That(sut.DayReleaseLocations, Is.Empty);
    }

    [Test]
    public void AtLearnerWorkplaceWithNoLocationDisplayMessage_WhenLocationTypeIsNational_ReturnsNationalMessage()
    {
        var sut = new CourseProviderViewModel
        {
            Location = string.Empty,
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = true, LocationType = LocationType.National }
            }
        };

        Assert.That(sut.AtLearnerWorkplaceWithNoLocationDisplayMessage, Is.EqualTo(CourseProviderViewModel.AtLearnerWorkplaceWithNoLocationNational));
    }

    [Test]
    public void AtLearnerWorkplaceWithNoLocationDisplayMessage_WhenLocationTypeIsRegional_ReturnsRegionalMessage()
    {
        var sut = new CourseProviderViewModel
        {
            Location = string.Empty,
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Regional }
            }
        };

        Assert.That(sut.AtLearnerWorkplaceWithNoLocationDisplayMessage, Is.EqualTo(CourseProviderViewModel.AtLearnerWorkplaceWithNoLocationRegional));
    }

    [Test]
    public void AtLearnerWorkplaceWithNoLocationDisplayMessage_WhenNoLocationIsFound_ReturnsNationalMessage()
    {
        var sut = new CourseProviderViewModel
        {
            Location = string.Empty,
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.National }
            }
        };

        Assert.That(sut.AtLearnerWorkplaceWithNoLocationDisplayMessage, Is.EqualTo(CourseProviderViewModel.AtLearnerWorkplaceWithNoLocationNational));
    }

    [Test]
    public void AchievementRateInformation_WhenAchievementRateIsNotNull_ReturnsFormattedMessage()
    {
        var sut = new CourseProviderViewModel
        {
            Qar = new QarModel
            {
                AchievementRate = "90",
                Leavers = "1001001",
                Period = "2122",
                NationalLeavers = "120",
                NationalAchievementRate = "20"
            }
        };

        var result = sut.AchievementRateInformation;

        var expected = "of apprentices (900,901 of 1,001,001) completed this course and passed their end-point assessment with this provider in academic year 2021 to 2022. 10% did not pass or left the course before taking the assessment.";

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void AchievementRateInformation_WhenAchievementRateIsNull_ReturnsEmptyString()
    {
        var sut = new CourseProviderViewModel
        {
            Qar = new QarModel
            {
                AchievementRate = null
            }
        };

        Assert.That(sut.AchievementRateInformation, Is.EqualTo(string.Empty));
    }

    [TestCase(0, "View 0 courses delivered by this training provider")]
    [TestCase(1, "View 1 course delivered by this training provider")]
    [TestCase(3, "View 3 courses delivered by this training provider")]
    public void CoursesDeliveredCountDisplay_WithCourseCount_ReturnsCorrectMessage(int courseCount, string expected)
    {
        var sut = new CourseProviderViewModel
        {
            Courses = Enumerable.Range(1, courseCount)
                                .Select(i => new ProviderCourseModel { LarsCode = i.ToString() })
                                .ToList()
        };

        Assert.That(sut.CoursesDeliveredCountDisplay, Is.EqualTo(expected));
    }

    [TestCase("1", "average review from 1 employer when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("5", "average review from 5 employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    public void EmployerReviewsDisplayMessage_WithValidEmployerReviews_ReturnsCorrectMessage(string reviewCount, string expected)
    {
        var sut = new CourseProviderViewModel
        {
            Reviews = new ReviewsModel
            {
                EmployerReviews = reviewCount
            }
        };

        Assert.That(sut.EmployerReviewsDisplayMessage, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("abc")]
    public void EmployerReviewsDisplayMessage_WhenEmployerReviewsCannotBeParsed_ReturnsEmpty(string invalidReviewValue)
    {
        var sut = new CourseProviderViewModel
        {
            Reviews = new ReviewsModel
            {
                EmployerReviews = invalidReviewValue
            }
        };

        Assert.That(sut.EmployerReviewsDisplayMessage, Is.EqualTo(string.Empty));
    }

    [TestCase("1", "average review from 1 apprentice when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("5", "average review from 5 apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    public void ApprenticeReviewsDisplayMessage_WithValidApprenticeReviews_ReturnsCorrectMessage(string reviewCount, string expected)
    {
        var sut = new CourseProviderViewModel
        {
            Reviews = new ReviewsModel
            {
                ApprenticeReviews = reviewCount
            }
        };

        Assert.That(sut.ApprenticeReviewsDisplayMessage, Is.EqualTo(expected));
    }

    [TestCase(null)]
    [TestCase("ds")]
    [TestCase("dsdsd")]
    public void ApprenticeReviewsDisplayMessage_WhenApprenticeReviewsCannotBeParsed_ReturnsEmpty(string invalidReviewValue)
    {
        var sut = new CourseProviderViewModel
        {
            Reviews = new ReviewsModel
            {
                ApprenticeReviews = invalidReviewValue
            }
        };

        Assert.That(sut.ApprenticeReviewsDisplayMessage, Is.EqualTo(string.Empty));
    }

    [Test]
    public void EndpointAssessmentsCountDisplay_WhenEndpointAssessmentsIsNull_ReturnsNoData()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = null
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo("No data"));
    }

    [Test]
    public void EndpointAssessmentsCountDisplay_WhenEarliestAssessmentIsNull_ReturnsNoData()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(null, 5)
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo("No data"));
    }

    [TestCase(8, "8")]
    [TestCase(80, "80")]
    [TestCase(800, "800")]
    [TestCase(8000, "8,000")]
    [TestCase(80000, "80,000")]
    [TestCase(800000, "800,000")]
    [TestCase(8000000, "8,000,000")]
    public void EndpointAssessmentsCountDisplay_WhenValid_ReturnsCountAsString(int count, string expectedCountDisplay)
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2022, 9, 1, 0, 0, 0, DateTimeKind.Utc), count)
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo(expectedCountDisplay));
    }

    [Test]
    public void HasMultipleBlockReleaseLocations_WhenMoreThanOne_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = true },
                new LocationModel { BlockRelease = true },
                new LocationModel { BlockRelease = false }
            }
        };

        Assert.That(sut.HasMultipleBlockReleaseLocations, Is.True);
    }

    [Test]
    public void HasMultipleBlockReleaseLocations_WhenOneOrLess_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = true },
                new LocationModel { BlockRelease = false }
            }
        };

        Assert.That(sut.HasMultipleBlockReleaseLocations, Is.False);
    }

    [Test]
    public void HasMultipleDayReleaseLocations_WhenMoreThanOne_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true },
                new LocationModel { DayRelease = true },
                new LocationModel { DayRelease = false }
            }
        };

        Assert.That(sut.HasMultipleDayReleaseLocations, Is.True);
    }

    [Test]
    public void HasMultipleDayReleaseLocations_WhenOneOrLess_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true },
                new LocationModel { DayRelease = false }
            }
        };

        Assert.That(sut.HasMultipleDayReleaseLocations, Is.False);
    }

    [Test]
    public void CourseNameAndLevel_WithCourseNameAndLevel_CombinesBoth()
    {
        var sut = new CourseProviderViewModel
        {
            CourseName = "Software Developer",
            Level = 4
        };

        Assert.That(sut.CourseNameAndLevel, Is.EqualTo("Software Developer (level 4)"));
    }

    [Test]
    public void ShowApprenticesWorkplaceOption_WhenAnyLocationTypeNationalExists_ReturnsTrue()
    {
        var viewModel = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Provider },
                new LocationModel { LocationType = LocationType.National }
            }
        };

        Assert.That(viewModel.ShowLearnerWorkplaceOption, Is.True);
    }

    [Test]
    public void ShowLearnerWorkplaceOption_WhenAnyLocationTypeRegionalExists_ReturnsTrue()
    {
        var viewModel = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Provider },
                new LocationModel { LocationType = LocationType.Regional }
            }
        };

        Assert.That(viewModel.ShowLearnerWorkplaceOption, Is.True);
    }

    [Test]
    public void ShowLearnerWorkplaceOption_WhenNoNationalOrRegionalLocations_ReturnsFalse()
    {
        var viewModel = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Provider }
            }
        };

        Assert.That(viewModel.ShowLearnerWorkplaceOption, Is.False);
    }

    [Test]
    public void ShowBlockReleaseOption_WhenAnyBlockReleaseIsTrue_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = false },
                new LocationModel { BlockRelease = true }
            }
        };

        Assert.That(sut.ShowBlockReleaseOption, Is.True);
    }

    [Test]
    public void ShowBlockReleaseOption_WhenNoneBlockRelease_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = false }
            }
        };

        Assert.That(sut.ShowBlockReleaseOption, Is.False);
    }

    [Test]
    public void ShowDayReleaseOption_WhenAnyDayReleaseIsTrue_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true }
            }
        };

        Assert.That(sut.ShowDayReleaseOption, Is.True);
    }

    [Test]
    public void ShowDayReleaseOption_WhenNoDayRelease_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = false }
            }
        };

        Assert.That(sut.ShowDayReleaseOption, Is.False);
    }

    [Test]
    public void ShortlistClass_WhenUnderMaximumAndShortlistIdPopulated_ReturnsAdded()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount - 1,
            ShortlistId = Guid.NewGuid()
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-added"));
    }

    [Test]
    public void ShortlistClass_WhenEqualToMaximumAndShortlistIdNull_ReturnsFull()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount,
            ShortlistId = null
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-full"));
    }

    [Test]
    public void ShortlistClass_WhenShortlistIdNullAndUnderMaximum_ReturnsDefault()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount - 1,
            ShortlistId = null
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ShortlistClass_WhenAboveMaximum_ReturnsFull()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount + 1
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-full"));
    }

    [Test]
    public void NationalLocation_WhenMatchingLocationExists_ReturnsFirstMatchingEmployerWithNationalLocationType()
    {
        var national = new LocationModel { AtEmployer = true, LocationType = LocationType.National };

        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = true, LocationType = LocationType.Regional },
                national,
                new LocationModel { AtEmployer = false, LocationType = LocationType.National }
            }
        };

        Assert.That(sut.NationalLocation, Is.EqualTo(national));
    }

    [Test]
    public void NationalLocation_WhenNoMatchingLocation_ReturnsNull()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = true, LocationType = LocationType.Regional },
                new LocationModel { AtEmployer = false, LocationType = LocationType.National }
            }
        };

        Assert.That(sut.NationalLocation, Is.Null);
    }

    [Test]
    public void NationalLocation_WhenLocationsIsEmpty_ReturnsNull()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>()
        };

        Assert.That(sut.NationalLocation, Is.Null);
    }

    [Test]
    public void ClosestBlockReleaseLocation_WhenMultipleLocationsExist_ReturnsLocationWithSmallestCourseDistance()
    {
        var closest = new LocationModel { BlockRelease = true, CourseDistance = 5.0 };
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = true, CourseDistance = 15.0 },
                closest,
                new LocationModel { BlockRelease = true, CourseDistance = 10.0 }
            }
        };

        Assert.That(sut.ClosestBlockReleaseLocation, Is.EqualTo(closest));
    }

    [Test]
    public void ClosestBlockReleaseLocation_WhenNoneAreBlockRelease_ReturnsNull()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = false, CourseDistance = 5.0 },
                new LocationModel { BlockRelease = false, CourseDistance = 10.0 }
            }
        };

        Assert.That(sut.ClosestBlockReleaseLocation, Is.Null);
    }

    [Test]
    public void ClosestDayReleaseLocation_WhenMultipleLocationsExist_ReturnsLocationWithSmallestCourseDistance()
    {
        var closest = new LocationModel { DayRelease = true, CourseDistance = 3.4 };
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true, CourseDistance = 8.1 },
                closest,
                new LocationModel { DayRelease = true, CourseDistance = 6.6 }
            }
        };

        Assert.That(sut.ClosestDayReleaseLocation, Is.EqualTo(closest));
    }

    [Test]
    public void ClosestDayReleaseLocation_WhenNoneAreDayRelease_ReturnsNull()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = false, CourseDistance = 1.2 },
                new LocationModel { DayRelease = false, CourseDistance = 2.5 }
            }
        };

        Assert.That(sut.ClosestDayReleaseLocation, Is.Null);
    }

    [TestCase(LocationType.National, false, true)]
    [TestCase(LocationType.Regional, true, true)]
    [TestCase(LocationType.Regional, false, false)]
    [TestCase(LocationType.Provider, true, false)]
    public void HasMatchingRegionalLocationOrNational_LocationTypeAndAtEmployerVaries_ReturnsExpected(LocationType locationType, bool atEmployer, bool expected)
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = locationType, AtEmployer = atEmployer }
            }
        };

        Assert.That(sut.HasMatchingRegionalLocationOrNational, Is.EqualTo(expected));
    }

    [TestCase(CourseType.ShortCourse, true, false)]
    [TestCase(CourseType.Apprenticeship, false, true)]
    public void IsShortCourseAndIsApprenticeship_AreDerivedFromCourseType(CourseType courseType, bool isShortCourse, bool isApprenticeship)
    {
        var sut = new CourseProviderViewModel { CourseType = courseType };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.IsShortCourse, Is.EqualTo(isShortCourse));
            Assert.That(sut.IsApprenticeship, Is.EqualTo(isApprenticeship));
        }
    }

    [Test]
    public void ClosestProviderLocation_ReturnsLocationWithSmallestCourseDistance()
    {
        var closest = new LocationModel { LocationType = LocationType.Provider, CourseDistance = 1.2 };
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
                {
                    new LocationModel { LocationType = LocationType.Provider, CourseDistance = 5.0 },
                    closest,
                    new LocationModel { LocationType = LocationType.Provider, CourseDistance = 3.5 }
                }
        };

        Assert.That(sut.ClosestProviderLocation, Is.EqualTo(closest));
    }

    [Test]
    public void ClosestProviderLocation_WhenNoneAreProvider_ReturnsNull()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
                {
                    new LocationModel { LocationType = LocationType.Online, CourseDistance = 1.0 },
                    new LocationModel { LocationType = LocationType.National, CourseDistance = 2.0 }
                }
        };

        Assert.That(sut.ClosestProviderLocation, Is.Null);
    }

    [TestCase(LocationType.Online, CourseType.ShortCourse, true)]
    [TestCase(LocationType.Online, CourseType.Apprenticeship, false)]
    [TestCase(LocationType.Provider, CourseType.ShortCourse, false)]
    public void ShowOnlineOption_LocationTypeAndCourseTypeVaries_ReturnsExpected(LocationType locationType, CourseType courseType, bool expected)
    {
        var sut = new CourseProviderViewModel
        {
            CourseType = courseType,
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = locationType }
            }
        };

        Assert.That(sut.ShowOnlineOption, Is.EqualTo(expected));
    }

    [TestCase(LocationType.Provider, CourseType.ShortCourse, true)]
    [TestCase(LocationType.Provider, CourseType.Apprenticeship, false)]
    [TestCase(LocationType.Online, CourseType.ShortCourse, false)]
    public void ShowProviderOption_LocationTypeAndCourseTypeVaries_ReturnsExpected(LocationType locationType, CourseType courseType, bool expected)
    {
        var sut = new CourseProviderViewModel
        {
            CourseType = courseType,
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = locationType }
            }
        };

        Assert.That(sut.ShowProviderOption, Is.EqualTo(expected));
    }

    [Test]
    public void HasMultipleProviderLocations_WhenMoreThanOneProviderLocation_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Provider },
                new LocationModel { LocationType = LocationType.Provider },
                new LocationModel { LocationType = LocationType.Online }
            }
        };

        Assert.That(sut.HasMultipleProviderLocations, Is.True);
    }

    [Test]
    public void HasMultipleProviderLocations_WhenOneProviderLocationOrLess_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Provider },
                new LocationModel { LocationType = LocationType.Online }
            }
        };

        Assert.That(sut.HasMultipleProviderLocations, Is.False);
    }

    [Test]
    public void ProviderLocations_WhenProviderAndNonProviderLocationsExist_ReturnsOnlyProviderLocations()
    {
        var providerLocation1 = new LocationModel { LocationType = LocationType.Provider };
        var providerLocation2 = new LocationModel { LocationType = LocationType.Provider };

        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                providerLocation1,
                new LocationModel { LocationType = LocationType.Online },
                providerLocation2
            }
        };

        Assert.That(sut.ProviderLocations, Is.EqualTo(new[] { providerLocation1, providerLocation2 }));
    }

    [Test]
    public void ShowBlockReleaseOption_WhenBlockReleaseLocationAndNotApprenticeship_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            CourseType = CourseType.ShortCourse,
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = true }
            }
        };

        Assert.That(sut.ShowBlockReleaseOption, Is.False);
    }

    [Test]
    public void ShowDayReleaseOption_WhenDayReleaseLocationAndNotApprenticeship_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            CourseType = CourseType.ShortCourse,
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true }
            }
        };

        Assert.That(sut.ShowDayReleaseOption, Is.False);
    }

    [Test]
    public void ImplicitOperator_WhenCoursesAndLocationsAreNull_ReturnsEmptyCollections()
    {
        var source = new GetCourseProviderQueryResult
        {
            Courses = null,
            Locations = null
        };

        var sut = (CourseProviderViewModel)source;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.Courses, Is.Not.Null);
            Assert.That(sut.Courses, Is.Empty);
            Assert.That(sut.Locations, Is.Not.Null);
            Assert.That(sut.Locations, Is.Empty);
            Assert.That(sut.ProviderCoursesDetails.Courses, Is.Empty);
        }
    }

    [Test]
    public void TrainingOptions_WhenAccessed_MapsExpectedValues()
    {
        var providerLocation = new LocationModel
        {
            LocationType = LocationType.Provider,
            CourseDistance = 1.25,
            AddressLine1 = "1 Provider Street",
            Town = "Leeds",
            Postcode = "LS1 1AA"
        };

        var sut = new CourseProviderViewModel
        {
            CourseType = CourseType.ShortCourse,
            Location = "Leeds",
            Locations = new List<LocationModel>
            {
                new LocationModel { LocationType = LocationType.Online },
                new LocationModel { LocationType = LocationType.National },
                providerLocation
            }
        };

        var result = sut.TrainingOptions;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Location, Is.EqualTo("Leeds"));
            Assert.That(result.ShowOnlineOption, Is.True);
            Assert.That(result.ShowLearnerWorkplaceOption, Is.True);
            Assert.That(result.HasMatchingRegionalLocationOrNational, Is.True);
            Assert.That(result.ShowProviderOption, Is.True);
            Assert.That(result.ProviderLocations, Is.EqualTo(new[] { providerLocation }));
            Assert.That(result.ClosestProviderLocation, Is.EqualTo(providerLocation));
            Assert.That(result.ClosestProviderLocationDistanceDisplay, Is.EqualTo("1.3"));
            Assert.That(result.ClosestProviderLocationAddress, Is.EqualTo("1 Provider Street, Leeds, LS1 1AA"));
        }
    }

    [Test]
    public void TrainingOptions_WhenProviderHasMultipleLocations_MapsProviderLocationSummaryValues()
    {
        var closestProviderLocation = new LocationModel
        {
            LocationType = LocationType.Provider,
            CourseDistance = 0.95,
            AddressLine1 = "1 Closest Street",
            Town = "Leeds",
            Postcode = "LS1 1AA"
        };

        var sut = new CourseProviderViewModel
        {
            CourseType = CourseType.ShortCourse,
            Locations = new List<LocationModel>
            {
                new LocationModel
                {
                    LocationType = LocationType.Provider,
                    CourseDistance = 2.5,
                    AddressLine1 = "2 Other Street",
                    Town = "Leeds",
                    Postcode = "LS1 2BB"
                },
                closestProviderLocation,
                new LocationModel { LocationType = LocationType.Online }
            }
        };

        var result = sut.TrainingOptions;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ShowProviderOption, Is.True);
            Assert.That(result.ProviderLocations, Has.Count.EqualTo(2));
            Assert.That(result.HasMultipleProviderLocations, Is.True);
            Assert.That(result.ClosestProviderLocation, Is.EqualTo(closestProviderLocation));
            Assert.That(result.ClosestProviderLocationDistanceDisplay, Is.EqualTo("1.0"));
            Assert.That(result.ClosestProviderLocationAddress, Is.EqualTo("1 Closest Street, Leeds, LS1 1AA"));
        }
    }

    [Test]
    public void AchievementsAndParticipation_WhenAccessed_MapsExpectedValues()
    {
        var endpointAssessments = new EndpointAssessmentModel(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc), 12);
        var qar = new QarModel
        {
            AchievementRate = "90",
            Leavers = "100",
            Period = "2122",
            NationalLeavers = "100",
            NationalAchievementRate = "80"
        };

        var sut = new CourseProviderViewModel
        {
            Qar = qar,
            EndpointAssessments = endpointAssessments
        };

        var result = sut.AchievementsAndParticipation;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Qar, Is.EqualTo(qar));
            Assert.That(result.AchievementRateInformation, Is.EqualTo(sut.AchievementRateInformation));
            Assert.That(result.EndpointAssessmentsCountDisplay, Is.EqualTo("12"));
            Assert.That(result.EndpointAssessmentDisplayMessage, Is.EqualTo(sut.EndpointAssessmentDisplayMessage));
        }
    }

    [Test]
    public void ContactDetails_WhenAccessed_MapsExpectedValues()
    {
        var contact = new ContactModel
        {
            Email = "provider@test.com",
            PhoneNumber = "01234567890"
        };

        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel
            {
                AddressLine1 = "1 High Street",
                Town = "Leeds",
                Postcode = "LS1 2AB"
            },
            Contact = contact
        };

        var result = sut.ContactDetails;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RegisteredAddress, Is.EqualTo("1 High Street, Leeds, LS1 2AB"));
            Assert.That(result.Email, Is.EqualTo(contact.Email));
            Assert.That(result.PhoneNumber, Is.EqualTo(contact.PhoneNumber));
        }
    }

    [Test]
    public void ProviderReviews_WhenAccessed_MapsExpectedValues()
    {
        var reviews = new ReviewsModel
        {
            EmployerReviews = "2",
            ApprenticeReviews = "1"
        };

        var sut = new CourseProviderViewModel
        {
            Reviews = reviews
        };

        var result = sut.ProviderReviews;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Reviews, Is.EqualTo(reviews));
            Assert.That(result.EmployerReviewsDisplayMessage, Is.EqualTo(sut.EmployerReviewsDisplayMessage));
            Assert.That(result.ApprenticeReviewsDisplayMessage, Is.EqualTo(sut.ApprenticeReviewsDisplayMessage));
        }
    }

    [Test]
    public void ShortlistPanel_WhenAccessed_MapsExpectedValues()
    {
        var shortlistId = Guid.NewGuid();
        var sut = new CourseProviderViewModel
        {
            LarsCode = "123",
            Ukprn = 10000001,
            ProviderName = "Provider A",
            Location = "Leeds",
            ShortlistId = shortlistId,
            TotalProvidersCount = 2,
            CourseName = "Software developer",
            Level = 4
        };

        var result = sut.ShortlistPanel;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ShortlistClass, Is.EqualTo("app-provider-shortlist-added"));
            Assert.That(result.LarsCode, Is.EqualTo("123"));
            Assert.That(result.Ukprn, Is.EqualTo(10000001));
            Assert.That(result.ProviderName, Is.EqualTo("Provider A"));
            Assert.That(result.Location, Is.EqualTo("Leeds"));
            Assert.That(result.ShortlistId, Is.EqualTo(shortlistId));
            Assert.That(result.ShowMultipleProvidersForCourse, Is.True);
            Assert.That(result.TotalProvidersCount, Is.EqualTo(2));
            Assert.That(result.CourseNameAndLevel, Is.EqualTo("Software developer (level 4)"));
        }
    }

    [Test]
    public void ContactDetails_ContactAndAddressAreNull_ReturnsEmptyDefaults()
    {
        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel(),
            Contact = null
        };

        var result = sut.ContactDetails;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RegisteredAddress, Is.EqualTo(string.Empty));
            Assert.That(result.Email, Is.EqualTo(string.Empty));
            Assert.That(result.PhoneNumber, Is.EqualTo(string.Empty));
            Assert.That(result.Website, Is.EqualTo(string.Empty));
        }
    }

    [Test]
    public void ContactDetails_ContactHasWebsite_ReturnsMappedWebsite()
    {
        var sut = new CourseProviderViewModel
        {
            ProviderAddress = new ShortProviderAddressModel
            {
                AddressLine1 = "1 High Street",
                Town = "Leeds",
                Postcode = "LS1 2AB"
            },
            Contact = new ContactModel
            {
                Website = "https://provider.test"
            }
        };

        var result = sut.ContactDetails;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.RegisteredAddress, Is.EqualTo("1 High Street, Leeds, LS1 2AB"));
            Assert.That(result.Website, Is.EqualTo("https://provider.test"));
        }
    }

    [Test]
    public void HasLocation_LocationIsNull_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Location = null
        };

        Assert.That(sut.HasLocation, Is.False);
    }

    [Test]
    public void HasLocation_LocationIsWhitespace_ReturnsFalse()
    {
        var sut = new CourseProviderViewModel
        {
            Location = "   "
        };

        Assert.That(sut.HasLocation, Is.False);
    }

    [Test]
    public void HasLocation_LocationIsSet_ReturnsTrue()
    {
        var sut = new CourseProviderViewModel
        {
            Location = "Leeds"
        };

        Assert.That(sut.HasLocation, Is.True);
    }
}

