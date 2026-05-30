using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int[] bibIds, bool includeItems = false, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/synch/bibs/MARCxml";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("bibids", string.Join(",", bibIds));
            if (includeItems)
            {
                request.QueryParameters.Add("includeItems", 1);
            }
            return await ExecutePapiAsync<Sync_BibsByIdGetResult>(request, cancellationToken).ConfigureAwait(false);
        }

        public Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int bibId, bool includeItems = false, CancellationToken cancellationToken = default) => Synch_BibsByIdGetAsync(new int[] { bibId }, includeItems, cancellationToken);
    }
}
