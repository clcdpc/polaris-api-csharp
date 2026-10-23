namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Result of a PatronTitleListAddTitle request
    /// </summary>
    public class PatronTitleListAddTitleResult : PapiResponseCommon
    {
        /// <summary>
        /// Position in the list
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Unique Identifier for the title in the list
        /// </summary>
        public int RecordID { get; set; }

        public override string ToString() => $"{Position} - {RecordID}";
    }
}
