using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.FAT.Web.UnitTests.Services
{
    public class FakeCookieCollection : IRequestCookieCollection
    {
        private Dictionary<string, string> _store { get; }

        public FakeCookieCollection()
        {
            _store = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Store => _store;

        public FakeCookieCollection(Dictionary<string, string> store)
        {
            _store = store;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _store.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _store.TryGetValue(key, out value); ;
        }

        public int Count => _store.Count;
        public ICollection<string> Keys => _store.Keys;

        public string this[string key] => _store[key];
    }
}
