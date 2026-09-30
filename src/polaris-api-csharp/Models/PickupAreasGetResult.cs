namespace Clc.Polaris.Api.Models
{
    public class PickupAreasGetResult : PapiResponseCommon
    {
        public List<PickupAreasRow> PickupAreasRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", PickupAreasRows ?? Enumerable.Empty<PickupAreasRow>());
    }

    public class PickupAreasRow
    {
        public int PickupAreaID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }

        public override string ToString() => $"{PickupAreaID} - {OrganizationID} - {Description ?? string.Empty}";
    }
}
