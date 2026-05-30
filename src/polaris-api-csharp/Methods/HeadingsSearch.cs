using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public Task<IRestResponse<BibHoldingsGetResult>> HeadingsSearchAsync(int bibId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("need to do this at some point");
        }
    }
}
