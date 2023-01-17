using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.FAT.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.FAT.Web.UnitTests.Services
{
    public class WhenCallingTheCookieStorageService
    {
        [Test, MoqAutoData]
        public void Then_The_Data_Is_Stored_For_A_Day(
            string testString,
            string testCookieName,
            Mock<IDataProtectionProvider> provider)
        {
            //Arrange
            var featureMock = new Mock<IHttpResponseFeature>();
            var mockHeaderDictionary = new HeaderDictionary();
            featureMock.Setup(x => x.Headers).Returns(mockHeaderDictionary);
            var responseMock = new FeatureCollection();
            responseMock.Set(featureMock.Object);
            var context = new DefaultHttpContext(responseMock);
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var service = new CookieStorageService<string>(mockHttpContextAccessor.Object, provider.Object);

            //Act
            service.Create(testString, testCookieName, 1);

            //Assert
            var actual = mockHeaderDictionary["set-cookie"].ToArray().First().Split(";");
            actual.First().Should().Contain(testCookieName);
            var actualExpiry = DateTime.Parse(actual.Skip(1).First().Split("=").Last());
            Assert.IsTrue(actualExpiry > DateTime.UtcNow.AddHours(23).AddMinutes(59));
        }

        [Test, AutoData]
        public void Then_The_Cookie_Data_Is_Retrieved(
            string testCookieName,
            string content)
        {
            //Arrange
            var mockDataProtector = new Mock<IDataProtector>();
            mockDataProtector.Setup(sut => sut.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)));
            var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
            mockDataProtectionProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockDataProtector.Object);
            var featureMock = new Mock<IRequestCookiesFeature>();
            var mockHeaderDictionary = new FakeCookieCollection(new Dictionary<string, string> { { testCookieName, Convert.ToBase64String(Encoding.UTF8.GetBytes(content)) } });
            featureMock.Setup(x => x.Cookies).Returns(mockHeaderDictionary);
            var responseMock = new FeatureCollection();
            responseMock.Set(featureMock.Object);
            var context = new DefaultHttpContext(responseMock);
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var service = new CookieStorageService<string>(mockHttpContextAccessor.Object, mockDataProtectionProvider.Object);

            //Act
            var actual = service.Get(testCookieName);

            //Assert
            actual.Should().NotBeNull();
            actual.Should().Be(content);
        }

        [Test, MoqAutoData]
        public void Then_The_Cookie_Is_Deleted_If_Exists(
            string testCookieName,
            string content,
            Mock<IDataProtectionProvider> provider)
        {
            // Arrange
            var featureMock = new Mock<IRequestCookiesFeature>();
            var responseCookiesFeature = new Mock<IResponseCookiesFeature>();
            var fakeCookieCollection = new FakeCookieCollection(new Dictionary<string, string> { { testCookieName, Convert.ToBase64String(Encoding.UTF8.GetBytes(content)) } });
            var fakeResponceCookies = new FakeResponseCookies(fakeCookieCollection.Store);
            featureMock.Setup(x => x.Cookies).Returns(fakeCookieCollection);
            responseCookiesFeature.Setup(x => x.Cookies).Returns(fakeResponceCookies);

            var responseMock = new FeatureCollection();
            responseMock.Set(featureMock.Object);
            responseMock.Set(responseCookiesFeature.Object);
            var context = new DefaultHttpContext(responseMock);
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var service = new CookieStorageService<string>(mockHttpContextAccessor.Object, provider.Object);

            //Act
            service.Delete(testCookieName);

            //Assert
            Assert.IsEmpty(fakeCookieCollection.Store);
        }
    }
}
