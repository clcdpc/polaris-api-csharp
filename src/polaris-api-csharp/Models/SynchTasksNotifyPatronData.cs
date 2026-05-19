namespace Clc.Polaris.Api.Models
{
    public class SynchTasksNotifyPatronData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string PatronBarcode { get; set; }
        public string Message { get; set; }
    }

    public class SynchTasksNotifyPatronResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
