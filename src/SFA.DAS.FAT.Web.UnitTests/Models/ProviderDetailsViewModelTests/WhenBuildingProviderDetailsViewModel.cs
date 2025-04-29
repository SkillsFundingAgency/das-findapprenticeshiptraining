using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Providers.Api.Responses;
using SFA.DAS.FAT.Web.Models.Providers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Models.ProviderDetailsViewModelTests;

public class WhenBuildingProviderDetailsViewModel
{
    public const string ProviderName = "Joe Cools Emporium";
    public const string Address1 = "10 Smith Street";
    public const string Address2 = "Lower Dakin";
    public const string Address3 = "Near Burnalt";
    public const string Address4 = "Smith";
    public const string Town = "Coventry";
    public const string Postcode = "CV1 1VC";

    [Test, MoqAutoData]
    public void Then_The_ViewModel_Is_Built(
        int ukprn,
        GetProviderQueryResponse response)
    {
        response.Ukprn = ukprn;
        response.AnnualEmployerFeedbackDetails = null;
        response.AnnualApprenticeFeedbackDetails = null;

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();
        sut.Should().BeEquivalentTo(response, options => options
            .Excluding(r => r.ProviderAddress)
            .Excluding(r => r.Qar)
            .Excluding(r => r.Reviews)
            .Excluding(r => r.Courses)
            .Excluding(r => r.EndpointAssessments)
            .Excluding(r => r.AnnualApprenticeFeedbackDetails)
            .Excluding(r => r.AnnualEmployerFeedbackDetails)
            .Excluding(r => r.Contact.Website)
        );

        sut!.ShowSearchCrumb.Should().Be(true);
        sut.ShowShortListLink.Should().Be(true);
    }

    [TestCase(null, 0, "", false)]
    [TestCase(0, 0, "View 0 courses delivered by this training provider", false)]
    [TestCase(1, 1, "View 1 course delivered by this training provider", true)]
    [TestCase(2, 2, "View 2 courses delivered by this training provider", true)]
    [TestCase(500, 500, "View 500 courses delivered by this training provider", true)]
    public void Then_The_Courses_Are_Set_Up(int? coursesCount, int expectedCount, string dropdownText, bool showCoursesExpected)
    {
        var ukprn = 12345678;

        List<GetProviderCourseDetails> courses = null;

        if (coursesCount != null)
        {
            courses = new List<GetProviderCourseDetails>();
            if (coursesCount > 0)
            {
                for (int i = 0; i < coursesCount; i++)
                {
                    courses.Add(new GetProviderCourseDetails { CourseName = $"course {i}", LarsCode = i, Level = 1 });
                }
            }
        }

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            Courses = courses
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        using (new AssertionScope())
        {
            sut.Should().NotBeNull();
            sut!.ProviderCoursesDetails.Courses.Should().BeEquivalentTo(courses,
                options => options
                    .Excluding(r => r.IfateReferenceNumber));
            sut.ProviderCoursesDetails.CourseCount.Should().Be(expectedCount);
            sut.ProviderCoursesDetails.CoursesDropdownText.Should().Be(dropdownText);
            sut.ShowCourses.Should().Be(showCoursesExpected);
        }

    }

    [TestCase("", "", false, 0, 0, "")]
    [TestCase("10", "", false, 0, 10, "")]
    [TestCase("", "95.3", false, 0, 0, "4.7")]
    [TestCase("200", "95.5", true, 191, 200, "4.5")]
    public void Then_The_Qar_Is_Set_Up(string leavers, string achievementRate,
         bool achievementRatePresent, int qarAchievers, int numberOfParticipants,
        string numberWhoDidNotPassPercentage)
    {
        var ukprn = 12345678;

        GetProviderQarModel qar = new GetProviderQarModel
        {
            Leavers = leavers,
            AchievementRate = achievementRate,
            NationalAchievementRate = "80.3"
        };

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Qar = qar,
            Ukprn = ukprn
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        using (new AssertionScope())
        {
            sut!.Qar.AchievementRatePresent.Should().Be(achievementRatePresent);
            sut.Qar.Achievers.Should().Be(qarAchievers);
            sut.Qar.TotalParticipantCount.Should().Be(numberOfParticipants);
            sut.Qar.DidNotPassPercentage.Should().Be(numberWhoDidNotPassPercentage);
            sut.Qar.AchievementRate.Should().Be(achievementRate);
            sut.Qar.NationalAchievementRate.Should().Be(response.Qar.NationalAchievementRate);
        }
    }

    [TestCase(null, 2025, "", "")]
    [TestCase(0, 2020, "0", "apprentices have completed a course and taken their end-point assessment with this provider.")]
    [TestCase(1, 2020, "1", "apprentice has completed a course and taken their end-point assessment with this provider since 2020.")]
    [TestCase(2, 2018, "2", "apprentices have completed a course and taken their end-point assessment with this provider since 2018.")]
    [TestCase(10, 2018, "10", "apprentices have completed a course and taken their end-point assessment with this provider since 2018.")]
    [TestCase(100, 2018, "100", "apprentices have completed a course and taken their end-point assessment with this provider since 2018.")]
    [TestCase(1000, 2017, "1,000", "apprentices have completed a course and taken their end-point assessment with this provider since 2017.")]
    [TestCase(10000, 2017, "10,000", "apprentices have completed a course and taken their end-point assessment with this provider since 2017.")]
    [TestCase(100000, 2016, "100,000", "apprentices have completed a course and taken their end-point assessment with this provider since 2016.")]
    [TestCase(1000000, 2015, "1,000,000", "apprentices have completed a course and taken their end-point assessment with this provider since 2015.")]
    public void Then_EndpointAssessments_Are_Set_Up(int? endpointAssessmentCount, int earliestYear, string expectedFormattedCount, string expectedEndpointAssessmentDetails)
    {
        var ukprn = 12345678;

        GetProviderEndpointAssessmentsDetails endpointAssessments = null;

        if (endpointAssessmentCount != null)
        {
            endpointAssessments = new GetProviderEndpointAssessmentsDetails
            {
                EarliestAssessment = new DateTime(earliestYear, 1, 1),
                EndpointAssessmentCount = endpointAssessmentCount.Value
            };
        }

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            EndpointAssessments = endpointAssessments
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        sut!.EndpointAssessments.CountFormatted.Should().Be(expectedFormattedCount);
        sut.EndpointAssessments.DetailsMessage.Should().Be(expectedEndpointAssessmentDetails);
    }

    [TestCase(null, "", "", "", "", "", "", 0, 0, 0, 0, "", "")]
    [TestCase("2324", "2023", "2024", "", "", "", "", 0, 0, 0, 0, "average review from 0 employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.", "average review from 0 apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("2324", "2023", "2024", "1", "2", "", "", 1, 2, 0, 0, "average review from 1 employer when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.", "average review from 0 apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("2223", "2022", "2023", "2", "3", "1", "2", 2, 3, 1, 2, "average review from 2 employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.", "average review from 1 apprentice when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    [TestCase("2122", "2021", "2022", "4", "5", "2", "3", 4, 5, 2, 3, "average review from 4 employers when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.", "average review from 2 apprentices when asked to rate this provider as 'Excellent', 'Good', 'Poor' or 'Very poor'.")]
    public void Then_ReviewDetails_Are_Set_Up(
        string reviewPeriod, string expectedReviewsStartYear, string expectedReviewsEndYear,

        string employerReviews, string employerStars,
        string apprenticeReviews, string apprenticeStars,

        int expectedEmployerReviewCount, int expectedEmployerStarsValue,
        int expectedApprenticeReviewCount, int expectedApprenticeStarsValue,
        string expectedEmployerMessage, string expectedApprenticeMessage
    )
    {
        var ukprn = 12345678;

        GetProviderReviewsModel reviews = null;

        if (reviewPeriod != null)
        {
            reviews = new GetProviderReviewsModel
            {
                ReviewPeriod = reviewPeriod,
                EmployerReviews = employerReviews,
                EmployerStars = employerStars,
                ApprenticeReviews = apprenticeReviews,
                ApprenticeStars = apprenticeStars,
            };
        }

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            Reviews = reviews
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        using (new AssertionScope())
        {
            sut!.Reviews.StartYear.Should().Be(expectedReviewsStartYear);
            sut.Reviews.EndYear.Should().Be(expectedReviewsEndYear);
            sut.Reviews.EmployerStarsValue.Should().Be(expectedEmployerStarsValue);
            sut.Reviews.EmployerReviewCount.Should().Be(expectedEmployerReviewCount);
            sut.Reviews.ApprenticeStarsValue.Should().Be(expectedApprenticeStarsValue);
            sut.Reviews.ApprenticeReviewCount.Should().Be(expectedApprenticeReviewCount);
            sut.Reviews.EmployerStarsMessage.Should().Be(expectedEmployerMessage);
            sut.Reviews.ApprenticeStarsMessage.Should().Be(expectedApprenticeMessage);
        }
    }

    [TestCase(ProviderRating.Excellent, true, ProviderRating.NotYetReviewed, false)]
    [TestCase(ProviderRating.Good, true, ProviderRating.VeryPoor, true)]
    [TestCase(ProviderRating.Poor, true, ProviderRating.Poor, true)]
    [TestCase(ProviderRating.VeryPoor, true, ProviderRating.Good, true)]
    [TestCase(ProviderRating.NotYetReviewed, false, ProviderRating.Excellent, true)]
    [TestCase(ProviderRating.NotYetReviewed, false, ProviderRating.NotYetReviewed, false)]
    public void Then_ReviewRatings_Are_Set_Up(
        ProviderRating employerProviderRating,
        bool isEmployerReviewed,
        ProviderRating apprenticeProviderRating,
        bool isApprenticeReviewed
        )
    {
        var ukprn = 12345678;

        var reviews = new GetProviderReviewsModel
        {
            ReviewPeriod = "2324",
            EmployerReviews = "1",
            EmployerStars = "2",
            ApprenticeReviews = "3",
            ApprenticeStars = "4",
            EmployerRating = employerProviderRating.ToString(),
            ApprenticeRating = apprenticeProviderRating.ToString()
        };


        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            Reviews = reviews
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        using (new AssertionScope())
        {
            sut!.EmployerReviewed.Should().Be(isEmployerReviewed);
            sut!.ApprenticeReviewed.Should().Be(isApprenticeReviewed);
        }
    }

    [TestCase(null, "", "", "", "", "", "", "")]
    [TestCase("", "", "", "", "", "", "", "")]
    [TestCase("", null, null, null, null, null, null, "")]
    [TestCase(ProviderName, "", "", "", "", "", "", ProviderName)]
    [TestCase(ProviderName, Address1, "", "", "", "", "", $"{ProviderName}, {Address1}")]
    [TestCase(ProviderName, "", Address2, "", "", "", "", $"{ProviderName}, {Address2}")]
    [TestCase(ProviderName, "", "", Address3, "", "", "", $"{ProviderName}, {Address3}")]
    [TestCase(ProviderName, "", "", "", Address4, "", "", $"{ProviderName}, {Address4}")]
    [TestCase(ProviderName, "", "", "", "", Postcode, "", $"{ProviderName}, {Postcode}")]
    [TestCase(ProviderName, "", "", "", "", "", Town, $"{ProviderName}, {Town}")]
    [TestCase("", Address1, "", "", "", "", "", $"{Address1}")]
    [TestCase("", "", Address2, "", "", "", "", $"{Address2}")]
    [TestCase("", "", "", Address3, "", "", "", $"{Address3}")]
    [TestCase("", "", "", "", Address4, "", "", $"{Address4}")]
    [TestCase("", "", "", "", "", Postcode, "", $"{Postcode}")]
    [TestCase("", "", "", "", "", "", Town, $"{Town}")]
    [TestCase(ProviderName, "", "", "", "", "", "", ProviderName)]
    [TestCase(ProviderName, Address1, Address2, "", "", "", "", $"{ProviderName}, {Address1}, {Address2}")]
    [TestCase(ProviderName, Address1, "", Address3, "", "", "", $"{ProviderName}, {Address1}, {Address3}")]
    [TestCase(ProviderName, Address1, "", "", Address4, "", "", $"{ProviderName}, {Address1}, {Address4}")]
    [TestCase(ProviderName, Address1, "", "", "", Town, "", $"{ProviderName}, {Address1}, {Town}")]
    [TestCase(ProviderName, Address1, "", "", "", "", Postcode, $"{ProviderName}, {Address1}, {Postcode}")]
    [TestCase(ProviderName, Address1, Address2, Address3, Address4, Town, Postcode, $"{ProviderName}, {Address1}, {Address2}, {Address3}, {Address4}, {Town}, {Postcode}")]
    public void Then_ComposedAddress_Is_Set(
       string providerName, string address1, string address2, string address3, string address4, string town, string postcode, string expectedAddress)
    {
        var ukprn = 12345678;

        GetProviderAddressModel addressModel = null;

        if (providerName != null)
        {
            addressModel = new GetProviderAddressModel
            {
                AddressLine1 = address1,
                AddressLine2 = address2,
                AddressLine3 = address3,
                AddressLine4 = address4,
                Town = town,
                Postcode = postcode
            };
        }

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            ProviderAddress = addressModel,
            ProviderName = providerName
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();
        sut!.ProviderAddress.Should().Be(expectedAddress);
    }

    [TestCase(false, "", "")]
    [TestCase(true, "", "")]
    [TestCase(true, "www.website.co.uk", "http://www.website.co.uk")]
    [TestCase(true, "http://www.website2.co.uk", "http://www.website2.co.uk")]
    [TestCase(true, "https://www.website3.co.uk", "https://www.website3.co.uk")]
    public void Then_Composed_Website_Is_Set(bool isContactSetUp, string website, string expectedWebsite)
    {
        var ukprn = 12345678;

        GetProviderContactDetails contactDetails = null;

        if (isContactSetUp)
        {
            contactDetails = new GetProviderContactDetails { Website = website };
        }

        GetProviderQueryResponse response = new GetProviderQueryResponse
        {
            Ukprn = ukprn,
            Contact = contactDetails
        };

        ProviderDetailsViewModel sut = response;
        sut.Should().NotBeNull();

        sut!.Contact.Website.Should().Be(expectedWebsite);
    }
}
