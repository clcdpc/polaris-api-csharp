namespace Clc.Polaris.Api.Models
{
    public class CreatePatronBlocksRequest
    {
        public int BlockTypeId { get; set; }
        public string BlockValue { get; set; } = string.Empty;

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
