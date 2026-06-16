
namespace Clc.Polaris.Api.Models
{
    public class SysHoldStatusesGetResult : PapiResponseCommon
    {
        public List<SysHoldStatusesRow> SysHoldStatusesRows { get; set; } = new();
    }
}
