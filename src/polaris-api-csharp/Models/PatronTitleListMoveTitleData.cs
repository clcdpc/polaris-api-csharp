namespace Clc.Polaris.Api.Models
{
    public class PatronTitleListMoveTitleData
    {
        public int FromRecordStoreId { get; set; }
        public int FromPosition { get; set; }
        public int ToRecordStoreId { get; set; }

        public override string ToString() => $"{FromRecordStoreId} - {FromPosition} - {ToRecordStoreId}";
    }
}
