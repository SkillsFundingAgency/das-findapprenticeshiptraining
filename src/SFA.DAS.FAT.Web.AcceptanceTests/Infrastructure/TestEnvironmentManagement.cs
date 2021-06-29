using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Courses.Api;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.MockServer;
using TechTalk.SpecFlow;
using WireMock.Server;

namespace SFA.DAS.FAT.Web.AcceptanceTests.Infrastructure
{
    [Binding]
    public sealed class TestEnvironmentManagement
    {
        private readonly ScenarioContext _context;
        private static HttpClient _staticClient;
        private static IWireMockServer _staticApiServer;
        private Mock<IApiClient> _mockApiClient;
        private static TestServer _server;

        public TestEnvironmentManagement(ScenarioContext context)
        {
            _context = context;
        }

        [BeforeScenario("WireMockServer")]
        public void StartWebApp()
        {
            _staticApiServer = MockApiServer.Start();
            var webApp = new CustomWebApplicationFactory<Startup>();
            _server = webApp.Server;
            _staticClient = new CustomWebApplicationFactory<Startup>().CreateClient(new WebApplicationFactoryClientOptions{HandleCookies = false});
            _context.Set(_server, ContextKeys.TestServer);
            _context.Set(_staticClient,ContextKeys.HttpClient);
        }

        [BeforeScenario("MockApiClient")]
        public void StartWebAppWithMockApiClient()
        {
            _mockApiClient = new Mock<IApiClient>();

            _mockApiClient.Setup(x => x.Get<TrainingCourses>(It.IsAny<GetCoursesApiRequest>()))
                .ReturnsAsync(new TrainingCourses());

            
            _server = new TestServer(new WebHostBuilder()
                .ConfigureTestServices(services => ConfigureTestServices(services, _mockApiClient))
                .UseStartup<Startup>()
                .UseConfiguration(ConfigBuilder.GenerateConfiguration()));

            _staticClient = _server.CreateClient();
            
            _context.Set(_mockApiClient, ContextKeys.MockApiClient);
            _context.Set(_staticClient,ContextKeys.HttpClient);
        }

        private void ConfigureTestServices(IServiceCollection serviceCollection,Mock<IApiClient> mockApiClient)
        {
            foreach(var descriptor in serviceCollection.Where(
                    d => d.ServiceType ==
                         typeof(IApiClient)).ToList())
            {
                serviceCollection.Remove(descriptor);
            }
            serviceCollection.AddSingleton(mockApiClient);
            serviceCollection.AddSingleton(mockApiClient.Object);
        }

        [AfterScenario("WireMockServer")]
        public void StopEnvironment()
        {
            _staticApiServer?.Stop();
            _staticClient?.Dispose();
        }
        
        [AfterScenario("MockApiClient")]
        public void StopTestEnvironment()
        {
            _server?.Dispose();
        }
    }
}
