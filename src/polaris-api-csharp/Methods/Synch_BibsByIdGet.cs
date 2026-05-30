using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int[] bibIds, bool includeItems = false, CancellationToken cancellationToken = default)
        {
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/MARCxml";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("bibids", string.Join(",", bibIds));
            if (includeItems)
            {
                request.QueryParameters.Add("includeItems", 1);
            }
            return await ExecutePapiAsync<Sync_BibsByIdGetResult>(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int bibId, bool includeItems = false, CancellationToken cancellationToken = default) => await Synch_BibsByIdGetAsync(new int[] { bibId }, includeItems, cancellationToken).ConfigureAwait(false);
    }
}
