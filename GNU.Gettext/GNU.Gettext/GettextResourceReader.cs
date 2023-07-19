using System.Collections;
using System.Resources;

namespace GNU.Gettext
{
    public class GettextResourceReader : IResourceReader
    {
        private readonly Hashtable _resources;

        public GettextResourceReader(Hashtable resources)
        {
            _resources = resources;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public void Close()
        {
            // No action needed
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public void Dispose()
        {
            _resources.Clear();
        }
    }
}
