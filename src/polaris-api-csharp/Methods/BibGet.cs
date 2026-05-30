using Clc.Rest;
using Clc.Polaris.Api.Models;
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
        /// <summary>
        /// Get information about a bibliographic record
        /// </summary>
        /// <param name="bibId"></param>
        /// <returns></returns>


        public async Task<IRestResponse<BibGetResult>> BibGetAsync(int bibId, int? branchId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/bib/{bibId}";
            var request = PapiRestRequest.Get(url);
            return await ExecutePapiAsync<BibGetResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
