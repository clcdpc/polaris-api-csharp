using System;

namespace Clc.Polaris.Api.Models
{
    public class SynchTasksCheckinData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string UniqueRecordID { get; set; }
        public string PatronBarcode { get; set; }
        public string TransactionDateTime { get; set; }
        public string PatronVendorContractID { get; set; }
    }

    public class SynchTasksCheckinResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
