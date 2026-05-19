using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersPostData
    {
        public string Vendor { get; set; }
        public string OrderedAtLocation { get; set; }
        public int OrderType { get; set; } = 3;
        public int PaymentMethod { get; set; } = 8;
        public string PONumber { get; set; }
        public string PostbackURL { get; set; }
        public string ExternalID { get; set; }
        public string ImportProfileName { get; set; }
        public List<MARCLineItem> MARCLineItems { get; set; }
    }

    public class MARCLineItem
    {
        public string ExternalLineItemID { get; set; }
        public int Copies { get; set; }
        public MARCRecord MARC { get; set; }
    }

    public class MARCRecord
    {
        public string Leader { get; set; }
        public List<ControlField> Controlfields { get; set; }
        public List<DataField> Datafields { get; set; }
    }

    public class ControlField
    {
        public string Tag { get; set; }
        public string Data { get; set; }
    }

    public class DataField
    {
        public string Tag { get; set; }
        public string Ind1 { get; set; }
        public string Ind2 { get; set; }
        public List<SubField> Subfields { get; set; }
    }

    public class SubField
    {
        public string Code { get; set; }
        public string Data { get; set; }
    }

    public class JobsPurchaseOrdersPostResult : PapiResponseCommon
    {
        public string ExternalID { get; set; }
        public Guid JobGuid { get; set; }
        public int JobStatusID { get; set; }
        public string JobStatusDescription { get; set; }
        public List<string> LineItemValidationErrors { get; set; }
    }

    public class JobsPurchaseOrdersPutData
    {
        public string Vendor { get; set; }
        public string OrderedAtLocation { get; set; }
        public int OrderType { get; set; } = 3;
        public int PaymentMethod { get; set; } = 8;
        public string ExternalID { get; set; }
        public int Copies { get; set; }
        public string ImportProfileName { get; set; }
        public List<LineItem> LineItems { get; set; }
    }

    public class LineItem
    {
        public string ExternalLineItemID { get; set; }
        public string Copies { get; set; }
        public List<LineItemSegment> LineItemSegments { get; set; }
    }

    public class LineItemSegment
    {
        public string Location { get; set; }
        public string Fund { get; set; }
        public string Collection { get; set; }
        public int Copies { get; set; }
    }

    public class JobsPurchaseOrdersStatusResult : PapiResponseCommon
    {
        public string JobID { get; set; }
        public int JobStatusID { get; set; }
        public string JobStatusDescription { get; set; }
    }

    public class JobsPurchaseOrdersResultData : PapiResponseCommon
    {
        public string ExternalID { get; set; }
        public string PONumber { get; set; }
        public int PurchaseOrderID { get; set; }
        public List<POLineItem> LineItems { get; set; }
        public List<LineItemError> LineItemErrors { get; set; }
        public List<string> ItemRecordCreateErrors { get; set; }
    }

    public class POLineItem
    {
        public int POLineItemID { get; set; }
        public int LineNumber { get; set; }
        public string ExternalLineItemID { get; set; }
        public int BibRecordID { get; set; }
        public List<POLineItemSegment> LineItemSegments { get; set; }
    }

    public class POLineItemSegment
    {
        public int POLineItemSegmentID { get; set; }
        public int POLISegmentNumber { get; set; }
        public string EDIPOLISegNum { get; set; }
        public string Location { get; set; }
        public string Fund { get; set; }
        public string Collection { get; set; }
        public string CallNumber { get; set; }
        public int Copies { get; set; }
    }

    public class LineItemError
    {
        public string ExternalLineItemID { get; set; }
        public string ErrorMessage { get; set; }
        public List<LineItemSegmentError> LineItemSegmentErrors { get; set; }
    }

    public class LineItemSegmentError
    {
        public string ErrorMessage { get; set; }
        public string Organization { get; set; }
        public string Collection { get; set; }
        public string Fund { get; set; }
        public int Quantity { get; set; }
        public string CallNumber { get; set; }
    }

    public class JobsPurchaseOrdersPreorderValidationResult : PapiResponseCommon
    {
        public string ExternalID { get; set; }
        public List<LineItemValidationError> LineItemValidationErrors { get; set; }
    }

    public class LineItemValidationError
    {
        public string ExternalLineItemID { get; set; }
        public List<LineItemValidationErrorDetail> Errors { get; set; }
    }

    public class LineItemValidationErrorDetail
    {
        public int PAPIErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
