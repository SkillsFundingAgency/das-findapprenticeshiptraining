using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace SFA.DAS.FAT.MockServer
{
    public static class MockApiServer
    {
        public static IWireMockServer Start()
        {
            var settings = new WireMockServerSettings
            {
                Port = 5003,
                Logger = new WireMockConsoleLogger()
            };
            
            var server = StandAloneApp.Start(settings);
           
            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s, "/trainingcourses/\\d+/providers/(?!(?:10000)$)\\d+$"))
                .WithParam(MatchLocationParamCoventry)
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-provider-details-notfound.json"));

            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s, "/trainingcourses/\\d+/providers/10000$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-provider-unavailable.json"));

            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s, "/trainingcourses/\\d+/providers/(?!(?:10000)$)\\d+$"))
                .WithParam(MatchLocationParam)
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-provider.json"));
            
            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s, "/trainingcourses/\\d+/providers/(?!(?:10000)$)\\d+$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-provider-nolocation.json"));


            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s,"/trainingcourses/(?!(?:102))\\d+/providers$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-providers-nolocation.json"));

            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s,"/trainingcourses/102/providers$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-providers-0-results.json"));
            
            server.Given(Request.Create()
                .WithPath(s => Regex.IsMatch(s,"/trainingcourses/(?!(?:102))\\d+/providers$"))
                .WithParam(MatchLocationParam)
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-providers.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"/trainingcourses/101$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-expired.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"/trainingcourses/24$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-lastdatestarts.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s, "/trainingcourses/333$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-regulated.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s, "/trainingcourses/102$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course-no-providers-at-location.json"));
                    
            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"/trainingcourses/(?!(?:101|102|24|333)$)\\d+$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("course.json"));

            server.Given(Request.Create().WithPath(IsLocation).UsingGet()).RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyFromFile("locations.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"/trainingcourses$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("courses.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"/shortlist/users/\\S+$"))
                .UsingGet()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyFromFile("shortlist.json"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"shortlist$"))
                .UsingPost()
                ).RespondWith(
                Response.Create()
                    .WithStatusCode(202)
                    .WithBody($"'{Guid.NewGuid().ToString()}'"));

            server.Given(Request.Create().WithPath(s => Regex.IsMatch(s,"shortlist/users/"))
                .UsingDelete()
            ).RespondWith(
                Response.Create()
                    .WithStatusCode(202));
            
            return server;
        }

        private static bool MatchLocationParam(IDictionary<string, WireMockList<string>> arg)
        {
            return arg.ContainsKey("location") && arg["location"].Count !=0 && !arg["location"][0].Equals("Coventry", StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool MatchLocationParamCoventry(IDictionary<string, WireMockList<string>> arg)
        {
            return arg.ContainsKey("location") && arg["location"].Count != 0 && arg["location"][0].Equals("Coventry", StringComparison.CurrentCultureIgnoreCase);        
        }

        private static bool IsLocation(string arg)
        {
            return Regex.IsMatch(arg, @"/locations");
        }
    }
}
