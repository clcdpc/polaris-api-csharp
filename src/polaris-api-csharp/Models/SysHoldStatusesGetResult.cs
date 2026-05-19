using System.Collections.Generic;
using System.Linq;

namespace Clc.Polaris.Api.Models
{
    public class SysHoldStatusesGetResult : PapiResponseCommon
    {
        public List<SysHoldStatusRow> SysHoldStatusesRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", SysHoldStatusesRows?.OrderBy(sh => sh.Name) ?? Enumerable.Empty<SysHoldStatusRow>());
        }
    }

    public class SysHoldStatusRow
    {
        public int SysHoldStatusID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString() => $"{SysHoldStatusID} - {Name}";
    }
}
