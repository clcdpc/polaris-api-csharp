
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public IRestResponse<PatronUpdateResult> PatronUpdate(string barcode, PatronUpdateParams updateParams, string password = "", bool ignoresa = true)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}?ignoresa={ignoresa}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = updateParams };
            return Execute<PatronUpdateResult>(request);
        }
    }
}
