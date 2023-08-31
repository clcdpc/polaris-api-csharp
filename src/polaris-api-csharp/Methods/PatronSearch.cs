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
            var url = $"/protected/v1/1033/100/{orgId ?? OrganizationId}/{Token.AccessToken}/search/patrons/Boolean?q={WebUtility.UrlEncode(query)}&patronsperpage={pageSize}&page={page}&sort={sortBy}";
            var request = new PapiRestRequest(url);
            return Execute<PatronSearchResult>(request);
        }
    }
}