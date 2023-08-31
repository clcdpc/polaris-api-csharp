using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="fromRecordStoreId"></param>
        /// <param name="fromPosition">starts at 1, not 0</param>
        /// <param name="toRecordStoreId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public IRestResponse<PapiResponseCommon> PatronTitleListCopyTitle(string barcode, int fromRecordStoreId, int fromPosition, int toRecordStoreId, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patrontitlelistcopytitle/";
            var body = new PatronTitleListCopyTitleData { FromRecordStoreId = fromRecordStoreId, FromPosition = fromPosition, ToRecordStoreId = toRecordStoreId };
            var request = new PapiRestRequest(HttpMethod.Post, url) { Password = password, Body = body };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
