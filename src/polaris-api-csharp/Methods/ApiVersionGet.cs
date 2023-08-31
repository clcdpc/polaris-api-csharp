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
		/// Returns PAPI version information
		/// </summary>
		/// <returns>ApiVersionGetResponse</returns>
		/// <seealso cref="ApiResult"/>
		public IRestResponse<ApiResult> ApiVersionGet()
        {
            var url = "/public/v1/1033/100/1/api";
            var request = new PapiRestRequest(url);// { AuthRequired = true, BlockStaffOverride = true };
            return Execute<ApiResult>(request);
        }
    }
}
