using System;

namespace Clc.Polaris.Api.Models
{
    public class SynchTasksExpireCopyData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string UniqueRecordID { get; set; }
        public string PatronBarcode { get; set; }
        public string TransactionDateTime { get; set; }
    }

    public class SynchTasksExpireCopyResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
