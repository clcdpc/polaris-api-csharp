namespace Clc.Polaris.Api.Models
{
    public class SynchTasksPullMARCData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string UniqueRecordID { get; set; }
    }

    public class SynchTasksPullMARCResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
