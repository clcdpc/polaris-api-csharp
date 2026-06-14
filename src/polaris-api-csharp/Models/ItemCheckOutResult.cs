using System;

namespace Clc.Polaris.Api.Models
{
    public class ItemCheckOutResult : PapiResponseCommon
    {
        public int ItemRecordID { get; set; }
        public bool IsRenewal { get; set; }
        public DateTime? DueDate { get; set; }
        public double ChargeAmount { get; set; }
        public int PatronBlockFlags { get; set; }
        public int ItemBlockFlags { get; set; }
        public int RenewalBlockFlags { get; set; }
        public int MaterialTypeID { get; set; }
        public int SelfCheckMediaTypeID { get; set; }
        public bool IsMagnetic { get; set; }
        public bool CanDesensitize { get; set; }
        public bool DoubleSided { get; set; }
        public bool Unlocker { get; set; }
        public int DDM_MediaFormatID { get; set; }
        public string? Title { get; set; }
    }
}
