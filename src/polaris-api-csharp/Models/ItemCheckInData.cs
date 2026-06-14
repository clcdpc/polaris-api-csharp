namespace Clc.Polaris.Api.Models
{
    public class ItemCheckInData
    {
        public int LogonBranchID { get; set; } = 1;
        public int LogonUserID { get; set; } = 1;
        public int LogonWorkstationID { get; set; } = 1;

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
