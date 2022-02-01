using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class HeaderCollection : IEnumerable<Header>
    {
        private readonly Dictionary<string, Header> headrCollection;

        public HeaderCollection()
        {
            headrCollection = new Dictionary<string, Header>();
        }

        public string this[string name]
            => headrCollection[name].Value;

        public int Count() => headrCollection.Count;

        public bool Contains(string name)
            => headrCollection.ContainsKey(name);

        public void Add(string name , string value)
        {
            headrCollection[name] = new Header(name, value);
        }

        public IEnumerator<Header> GetEnumerator() => headrCollection.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
