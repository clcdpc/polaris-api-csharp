namespace Clc.Polaris.Api.Models
{
    public class ItemCheckInData
    {
        public int? LogonBranchID { get; set; }
        public int? LogonUserID { get; set; }
        public int? LogonWorkstationID { get; set; }

        public ItemCheckInData()
        {
        }

        public ItemCheckInData(int logonBranchId, int logonUserId, int logonWorkstationId)
        {
            LogonBranchID = logonBranchId;
            LogonUserID = logonUserId;
            LogonWorkstationID = logonWorkstationId;
        }
    }
}
