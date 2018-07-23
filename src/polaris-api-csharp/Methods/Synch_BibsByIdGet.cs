using Clc.Polaris.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public PapiResponse<string> Synch_BibsByIdGet(int[] bibIds)
        {
            var url = $"/PAPIService/REST/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/MARCxml?bibids={string.Join(",", bibIds)}";
            return Execute<string>(HttpMethod.Get, url, pin: Token.AccessSecret);
        }

        public PapiResponse<string> Synch_BibsByIdGet(int bibId)
        {
            return Synch_BibsByIdGet(new int[] { bibId });
        }
    }
}
