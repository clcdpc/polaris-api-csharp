namespace Clc.Polaris.Api.Models
{

    public class PatronMessagesGetResult : PapiResponseCommon
    {
        public Patronmessagesgetrow[] PatronMessagesGetRows { get; set; } = Array.Empty<Patronmessagesgetrow>();
    }

    public class Patronmessagesgetrow
    {
        public int Type { get; set; }
        public int ID { get; set; }
        public string? OrgName { get; set; }
        public DateTime Date { get; set; }
        public string? Value { get; set; }
        public bool IsRead { get; set; }

        public override string ToString() => $"{Date} - {OrgName} - {Value} - {IsRead}";
    }

}
