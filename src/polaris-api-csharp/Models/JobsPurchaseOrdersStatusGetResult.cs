namespace Clc.Polaris.Api.Models
{
    public class JobsPurchaseOrdersStatusGetResult
    {
        public int PAPIErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? JobID { get; set; }
        public int? JobStatusID { get; set; }
        public string? JobStatusDescription { get; set; }
    }
}
