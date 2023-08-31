
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
        /// <param name="listId"></param>
        /// <param name="position">starts at 1, not 0</param>
        /// <param name="password"></param>
        /// <returns></returns>
        

        public IRestResponse<PapiResponseCommon> PatronTitleListDeleteTitle(string barcode, int listId, int position, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/patrontitlelistdeletetitle?list={listId}&position={position}";
            var request = new PapiRestRequest(HttpMethod.Delete, url) { Password = password };
            return Execute<PapiResponseCommon>(request);
        }
    }
}
