namespace Clc.Polaris.Api.Models
{
    public class PatronNotesGetResult : PapiResponseCommon
    {
        public PatronNotes PatronNotes { get; set; }

        public override string ToString() => PatronNotes.ToString();
    }
}
