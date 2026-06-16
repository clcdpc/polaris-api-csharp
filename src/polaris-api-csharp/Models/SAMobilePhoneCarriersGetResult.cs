namespace Clc.Polaris.Api.Models
{
    public class SAMobilePhoneCarriersGetResult : PapiResponseCommon
    {
        public List<SAMobilePhoneCarriersGetRow> SAMobilePhoneCarriersGetRows { get; set; } = new();
    }
}
