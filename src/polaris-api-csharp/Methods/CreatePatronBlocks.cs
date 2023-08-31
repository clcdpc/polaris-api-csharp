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
        public IRestResponse<CreatePatronBlocksResult> CreatePatronBlocks(string barcode, BlockType blockType, string blockValue, int? userId = null, int? workstationId = null)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/{barcode}/blocks?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var body = new CreatePatronBlocksRequest((int)blockType, blockValue);
            var request = new PapiRestRequest(HttpMethod.Post, url) { Body = body };

            return Execute<CreatePatronBlocksResult>(request);
        }

        

        public IRestResponse<CreatePatronBlocksResult> CreatePatronFreeTextBlock(string barcode, string blockText, int? userId = null, int? workstationId = null)
        {
            return CreatePatronBlocks(barcode, BlockType.FreeText, blockText, workstationId ?? WorkstationId, userId ?? UserId);
        }

        

        public IRestResponse<CreatePatronBlocksResult> CreatePatronLibraryAssignedBlock(string barcode, int blockId, int? userId = null, int? workstationId = null)
        {
            return CreatePatronBlocks(barcode, BlockType.LibraryAssigned, blockId.ToString(), workstationId ?? WorkstationId, userId ?? UserId);
        }

        

        public IRestResponse<CreatePatronBlocksResult> CreatePatronSystemBlock(string barcode, SystemBlocks systemBlock, int? userId = null, int? workstationId = null)
        {
            return CreatePatronBlocks(barcode, BlockType.System, ((int)systemBlock).ToString(), workstationId ?? WorkstationId, userId ?? UserId);
        }
    }
}
