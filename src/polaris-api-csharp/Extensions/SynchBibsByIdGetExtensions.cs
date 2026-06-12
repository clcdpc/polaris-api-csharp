using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Rest;

namespace Clc.Polaris.Api
{
    public static class SynchBibsByIdGetExtensions
    {
        public static Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(
            this IPapiClient client,
            int bibId,
            bool includeItems = false,
            CancellationToken cancellationToken = default)
        {
            return client.Synch_BibsByIdGetAsync([bibId], includeItems, cancellationToken);
        }
    }
}
