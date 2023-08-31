
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PickupBranchesRow
    {
        public int ID { get; set; }

        public override string ToString() => ID.ToString();
    }

    public class PickupBranchesGetResult : PapiResponseCommon
    {
        public List<PickupBranchesRow> PickupBranchesRows { get; set; }

        public List<int> PickupBranches => PickupBranchesRows.Select(b => b.ID).ToList();
    }
}
