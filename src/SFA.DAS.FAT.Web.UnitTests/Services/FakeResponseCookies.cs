using Microsoft.AspNetCore.Http;

namespace SFA.DAS.FAT.Web.UnitTests.Services
{
    public class FakeResponseCookies : IResponseCookies
    {
        private Dictionary<string, string> _store { get; }

        public FakeResponseCookies(Dictionary<string, string> store)
        {
            _store = store;
        }

        public FakeResponseCookies()
        {
            _store = new Dictionary<string, string>();
        }

        public void Append(string key, string value)
        {
            _store.Add(key, value);
        }

        public void Append(string key, string value, CookieOptions options)
        {
            _store.Add(key, value);
        }

        public void Delete(string key)
        {
            _store.Remove(key);
        }

        public void Delete(string key, CookieOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
