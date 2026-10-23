namespace Clc.Polaris.Api.Models
{
    public class SAMobilePhoneCarriersGetRow
    {
        public int? CarrierID { get; set; }
        public string? CarrierName { get; set; }
        public string? Email2SMSEmailAddress { get; set; }
        public int? NumberOfDigits { get; set; }
        public bool? Display { get; set; }

        public override string ToString() => $"{CarrierID?.ToString() ?? string.Empty} - {CarrierName ?? string.Empty}";
    }
}
