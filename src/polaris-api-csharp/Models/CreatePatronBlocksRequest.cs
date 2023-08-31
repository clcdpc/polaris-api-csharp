using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class CreatePatronBlocksRequest
    {
        public int BlockTypeId { get; set; }
        public string BlockValue { get; set; }

        public CreatePatronBlocksRequest()
        {

        }

        public CreatePatronBlocksRequest(int blockTypeId, string blockValue)
        {
            BlockTypeId = blockTypeId;
            BlockValue = blockValue;
        }
    }
}
