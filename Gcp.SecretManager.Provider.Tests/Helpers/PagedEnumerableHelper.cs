using Google.Api.Gax;
using System.Collections.Generic;

namespace Gcp.SecretManager.Provider.Tests.Helpers
{
    class PagedEnumerableHelper<TResponse, TResource> : PagedEnumerable<TResponse, TResource>
    {
        private IEnumerable<TResource> _resources;

        public PagedEnumerableHelper(IEnumerable<TResource> resources)
        {
            _resources = resources;
        }

        public override IEnumerator<TResource> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }
    }
}
