namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Contains the results of a hold request creation.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("HoldRequestResult")]
    public class HoldRequestCreateResult : PapiResponseCommon
    {
        private string? _txnGroupQualifier;
        private string? _legacyTxnGroupQualifier;

        /// <summary>
        /// Hold request GUID.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("RequestGUID")]
        [System.Text.Json.Serialization.JsonPropertyName("RequestGUID")]
        [System.Xml.Serialization.XmlIgnore]
        public Guid? RequestGuid { get; set; }

        /// <summary>
        /// XML boundary representation, because PAPI also returns an empty RequestGUID element.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [System.Xml.Serialization.XmlElement("RequestGUID")]
        public string? RequestGuidXml
        {
            get => RequestGuid?.ToString("D");
            set => RequestGuid = string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);
        }

        /// <summary>
        /// TxnGroupQualifier of the hold request.
        /// </summary>
        public string? TxnGroupQualifier
        {
            get => _txnGroupQualifier ?? _legacyTxnGroupQualifier;
            set => _txnGroupQualifier = value;
        }

        /// <summary>
        /// Legacy PAPI spelling of the transaction group qualifier. Prefer <see cref="TxnGroupQualifier"/>.
        /// </summary>
        public string? TxnGroupQualifer
        {
            get => _legacyTxnGroupQualifier;
            set => _legacyTxnGroupQualifier = value;
        }

        /// <summary>
        /// TxnQualifier of the hold request.
        /// </summary>
        public string? TxnQualifier { get; set; }

        /// <summary>
        /// Status type of the hold request.
        /// </summary>
        public int StatusType { get; set; }

        /// <summary>
        /// Status value of the hold request.
        /// </summary>
        public int StatusValue { get; set; }

        /// <summary>
        /// Display text.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Position of this hold in the queue.
        /// </summary>
        public int QueuePosition { get; set; }

        /// <summary>
        /// Retained source-compatible spelling. The PAPI response field is QueuePosition.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [System.Xml.Serialization.XmlIgnore]
        public int QueuePostition
        {
            get => QueuePosition;
            set => QueuePosition = value;
        }

        /// <summary>
        /// Total number of holds in the queue.
        /// </summary>
        public int QueueTotal { get; set; }

        public override string ToString() => RequestGuid?.ToString() ?? string.Empty;
    }
}
