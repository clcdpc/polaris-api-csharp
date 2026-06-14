namespace Clc.Polaris.Api.Models
{
    public class ItemCheckOutData
    {
        public string ItemBarcode { get; set; } = string.Empty;
        public int LogonBranchID { get; set; } = 1;
        public int LogonUserID { get; set; } = 1;
        public int LogonWorkstationID { get; set; } = 1;

        public ItemCheckOutData()
        {
        }

        public ItemCheckOutData(string itemBarcode, int logonBranchId, int logonUserId, int logonWorkstationId)
        {
            ItemBarcode = itemBarcode;
            LogonBranchID = logonBranchId;
            LogonUserID = logonUserId;
            LogonWorkstationID = logonWorkstationId;
        }
    }
}
