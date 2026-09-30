namespace Clc.Polaris.Api.Models
{
    public class PatronTitleListCopyAllTitlesData
    {
        public int FromRecordStoreId { get; set; }
        public int ToRecordStoreId { get; set; }

        public override string ToString() => $"{FromRecordStoreId} - {ToRecordStoreId}";
    }
}
