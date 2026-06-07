using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<BibSearchResult>> BibSearchAsync(BibSearchOptions options, CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{options.Branch}/search/bibs/{options.SearchType}";
            if (options.SearchType == BibSearchTypes.keyword) { url += $"/{options.Qualifier}"; }

            var request = PapiRestRequest.Get(url);
            request.BlockStaffOverride = true;
            request.QueryParameters.Add("q", options.Term ?? string.Empty);
            request.QueryParameters.Add("sort", options.SortOption);
            request.QueryParameters.Add("page", options.Page);
            request.QueryParameters.Add("bibsperpage", options.PageSize);
            request.QueryParameters.Add("limit", options.Limit ?? string.Empty);

            return await ExecutePapiAsync<BibSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
