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
        /// Get information about a bibliographic record
        /// </summary>
        /// <param name="bibId"></param>
        /// <returns></returns>
        

        public IRestResponse<BibGetResult> BibGet(int bibId, int? branchId = null)
        {
            var url = $"/public/v1/1033/100/{branchId ?? OrganizationId}/bib/{bibId}";
            var request = new PapiRestRequest(url);
            return Execute<BibGetResult>(request);
        }
    }
}
