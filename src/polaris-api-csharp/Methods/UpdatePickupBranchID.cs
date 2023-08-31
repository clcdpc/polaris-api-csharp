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
        

        public IRestResponse<PapiResponseCommon> UpdatePickupBranchID(string barcode, int requestId, int pickupBranchId, string password = "", int? userId = null, int? workstationId = null)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/pickupbranch?userid={userId ?? UserId}&wsid={workstationId ?? WorkstationId}&pickupbranchid={pickupBranchId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
