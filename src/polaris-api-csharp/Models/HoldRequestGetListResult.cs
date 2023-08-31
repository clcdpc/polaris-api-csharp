using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class HoldRequestGetListResult : PapiResponseCommon
    {
        public int RecordCount { get; set; }
        public DateTime PickListCreateTime { get; set; }
        public DateTime PickListExpirationTime { get; set; }
        public Requestpicklistrow[] RequestPicklistRows { get; set; }
    }

    public class Requestpicklistrow
    {
        public int SysHoldRequestID { get; set; }
        public int SysHoldStatusID { get; set; }
        public string HoldStatus { get; set; }
        public int PickupBranchID { get; set; }
        public string PickupBranch { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime StatusDate { get; set; }
        public int BibliographicRecordID { get; set; }
        public int ConstituentBibRecordID { get; set; }
        public string BrowseAuthor { get; set; }
        public string BrowseTitle { get; set; }
        public int PrimaryMARCTOMID { get; set; }
        public string MarcTypeOfMaterial { get; set; }
        public bool ItemLevelHold { get; set; }
        public bool BorrowByMail { get; set; }
        public int PatronID { get; set; }
        public string PatronBarcode { get; set; }
        public int PatronBranchID { get; set; }
        public string PatronBranch { get; set; }
        public string PatronFullName { get; set; }
        public int PatronCodeID { get; set; }
        public string PatronCode { get; set; }
        public string EmailAddress { get; set; }
        public string AltEmailAddress { get; set; }
        public string PhoneVoice1 { get; set; }
        public string SMSAddress { get; set; }
        public int ItemRecordID { get; set; }
        public string ItemBarcode { get; set; }
        public int ItemBranchID { get; set; }
        public string ItemBranch { get; set; }
        public string CallNumber { get; set; }
        public string CopyNumber { get; set; }
        public string VolumeNumber { get; set; }
        public string Pages { get; set; }
        public int AssignedCollectionID { get; set; }
        public string CollectionName { get; set; }
        public int MaterialTypeID { get; set; }
        public string MaterialType { get; set; }
        public int ShelfLocationID { get; set; }
        public object ShelfLocation { get; set; }
        public int PublicationYear { get; set; }
        public string Publisher { get; set; }
        public string Designation { get; set; }
        public string Edition { get; set; }
        public string StaffDisplayNotes { get; set; }
        public string NonPublicNotes { get; set; }
        public string PACDisplayNotes { get; set; }
        public string SortTitle { get; set; }
        public string SortAuthor { get; set; }

        public override string ToString() => $"{ItemRecordID} - {ItemBarcode} - {BrowseTitle} - {PatronFullName}";
    }

}
