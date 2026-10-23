namespace Clc.Polaris.Api.Models
{
    public class ItemRenewResultWrapper : PapiResponseCommon
    {
        public ItemRenewResultBody? ItemRenewResult { get; set; }

        public override string ToString() => ItemRenewResult?.ToString() ?? base.ToString();
    }
    /// <summary>
    /// Lists of items that could and couldn't be renewed.
    /// </summary>
    public class ItemRenewResultBody
    {
        /// <summary>
        /// A list of items that could not be renewed.
        /// </summary>
        public List<ItemRenewBlockRow> BlockRows { get; set; } = new(); // = new List<ItemRenewBlockRow>();

        /// <summary>
        /// A list of successfully renewed items.
        /// </summary>
        public List<ItemRenewDueDateRow> DueDateRows { get; set; } = new(); // = new List<ItemRenewDueDateRow>();

        public override string ToString()
        {
            var rows = (BlockRows ?? Enumerable.Empty<ItemRenewBlockRow>()).Cast<object>()
                .Concat((DueDateRows ?? Enumerable.Empty<ItemRenewDueDateRow>()).Cast<object>());

            return string.Join("\r\n", rows);
        }
    }

    /// <summary>
    /// An item that could not be renewed by the Polaris API.
    /// </summary>
    public class ItemRenewBlockRow
    {
        /// <summary>
        /// A PAPI code to tell which 'process' caused the error
        /// 1 - Patron block
        /// 2 - Item renewal block
        /// </summary>
        public int PAPIErrorType { get; set; }

        /// <summary>
        /// The current error code return via Polaris base processing
        /// </summary>
        public int PolarisErrorCode { get; set; }

        /// <summary>
        /// Is the error overridable.
        /// </summary>
        public bool ErrorAllowOverride { get; set; }

        /// <summary>
        /// Description of the error.
        /// </summary>
        public string? ErrorDesc { get; set; }

        /// <summary>
        /// ID of the item record.
        /// </summary>
        public int ItemRecordID { get; set; }

        public override string ToString() => $"{ItemRecordID} - {PAPIErrorType} - {PolarisErrorCode} - {ErrorAllowOverride} - {ErrorDesc ?? string.Empty}";
    }

    /// <summary>
    /// An item that was successfully renewed by the Polaris API.
    /// </summary>
    public class ItemRenewDueDateRow
    {
        /// <summary>
        /// ID of the item record.
        /// </summary>
        public int ItemRecordID { get; set; }

        /// <summary>
        /// Due date of the item.
        /// </summary>
        public DateTime DueDate { get; set; }

        public override string ToString() => $"{ItemRecordID} - {DueDate:O}";
    }
}