using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using SFA.DAS.FAT.Application.Courses.Queries.GetCourses;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Web.Infrastructure;

namespace SFA.DAS.FAT.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private IDistributedCache _cache;
        private IConfiguration _configuration;

        public HomeController (IMediator mediator, IDistributedCache cache, IConfiguration configuration)
        {
            _mediator = mediator;
            _cache = cache;
            _configuration = configuration;
        }
        
        [Route("", Name = RouteNames.ServiceStartDefault, Order = 0)]
        [Route("start", Name = RouteNames.ServiceStart, Order = 1)]
        public IActionResult Index()
        {
            if (_configuration["Environment"].Equals("PROD"))
            {
                return RedirectPermanent("https://www.gov.uk/employers-find-apprenticeship-training");
            }

            return View();
        }

        [Route("cookies", Name = RouteNames.Cookies)]
        public IActionResult Cookies()
        {
            return View();
        }

        [Route("cookies-details", Name = RouteNames.CookiesDetails)]
        public IActionResult CookiesDetails()
        {
            return View();
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {

            var cacheContent = await _cache.GetAsync("Sitemap");
            if (cacheContent != null)
            {
                return new ContentResult
                {
                    Content = Encoding.UTF8.GetString(cacheContent),
                    ContentType = "application/xml",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            
            var result = await _mediator.Send(new GetCoursesQuery
            {
                Keyword = "",
                Routes = null,
                Levels = null,
                OrderBy = OrderBy.Title,
                ShortlistUserId = null
            });
            
            var urlList = new List<string>
            {
                Url.RouteUrl(RouteNames.ServiceStart,null,Request.Scheme, Request.Host.Host),
                Url.RouteUrl(RouteNames.Courses,null,Request.Scheme, Request.Host.Host),
            
            };
            urlList.AddRange(result.Standards.Select(course => Url.RouteUrl(RouteNames.CourseDetails, new { id = course.LarsCode }, Request.Scheme, Request.Host.Host)));
            urlList.AddRange(result.Standards.Select(course => Url.RouteUrl(RouteNames.CourseProviders, new { id = course.LarsCode }, Request.Scheme, Request.Host.Host)));

            urlList.Add(Url.RouteUrl(RouteNames.ShortList, null, Request.Scheme, Request.Host.Host));
            urlList.Add(Url.RouteUrl(RouteNames.Cookies, null, Request.Scheme, Request.Host.Host));
            urlList.Add(Url.RouteUrl(RouteNames.CookiesDetails, null, Request.Scheme, Request.Host.Host));
            urlList.Add(Url.RouteUrl(RouteNames.Privacy, null, Request.Scheme, Request.Host.Host));
            urlList.Add(Url.RouteUrl(RouteNames.AccessibilityStatement,null,Request.Scheme, Request.Host.Host));


            var output = new StringBuilder();
            var xml = XmlWriter.Create(output, new XmlWriterSettings { Indent = true, Async = true});
            await xml.WriteStartDocumentAsync();
            xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var url in urlList)
            {
                xml.WriteStartElement("url");
                xml.WriteElementString("loc", url);
                await xml.WriteEndElementAsync();    
            }
            
            await xml.WriteEndElementAsync();
            await xml.FlushAsync();
            xml.Dispose();

            var content = output.ToString();
            
            await _cache.SetAsync("Sitemap",Encoding.UTF8.GetBytes(content),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(20)}, CancellationToken.None);
            
            return new ContentResult
            {
                Content = content,
                ContentType = "application/xml",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}
