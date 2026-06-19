namespace Clc.Polaris.Api.Models
{
    public class PatronTitleListAddTitleData
    {
        public int RecordStoreId { get; set; }
        public int LocalControlNumber { get; set; }

        public override string ToString() => $"{RecordStoreId} - {LocalControlNumber}";
    }
}
