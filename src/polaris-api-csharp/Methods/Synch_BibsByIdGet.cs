using Clc.Rest;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<Sync_BibsByIdGetResult>> Synch_BibsByIdGetAsync(int[] bibIds, bool includeItems = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(bibIds);
            if (bibIds.Length == 0)
            {
                throw new ArgumentException("At least one bibliographic record ID is required.", nameof(bibIds));
            }

            foreach (var bibId in bibIds)
            {
                Require.Positive(bibId);
            }

            var url = $"/protected/v1/1033/100/{OrganizationId}/{ProtectedToken.Placeholder}/synch/bibs/MARCxml";
            var request = PapiRestRequest.Get(url);
            request.QueryParameters.Add("bibids", string.Join(",", bibIds));
            if (includeItems)
            {
                request.QueryParameters.Add("includeItems", 1);
            }
            return await ExecutePapiAsync<Sync_BibsByIdGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
