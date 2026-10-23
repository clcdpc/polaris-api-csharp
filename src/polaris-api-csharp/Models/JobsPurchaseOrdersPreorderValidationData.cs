namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersPreorderValidationData
    {
        public string? Vendor { get; set; }
        public string? OrderedAtLocation { get; set; }
        public string? OrderType { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ExternalID { get; set; }
        public int? Copies { get; set; }
        public string? ImportProfileName { get; set; }
        public List<JobsPurchaseOrdersPreorderValidationLineItem>? LineItems { get; set; }
    }
}
