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
        

        public IRestResponse<PatronBasicDataGetResult> PatronBasicDataGet(string barcode, string password = "", bool addresses = false, bool notes = false)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/basicdata?addresses={Convert.ToInt32(addresses)}&notes={Convert.ToInt32(notes)}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronBasicDataGetResult>(request);
        }
    }
}
