using System;
using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class BibGetByTypeResult : PapiResponseCommon { public List<BibGetByTypeRow> BibGetByTypeRows { get; set; } = new(); }
    public class BibGetByTypeRow { public int ElementID { get; set; } public int Occurrence { get; set; } public string? Label { get; set; } public string? Value { get; set; } public string? Alternate { get; set; } }

    public class MultipartGetResult : PapiResponseCommon { public MultipartRow[] MultipartRows { get; set; } = Array.Empty<MultipartRow>(); }
    public class MultipartRow { public string? Type { get; set; } public string? Value { get; set; } public string? ItemBarcode { get; set; } }

    public class HeadingsSearchResult : PapiResponseCommon { public HeadingsSearchRow[] HeadingsSearchRows { get; set; } = Array.Empty<HeadingsSearchRow>(); }
    public class HeadingsSearchRow { public int Position { get; set; } public string? DisplayType { get; set; } public string? DisplayConstant { get; set; } public string? DisplayTerm { get; set; } public int GlobalOccurrences { get; set; } public int HeadingID { get; set; } }

    public class SortOptionsGetResult : PapiResponseCommon { public SortOptionsRow[] SortOptionsRows { get; set; } = Array.Empty<SortOptionsRow>(); }
    public class SortOptionsRow { public string? Code { get; set; } public string? Description { get; set; } public string? Options { get; set; } }

    public class SysHoldStatusesGetResult : PapiResponseCommon { public SysHoldStatusesRow[] SysHoldStatusesRows { get; set; } = Array.Empty<SysHoldStatusesRow>(); }
    public class SysHoldStatusesRow { public int SysHoldStatusID { get; set; } public string? Description { get; set; } public string? Name { get; set; } }

    public class SAMobilePhoneCarriersGetResult : PapiResponseCommon { public SAMobilePhoneCarriersGetRow[] SAMobilePhoneCarriersGetRows { get; set; } = Array.Empty<SAMobilePhoneCarriersGetRow>(); }
    public class SAMobilePhoneCarriersGetRow { public int CarrierID { get; set; } public string? CarrierName { get; set; } public string? Email2SMSEmailAddress { get; set; } public int NumberOfDigits { get; set; } public bool Display { get; set; } }

    public class ILLRequestCancelResult : PapiResponseCommon { public List<ILLRequestCancelRow> ILLRequestCancelRows { get; set; } = new(); }
    public class ILLRequestCancelRow { public int ILLRequestID { get; set; } public int ReturnCode { get; set; } public string? ErrorMessage { get; set; } }

    public enum RequestStatusAction { Deny, Locate, Return, AskMeLater }
}
