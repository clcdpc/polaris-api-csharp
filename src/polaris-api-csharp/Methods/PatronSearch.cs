using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public IRestResponse<PatronSearchResult> PatronSearch(string query, int page = 1, int pageSize = 10, PatronSortKeys sortBy = PatronSortKeys.PATN, int? orgId = null)
        {
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/search/patrons/Boolean";
            var request = new PapiRestRequest(url);
            request.QueryParameters.Add("q", query);
            request.QueryParameters.Add("patronsperpage", pageSize);
            request.QueryParameters.Add("page", page);
            request.QueryParameters.Add("sort", sortBy);
            return Execute<PatronSearchResult>(request);
        }
    }
}