using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ItemCheckInData
    {
        public int LogonBranchID { get; set; }
        public int LogonUserID { get; set; }
        public int LogonWorkstationID { get; set; }
    }

    public class ItemCheckInResult : PapiResponseCommon
    {
        public int ItemRecordID { get; set; }
        public DateTime? OriginalCheckOutDate { get; set; }
        public int AssignedBranchID { get; set; }
        public int? AssignedCollectionID { get; set; }
        public int ItemStatusID { get; set; }
        public int PreviousItemStatusID { get; set; }
        public string ItemBarcode { get; set; }
        public string Title { get; set; }
        public int MaterialTypeID { get; set; }
        public int SelfCheckMediaTypeID { get; set; }
        public bool IsMagnetic { get; set; }
        public bool CanDesensitize { get; set; }
        public bool DoubleSided { get; set; }
        public bool Unlocker { get; set; }
        public int DDM_MediaFormatID { get; set; }
        public string ShelfLocation { get; set; }
        public string CallNumber { get; set; }
        public string PatronBarcode { get; set; }
        public string Comment { get; set; }
        public ItemCheckInHoldData HoldData { get; set; }
        public ItemCheckInInTransitResult InTransitResult { get; set; }
    }

    public class ItemCheckInHoldData
    {
        public string TrappingPatronBarcode { get; set; }
        public string TrappingPatronName { get; set; }
        public string PickupBranchName { get; set; }
        public string PickupArea { get; set; }
    }

    public class ItemCheckInInTransitResult
    {
        public int InTransitSentBranchID { get; set; }
        public int InTransitRecvdBranchID { get; set; }
        public DateTime ItemStatusDate { get; set; }
        public DateTime InTransitSentDate { get; set; }
        public string Status { get; set; }
    }
}
