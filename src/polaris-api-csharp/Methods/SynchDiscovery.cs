using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<AuthIDListGetResult> Synch_GetDeletedAuths(string deletedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/deleted?deletedate={WebUtility.UrlEncode(deletedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<AuthIDListGetResult>(request);
        }

        public IRestResponse<AuthIDListGetResult> Synch_GetDeletedAuthsPaged(string deletedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/deleted/paged?deletedate={WebUtility.UrlEncode(deletedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<AuthIDListGetResult>(request);
        }

        public IRestResponse<AuthIDListGetResult> Synch_GetUpdatedAuths(string updatedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/updated?updatedate={WebUtility.UrlEncode(updatedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<AuthIDListGetResult>(request);
        }

        public IRestResponse<AuthIDListGetResult> Synch_GetUpdatedAuthsPaged(string updatedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/auths/updated/paged?updatedate={WebUtility.UrlEncode(updatedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<AuthIDListGetResult>(request);
        }

        public IRestResponse<BibIDListGetResult> Synch_GetDeletedBibs(string deletedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/deleted?deletedate={WebUtility.UrlEncode(deletedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<BibIDListGetResult>(request);
        }

        public IRestResponse<BibIDListGetResult> Synch_GetDeletedBibsPaged(string deletedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/deleted/paged?deletedate={WebUtility.UrlEncode(deletedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<BibIDListGetResult>(request);
        }

        public IRestResponse<BibIDListGetResult> Synch_GetUpdatedBibs(string updatedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/updated?updatedate={WebUtility.UrlEncode(updatedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<BibIDListGetResult>(request);
        }

        public IRestResponse<BibIDListGetResult> Synch_GetUpdatedBibsPaged(string updatedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/updated/paged?updatedate={WebUtility.UrlEncode(updatedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<BibIDListGetResult>(request);
        }

        public IRestResponse<ItemIDListGetResult> Synch_GetDeletedItems(string deletedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/deleted?deletedate={WebUtility.UrlEncode(deletedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<ItemIDListGetResult>(request);
        }

        public IRestResponse<ItemIDListGetResult> Synch_GetDeletedItemsPaged(string deletedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/deleted/paged?deletedate={WebUtility.UrlEncode(deletedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<ItemIDListGetResult>(request);
        }

        public IRestResponse<ItemIDListGetResult> Synch_GetUpdatedItems(string updatedate, string enddate = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/updated?updatedate={WebUtility.UrlEncode(updatedate)}";
            if (!string.IsNullOrEmpty(enddate))
            {
                url += $"&enddate={WebUtility.UrlEncode(enddate)}";
            }
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<ItemIDListGetResult>(request);
        }

        public IRestResponse<ItemIDListGetResult> Synch_GetUpdatedItemsPaged(string updatedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/items/updated/paged?updatedate={WebUtility.UrlEncode(updatedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<ItemIDListGetResult>(request);
        }

        public IRestResponse<GetBarcodeAndPatronIDResult> Synch_GetDeletedPatrons(string deletedate)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/patrons/deleted?deletedate={WebUtility.UrlEncode(deletedate)}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<GetBarcodeAndPatronIDResult>(request);
        }

        public IRestResponse<GetBarcodeAndPatronIDResult> Synch_GetDeletedPatronsPaged(string deletedate, int lastid, int nrecs)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/patrons/deleted/paged?deletedate={WebUtility.UrlEncode(deletedate)}&lastid={lastid}&nrecs={nrecs}";
            var request = new PapiRestRequest(HttpMethod.Get, url);
            return Execute<GetBarcodeAndPatronIDResult>(request);
        }
    }
}
