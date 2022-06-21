using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SFA.DAS.FAT.Web.AcceptanceTests.Infrastructure;
using SFA.DAS.FAT.Web.Models;
using TechTalk.SpecFlow;

namespace SFA.DAS.FAT.Web.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CookieSteps
    {
        private readonly ScenarioContext _context;
        private const string ProtectorPurpose = "CookieStorageService"; // note: this is copied from CookieStorageService

        public CookieSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given("I have a provider filters cookie")]
        [When("I have a provider filters cookie")]
        public void GivenIHaveAProviderFiltersCookie()
        {
            var protector = _context.Get<TestServer>(ContextKeys.TestServer).Services
                .GetService<IDataProtectionProvider>()
                .CreateProtector(ProtectorPurpose);

            var getProvidersRequest = new GetCourseProvidersRequest
            {
                Id = 324,
                Location = "Somewhere",
                DeliveryModes = new List<DeliveryModeType> {DeliveryModeType.BlockRelease},
                EmployerProviderRatings = new List<ProviderRating> {ProviderRating.Excellent},
                ApprenticeProviderRatings = new List<ProviderRating> { ProviderRating.Excellent }

            };

            var json = JsonConvert.SerializeObject(getProvidersRequest);

            var encoded = Convert.ToBase64String(
                protector.Protect(System.Text.Encoding.UTF8.GetBytes(json)));

            _context.Set($"{nameof(GetCourseProvidersRequest)}={encoded}", ContextKeys.ProviderFiltersCookie);
        }
    }
}
