using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronBlocksAsync(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/{EncodeBarcodePathSegment(barcode)}/blocks";
            var body = new CreatePatronBlocksRequest((int)blockType, blockValue);
            var request = PapiRestRequest.Post(url, body: body);
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);

            return await ExecutePapiAsync<CreatePatronBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
