using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Collections.Generic;
using System.Net;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<GetSerialCompressedHoldingsPagedResult> Synch_GetSerialCompressedHoldingsPaged(
            int lastbibid,
            int? lastorgid = null,
            int? nrecs = null,
            string startdatemodified = null,
            bool systemlevel = false)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/serialholdings/textual/compressed/paged?lastbibid={lastbibid}";
            var queryParams = new List<string>();
            if (lastorgid.HasValue && !systemlevel) queryParams.Add($"lastorgid={lastorgid}");
            if (nrecs.HasValue) queryParams.Add($"nrecs={nrecs}");
            if (!string.IsNullOrEmpty(startdatemodified)) queryParams.Add($"startdatemodified={WebUtility.UrlEncode(startdatemodified)}");
            if (systemlevel) queryParams.Add("systemlevel=true");
            else queryParams.Add("systemlevel=false");

            if (queryParams.Count > 0)
            {
                url += "&" + string.Join("&", queryParams);
            }
            var request = new PapiRestRequest(url);
            return Execute<GetSerialCompressedHoldingsPagedResult>(request);
        }
    }
}
