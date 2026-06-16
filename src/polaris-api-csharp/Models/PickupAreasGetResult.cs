namespace Clc.Polaris.Api.Models
{
    public class PickupAreasGetResult : PapiResponseCommon
    {
        public List<PickupAreasRow> PickupAreasRows { get; set; } = new();
    }

    public class PickupAreasRow
    {
        public int PickupAreaID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }
    }
}
