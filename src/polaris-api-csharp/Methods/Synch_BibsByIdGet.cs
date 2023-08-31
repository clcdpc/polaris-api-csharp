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
        public IRestResponse<Sync_BibsByIdGetResult> Synch_BibsByIdGet(int[] bibIds)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/synch/bibs/MARCxml?bibids={string.Join(",", bibIds)}";
            var request = new PapiRestRequest(url);
            return Execute<Sync_BibsByIdGetResult>(request);
        }

        public IRestResponse<Sync_BibsByIdGetResult> Synch_BibsByIdGet(int bibId) => Synch_BibsByIdGet(new int[] { bibId });
    }
}
