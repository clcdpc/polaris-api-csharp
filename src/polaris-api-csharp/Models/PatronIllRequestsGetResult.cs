using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    
    public class PatronILLRequestsGetResult : PapiResponseCommon
    {
        public List<PatronILLRequestsGetRow> PatronILLRequestsGetRows { get; set; }
    }

    public class PatronILLRequestsGetRow
    {
        public int ILLRequestID { get; set; }
        public int ILLStatusID { get; set; }
        public int PatronID { get; set; }
        public int ItemRecordID { get; set; }
        public int BibRecordID { get; set; }
        public int PickupBranchID { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ActivationDate { get; set; }
        public string Status { get; set; }
        public string Item { get; set; }
        public DateTime NeedByDate { get; set; }
        public string PickupBranch { get; set; }
        public int FormatID { get; set; }
        public DateTime LastStatusTransitionDate { get; set; }
        public string OpacNotes { get; set; }
        public string CallNumber { get; set; }
        public string VolumeAndIssue { get; set; }
        public DateTime? PickupByDate { get; set; }
    }    
}
