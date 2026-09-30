namespace Clc.Polaris.Api.Models
{
    public class ShelfLocationsGetResult : PapiResponseCommon
    {
        public Shelflocationsrow[] ShelfLocationsRows { get; set; } = Array.Empty<Shelflocationsrow>();

        public override string ToString() => string.Join("\r\n", ShelfLocationsRows ?? Enumerable.Empty<Shelflocationsrow>());
    }

    public class Shelflocationsrow
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string? Description { get; set; }

        public override string ToString() => $"{ID} - {OrganizationID} - {Description}";
    }
}
