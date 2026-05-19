using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ItemCheckoutData
    {
        public string ItemBarcode { get; set; }
        public int LogonBranchID { get; set; }
        public int LogonUserID { get; set; }
        public int LogonWorkstationID { get; set; }
    }

    public class ItemCheckoutResult : PapiResponseCommon
    {
        public int ItemRecordID { get; set; }
        public bool IsRenewal { get; set; }
        public DateTime? DueDate { get; set; }
        public double ChargeAmount { get; set; }
        public long PatronBlockFlags { get; set; }
        public long ItemBlockFlags { get; set; }
        public long RenewalBlockFlags { get; set; }
        public int MaterialTypeID { get; set; }
        public int SelfCheckMediaTypeID { get; set; }
        public bool IsMagnetic { get; set; }
        public bool CanDesensitize { get; set; }
        public bool DoubleSided { get; set; }
        public bool Unlocker { get; set; }
        public int DDM_MediaFormatID { get; set; }
        public string Title { get; set; }
    }
}
