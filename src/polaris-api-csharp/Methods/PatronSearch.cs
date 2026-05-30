using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {


        public async Task<IRestResponse<PatronSearchResult>> PatronSearchAsync(string query, int page = 1, int pageSize = 10, PatronSortKeys sortBy = PatronSortKeys.PATN, int? orgId = null, CancellationToken cancellationToken = default)
        {
            await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/search/patrons/Boolean";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("q", query);
            request.QueryParameters.Add("patronsperpage", pageSize);
            request.QueryParameters.Add("page", page);
            request.QueryParameters.Add("sort", sortBy);
            return await ExecutePapiAsync<PatronSearchResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}