using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;
using SFA.DAS.FAT.MockServer;
using SFA.DAS.FAT.Web.AcceptanceTests.Infrastructure;
using TechTalk.SpecFlow;
using DeliveryModeType = SFA.DAS.FAT.Web.Models.DeliveryModeType;

namespace SFA.DAS.FAT.Web.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CourseProviderSteps
    {
        private readonly ScenarioContext _context;

        public CourseProviderSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Then("there is a row for each course provider")]
        public async Task ThenThereIsARowForEachCourseProvider()
        {
            var response = _context.Get<HttpResponseMessage>(ContextKeys.HttpResponse);

            var actualContent = await response.Content.ReadAsStringAsync();

            var json = DataFileManager.GetFile("course-providers.json");
            var expectedApiResponse = JsonConvert.DeserializeObject<TrainingCourseProviders>(json);

            foreach (var courseProvider in expectedApiResponse.CourseProviders)
            {
                actualContent.Should().Contain(HttpUtility.HtmlEncode(courseProvider.Name));
            }
        }

        [Then("the delivery modes for each provider are (not )?displayed")]
        public async Task ThenTheDeliveryModesForEachProviderAreDisplayed(string not)
        {
            var json = DataFileManager.GetFile("course-providers.json");
            var expectedApiResponse = JsonConvert.DeserializeObject<TrainingCourseProviders>(json);

            var response = _context.Get<HttpResponseMessage>(ContextKeys.HttpResponse);
            var actualContent = await response.Content.ReadAsStringAsync();

            foreach (var mode in Enum.GetValues(typeof(DeliveryModeType)).Cast<DeliveryModeType>())
            {
                if (mode == DeliveryModeType.NotFound || mode == DeliveryModeType.National)
                {
                    continue;
                }
                var modeName = mode.GetDescription().Replace("’", "&#x2019;");// for some reason HtmlEncode doesn't encode '’'

                if (not == string.Empty)
                {
                    actualContent.Should().Contain(modeName, Exactly.Times(expectedApiResponse.Total + 1));// additional time in the filter ui
                }
                else
                {
                    actualContent.Should().NotContain(modeName);
                }
            }
        }
    }
}
