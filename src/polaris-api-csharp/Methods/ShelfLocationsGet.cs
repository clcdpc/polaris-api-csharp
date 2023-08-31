using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<ShelfLocationsGetResult> ShelfLocationsGet(int? branchId = null)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/shelflocations";
            var request = new PapiRestRequest(url) { BlockStaffOverride = true };
            return Execute<ShelfLocationsGetResult>(request);
        }
    }
}
