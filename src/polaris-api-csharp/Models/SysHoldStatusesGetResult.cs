namespace Clc.Polaris.Api.Models
{
    public class SysHoldStatusesGetResult : PapiResponseCommon
    {
        public List<SysHoldStatusesRow> SysHoldStatusesRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", SysHoldStatusesRows ?? Enumerable.Empty<SysHoldStatusesRow>());
    }
}
