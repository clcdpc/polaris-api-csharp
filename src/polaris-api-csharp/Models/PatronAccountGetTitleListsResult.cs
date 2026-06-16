
namespace Clc.Polaris.Api.Models
{
    public class PatronAccountGetTitleListsResult : PapiResponseCommon
    {
        public List<PatronAccountTitleListsRow> PatronAccountTitleListsRows { get; set; } = new();

        public override string ToString() => string.Join("\r\n", PatronAccountTitleListsRows);
    }

    public class PatronAccountTitleListsRow
    {
        public string? RecordStoreName { get; set; }
        public bool Sorted { get; set; }
        public int Count { get; set; }
        public int RecordStoreId { get; set; }

        public override string ToString() => $"{RecordStoreId} - {RecordStoreName} - {Count}";
    }
}
