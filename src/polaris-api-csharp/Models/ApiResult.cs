namespace Clc.Polaris.Api.Models
{
    public class ApiResult : PapiResponseCommon
    {
        public string? Version { get; set; }

        public override string ToString() => Version ?? string.Empty;
    }
}
