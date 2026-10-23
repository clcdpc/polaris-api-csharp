namespace Clc.Polaris.Api.Models
{
    public class PurchaseOrderValidationError
    {
        public string? Field { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
