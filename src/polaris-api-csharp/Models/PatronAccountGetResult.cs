using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Result of a PatronAccountGet call
    /// </summary>
    [XmlRoot(ElementName = "PatronAccountGetResult")]
    public class PatronAccountGetResult : PapiResponseCommon
    {
        /// <summary>
        /// Rows of patron fine and fee information
        /// </summary>
        public List<PatronAccountGetRow> PatronAccountGetRows { get; set; }
    }

    /// <summary>
    /// Patron fine/fee information
    /// </summary>
    public class PatronAccountGetRow
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        public int TransactionID { get; set; }

        /// <summary>
        /// Transaction date
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Associated branch
        /// </summary>
        public int BranchID { get; set; }

        /// <summary>
        /// Associated branch
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Transaction type id
        /// </summary>
        public int TransactionTypeID { get; set; }

        /// <summary>
        /// Transaction type description
        /// </summary>
        public string TransactionTypeDescription { get; set; }

        /// <summary>
        /// Fee description
        /// </summary>
        public string FeeDescription { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public double TransactionAmount { get; set; }

        /// <summary>
        /// Outstanding amount
        /// </summary>
        public double OutstandingAmount { get; set; }

        /// <summary>
        /// Free text note
        /// </summary>
        public string FreeTextNote { get; set; }

        /// <summary>
        /// Item ID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Item barcode
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Item bibiographic record ID
        /// </summary>
        public int BibID { get; set; }

        /// <summary>
        /// Material type ID
        /// </summary>
        public int FormatID { get; set; }

        /// <summary>
        /// Material type description
        /// </summary>
        public string FormatDescription { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Call number
        /// </summary>
        public string CallNumber { get; set; }

        /// <summary>
        /// Checkout date
        /// </summary>
        public DateTime? CheckOutDate { get; set; }

        /// <summary>
        /// Due date
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
}
