using Clc.Rest;
using Clc.Polaris.Api.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronBlocksAsync(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            var url = $"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/{WebUtility.UrlEncode(barcode)}/blocks";
            var body = new CreatePatronBlocksRequest((int)blockType, blockValue);
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = body };
            request.QueryParameters.Add("wsid", workstationId ?? WorkstationId);
            request.QueryParameters.Add("userid", userId ?? UserId);

            return await ExecutePapiAsync<CreatePatronBlocksResult>(request, cancellationToken).ConfigureAwait(false);
        }



        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronFreeTextBlockAsync(string barcode, string blockText, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            return await CreatePatronBlocksAsync(barcode, BlockType.FreeText, blockText, userId ?? UserId, workstationId ?? WorkstationId, cancellationToken).ConfigureAwait(false);
        }



        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronLibraryAssignedBlockAsync(string barcode, int blockId, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            return await CreatePatronBlocksAsync(barcode, BlockType.LibraryAssigned, blockId.ToString(), userId ?? UserId, workstationId ?? WorkstationId, cancellationToken).ConfigureAwait(false);
        }



        public async Task<IRestResponse<CreatePatronBlocksResult>> CreatePatronSystemBlockAsync(string barcode, SystemBlocks systemBlock, int? userId = null, int? workstationId = null, CancellationToken cancellationToken = default)
        {
            return await CreatePatronBlocksAsync(barcode, BlockType.System, ((int)systemBlock).ToString(), userId ?? UserId, workstationId ?? WorkstationId, cancellationToken).ConfigureAwait(false);
        }
    }
}
