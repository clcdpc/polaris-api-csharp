using System;

namespace Clc.Polaris.Api.Models
{
    public class SynchTasksCheckoutData
    {
        public string VendorID { get; set; }
        public string VendorContractID { get; set; }
        public string UniqueRecordID { get; set; }
        public string PatronBarcode { get; set; }
        public string ItemExpireDateTime { get; set; }
        public string TransactionDateTime { get; set; }
        public string PatronVendorContractID { get; set; }
    }

    public class SynchTasksCheckoutResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
    }
}
