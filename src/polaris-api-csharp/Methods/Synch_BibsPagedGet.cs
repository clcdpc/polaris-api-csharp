using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Collections.Generic;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetBibsPagedResult> Synch_BibsPagedGet(
            int? lastId = null,
            int? nrecs = null,
            string startdatecreated = null,
            string enddatecreated = null,
            string startdatemodified = null,
            string enddatemodified = null,
            bool includeItems = false)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/MARCXML/paged?";
            var queryParams = new List<string>();
            if (lastId.HasValue) queryParams.Add($"lastid={lastId}");
            if (nrecs.HasValue) queryParams.Add($"nrecs={nrecs}");
            if (!string.IsNullOrEmpty(startdatecreated)) queryParams.Add($"startdatecreated={startdatecreated}");
            if (!string.IsNullOrEmpty(enddatecreated)) queryParams.Add($"enddatecreated={enddatecreated}");
            if (!string.IsNullOrEmpty(startdatemodified)) queryParams.Add($"startdatemodified={startdatemodified}");
            if (!string.IsNullOrEmpty(enddatemodified)) queryParams.Add($"enddatemodified={enddatemodified}");
            if (includeItems) queryParams.Add("includeItems=1");

            url += string.Join("&", queryParams);
            var request = new PapiRestRequest(url);
            return Execute<GetBibsPagedResult>(request);
        }
    }
}
