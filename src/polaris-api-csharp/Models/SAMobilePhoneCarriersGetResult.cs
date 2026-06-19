namespace Clc.Polaris.Api.Models
{
    public class SAMobilePhoneCarriersGetResult : PapiResponseCommon
    {
        public List<SAMobilePhoneCarriersGetRow> SAMobilePhoneCarriersGetRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", SAMobilePhoneCarriersGetRows ?? Enumerable.Empty<SAMobilePhoneCarriersGetRow>());
    }
}
