namespace Clc.Polaris.Api.Models
{
    public class MarcRecord
    {
        public string? Leader { get; set; }
        public List<MarcControlField>? ControlFields { get; set; }
        public List<MarcDataField>? DataFields { get; set; }
    }
}
