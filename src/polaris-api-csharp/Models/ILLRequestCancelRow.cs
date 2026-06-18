namespace Clc.Polaris.Api.Models
{
    public class ILLRequestCancelRow
    {
        public int? ILLRequestID { get; set; }
        public int? ReturnCode { get; set; }
        public string? ErrorMessage { get; set; }

        public override string ToString() => $"{ILLRequestID?.ToString() ?? string.Empty} - {ReturnCode?.ToString() ?? string.Empty} - {ErrorMessage ?? string.Empty}";
    }
}
