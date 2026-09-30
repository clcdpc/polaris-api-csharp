namespace Clc.Polaris.Api.Models
{
    public class MARCTypeOfMaterialsGetResult : PapiResponseCommon
    {
        public MARCTypeOfMaterialsRow[] MARCTypeOfMaterialsRows { get; set; } = Array.Empty<MARCTypeOfMaterialsRow>();

        public override string ToString() => string.Join("\r\n", MARCTypeOfMaterialsRows ?? Enumerable.Empty<MARCTypeOfMaterialsRow>());
    }

    public class MARCTypeOfMaterialsRow
    {
        public int MARCTypeOfMaterialId { get; set; }
        public string? SearchCode { get; set; }
        public string? Description { get; set; }

        public override string ToString() => $"{MARCTypeOfMaterialId} - {SearchCode} - {Description}";
    }
}
