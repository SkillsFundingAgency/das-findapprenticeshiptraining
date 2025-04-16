using System.Net;
using NUnit.Framework;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourseProviderDetails;
using SFA.DAS.FAT.Domain;
using SFA.DAS.FAT.Domain.CourseProviders;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models;

public class WhenCreatingCourseProviderViewModel
{
    [Test, MoqAutoData]
    public void Then_Properties_Are_Correctly_Mapped(GetCourseProviderQueryResult source)
    {
        var sut = (CourseProviderViewModel)source;

        Assert.Multiple(() =>
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
            Assert.That(sut.Courses, Is.EqualTo(source.Courses));
            Assert.That(sut.AnnualEmployerFeedbackDetails, Is.EqualTo(source.AnnualEmployerFeedbackDetails));
            Assert.That(sut.AnnualApprenticeFeedbackDetails, Is.EqualTo(source.AnnualApprenticeFeedbackDetails));
        });
    }

    [Test]
    public void Returns_Not_Enough_Apprentices_Message_When_Endpoint_Assessments_Is_Null()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = null
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("Not enough apprentices have completed a course with this provider to show participation"));
    }

    [Test]
    public void Returns_Not_Enough_Apprentices_Message_When_Earliest_Assessment_Is_Null()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(null, 0)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("Not enough apprentices have completed a course with this provider to show participation"));
    }

    [Test]
    public void Returns_Message_When_Assessment_Count_Is_Zero()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentices have completed a course and taken their end-point assessment with this provider."));
    }

    [Test]
    public void Returns_Message_With_Year_And_Plural_When_Assessment_Count_Greater_Than_One()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2019, 5, 1, 0, 0, 0, DateTimeKind.Utc), 5)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentices have completed this course and taken their end-point assessment with this provider since 2019"));
    }

    [Test]
    public void Returns_Message_With_Year_And_Singular_When_Assessment_Count_Is_One()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2021, 3, 1, 0, 0, 0, DateTimeKind.Utc), 1)
        };

        Assert.That(sut.EndpointAssessmentDisplayMessage, Is.EqualTo("apprentice have completed this course and taken their end-point assessment with this provider since 2021"));
    }

    [Test]
    public void Provider_Address_Should_Combine_All_Non_Empty_Properties_With_Commas()
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
    public void Then_Provider_Address_Should_Return_Empty_If_All_Parts_Are_Empty()
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
    public void Then_Provider_Address_Should_Trim_Values()
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
    public void Then_Block_Release_Locations_Should_Return_Formatted_Addresses_For_Block_Release_Only()
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

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("123 Main St, Building 4, Belfast, Antrim, BT1 1AA"));
            Assert.That(result[1], Is.EqualTo("456 Side St, Derry, BT2 2BB"));
        });
    }

    [Test]
    public void Then_Day_Release_Locations_Should_Return_Formatted_Addresses_For_Day_Release_Only()
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

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("Day Site 1, Newry, BT3 3CC"));
        });
    }

    [Test]
    public void Then_Block_Release_Locations_Should_Return_EmptyList_If_No_Block_Release_Locations_Exist()
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
    public void Then_Day_Release_Locations_Should_Return_Empty_List_If_No_Day_Release_Locations_Exist()
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
    public void Then_Get_Apprentice_Work_place_Display_Message_Returns_National_Message_When_Location_Type_Is_National()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = true, LocationType = LocationType.National }
            }
        };

        Assert.That(sut.ApprenticeWorkplaceDisplayMessage, Is.EqualTo("Training is provided at apprentice’s workplaces across England."));
    }

    [Test]
    public void Then_Get_Apprentice_Workplace_Display_Message_Returns_Regional_Message_When_Location_Type_Is_Regional()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = true, LocationType = LocationType.Regional }
            }
        };

        Assert.That(sut.ApprenticeWorkplaceDisplayMessage, Is.EqualTo("Training is provided at apprentice’s workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the apprentice’s workplace in your location."));
    }

    [Test]
    public void Then_Get_Apprentice_Workplace_Display_Message_Returns_Regional_Message_When_No_Location_Is_Found()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = false, LocationType = LocationType.National }
            }
        };

        Assert.That(sut.ApprenticeWorkplaceDisplayMessage, Is.EqualTo("Training is provided at apprentice’s workplaces in certain regions. Search for a city or postcode to see if the provider offers training at the apprentice’s workplace in your location."));
    }

    
    [Test]
    public void Get_Achievement_Rate_Information_Returns_Formatted_Message_When_Achievement_Rate_Is_Not_Null()
    { 
        var sut = new CourseProviderViewModel
        {
            Qar = new QarModel
            {
                AchievementRate = "90",
                Leavers = "100",
                Period = "2122",
                NationalLeavers = "120",
                NationalAchievementRate = "20"
            }
        };

        var result = sut.AchievementRateInformation;

        var expected = "of apprentices (90 of 100) completed this course and passed their end-point assessment with this provider in academic year 2021 to 2022. 10% did not pass or left the course before taking the assessment.";

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Get_Achievement_Rate_Information_Returns_Empty_String_When_Achievement_Rate_Is_Null()
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
    public void Courses_Delivered_Display_Text_Returns_Correct_Message_Based_On_Courses_Delivered_Count(int courseCount, string expected)
    {
        var sut = new CourseProviderViewModel
        {
            Courses = Enumerable.Range(1, courseCount)
                                .Select(i => new ProviderCourseModel { LarsCode = i })
                                .ToList()
        };

        Assert.That(sut.CoursesDeliveredCountDisplay, Is.EqualTo(expected));
    }

    [TestCase("1", "average review from 1 employer when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("5", "average review from 5 employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    public void Then_Get_Employer_Reviews_Display_Message_Returns_Correct_Message_For_Valid_Employer_Reviews(string reviewCount, string expected)
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
    public void Then_Get_Employer_Reviews_Display_Message_Returns_Empty_If_Employer_Reviews_Cannot_Be_Parsed(string invalidReviewValue)
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
    public void Then_Get_Apprentice_Reviews_Display_Message_Returns_Correct_Message_For_Valid_Employer_Reviews(string reviewCount, string expected)
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
    public void Then_Get_Apprentice_Reviews_Display_Message_Returns_Empty_If_Apprentice_Reviews_Cannot_Be_Parsed(string invalidReviewValue)
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
    public void Then_Endpoint_Assessments_Count_Display_Returns_No_Data_When_Endpoint_Assessments_Is_Null()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = null
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo("No data"));
    }

    [Test]
    public void Then_Endpoint_Assessments_Count_Display_Returns_NoData_When_Earliest_Assessment_Is_Null()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(null, 5)
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo("No data"));
    }

    [Test]
    public void Then_Endpoint_Assessments_Count_Display_Returns_Count_As_String_When_Valid()
    {
        var sut = new CourseProviderViewModel
        {
            EndpointAssessments = new EndpointAssessmentModel(new DateTime(2022, 9, 1, 0, 0, 0, DateTimeKind.Utc), 8)
        };

        Assert.That(sut.EndpointAssessmentsCountDisplay, Is.EqualTo("8"));
    }

    [Test]
    public void Then_Has_Multiple_Block_Release_Locations_Returns_True_When_More_Than_One()
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
    public void Then_Has_Multiple_Block_Release_Locations_Returns_False_When_One_Or_Less()
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
    public void Then_Has_Multiple_Day_Release_Locations_Returns_True_When_More_Than_One()
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
    public void Then_Has_Multiple_Day_Release_Locations_Returns_False_When_One_Or_Less()
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
    public void Then_Course_Name_And_Level_Should_Combine_Course_Name_And_Level()
    {
        var sut = new CourseProviderViewModel
        {
            CourseName = "Software Developer",
            Level = 4
        };

        Assert.That(sut.CourseNameAndLevel, Is.EqualTo("Software Developer (level 4)"));
    }

    [Test]
    public void Then_Is_National_Returns_True_If_Any_At_Employer_And_Location_Type_National_Is_True()
    {
        var viewModel = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = false, LocationType = LocationType.Provider },
                new LocationModel { AtEmployer = true, LocationType = LocationType.National }
            }
        };

        Assert.That(viewModel.IsNational, Is.True);
    }

    [Test]
    public void Then_Is_National_Returns_False_If_None_At_Employer()
    {
        var viewModel = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { AtEmployer = false, LocationType = LocationType.National }
            }
        };

        Assert.That(viewModel.IsNational, Is.False);
    }

    [Test]
    public void Is_Block_Release_Returns_True_If_Any_Block_Release_Is_True()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = false },
                new LocationModel { BlockRelease = true }
            }
        };

        Assert.That(sut.IsBlockRelease, Is.True);
    }

    [Test]
    public void Then_Is_Block_Release_Returns_False_If_None_Block_Release()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { BlockRelease = false }
            }
        };

        Assert.That(sut.IsBlockRelease, Is.False);
    }

    [Test]
    public void Then_Is_Day_Release_Returns_True_If_Any_Day_Release_Is_True()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = true }
            }
        };

        Assert.That(sut.IsDayRelease, Is.True);
    }

    [Test]
    public void Then_Is_Day_Release_Returns_False_If_No_Day_Release()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel { DayRelease = false }
            }
        };

        Assert.That(sut.IsDayRelease, Is.False);
    }

    [Test]
    public void Shortlist_Class_Returns_Added_When_Under_Maximum_Shortlist_Count()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount - 1
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-added"));
    }

    [Test]
    public void Shortlist_Class_Returns_Full_When_Equal_To_Maximum_Shortlist_Count()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-full"));
    }

    [Test]
    public void Shortlist_Class_Returns_Full_When_Above_Maximum_Shortlist_Count()
    {
        var sut = new CourseProviderViewModel
        {
            ShortlistCount = ShortlistConstants.MaximumShortlistCount + 1
        };

        Assert.That(sut.ShortlistClass, Is.EqualTo("app-provider-shortlist-full"));
    }

    [Test]
    public void Then_National_Location_Returns_First_Matching_Employer_With_National_LocationType()
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
    public void Then_National_Location_Returns_Null_If_No_Matching_Location()
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
    public void Then_National_Location_Returns_Null_If_Locations_Is_Empty()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>()
        };

        Assert.That(sut.NationalLocation, Is.Null);
    }

    [Test]
    public void Then_National_Work_Location_Distance_Display_Text_Returns_Formatted_Distance_When_National_Location_Is_Not_Null()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel
                {
                    AtEmployer = true,
                    LocationType = LocationType.National,
                    CourseDistance = 12.456
                }
            }
        };

        Assert.That(sut.NationalWorkLocationDistanceDisplayText, Is.EqualTo("12.5 miles"));
    }

    [Test]
    public void Then_National_Work_Location_Distance_Display_Text_Returns_Zero_Miles_When_National_Location_Is_Null()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>
            {
                new LocationModel
                {
                    AtEmployer = false,
                    LocationType = LocationType.Regional,
                    CourseDistance = 99.99
                }
            }
        };

        Assert.That(sut.NationalWorkLocationDistanceDisplayText, Is.EqualTo("0.0 miles"));
    }

    [Test]
    public void Then_National_Work_Location_Distance_Display_Text_Returns_Zero_Miles_When_Locations_Is_Empty()
    {
        var sut = new CourseProviderViewModel
        {
            Locations = new List<LocationModel>()
        };

        Assert.That(sut.NationalWorkLocationDistanceDisplayText, Is.EqualTo("0.0 miles"));
    }

    [Test]
    public void Then_Closest_Block_Release_Location_Returns_Location_With_Smallest_Course_Distance()
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
    public void Then_Closest_Block_Release_Location_Returns_Null_If_None_Are_Block_Release()
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
    public void Then_Closest_Day_Release_Location_Returns_Location_With_Smallest_Course_Distance()
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
    public void Then_Closest_Day_Release_Location_Returns_Null_If_None_Are_DayRelease()
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
}
