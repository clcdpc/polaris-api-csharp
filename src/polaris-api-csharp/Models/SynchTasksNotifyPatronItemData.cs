namespace Clc.Polaris.Api.Models
{
    public class SynchTasksNotifyPatronItemData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string UniqueRecordID { get; set; }
        public string PatronBarcode { get; set; }
        public string Message { get; set; }
    }

    public class SynchTasksNotifyPatronItemResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
