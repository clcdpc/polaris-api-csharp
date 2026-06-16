namespace Clc.Polaris.Api.Models
{
    public class PickupBranchesRow
    {
        public int ID { get; set; }

        public override string ToString() => ID.ToString();
    }

    public class PickupBranchesGetResult : PapiResponseCommon
    {
        public List<PickupBranchesRow> PickupBranchesRows { get; set; } = new();

        public List<int> PickupBranches => PickupBranchesRows.Select(b => b.ID).ToList();
    }
}
