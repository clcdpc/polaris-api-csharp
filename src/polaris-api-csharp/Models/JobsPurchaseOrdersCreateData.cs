namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersCreateData
    {
        public string? Vendor { get; set; }
        public string? OrderedAtLocation { get; set; }
        public string? OrderType { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PONumber { get; set; }
        public string? PostbackURL { get; set; }
        public string? ExternalID { get; set; }
        public string? ImportProfileName { get; set; }
        public List<JobsPurchaseOrdersMarcLineItem>? MARCLineItems { get; set; }
    }
}
