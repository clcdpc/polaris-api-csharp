using System;
using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetSerialCompressedHoldingsRow
    {
        public int OrganizationID { get; set; }
        public int BibliographicRecordID { get; set; }
        public string CompressedStatement { get; set; }
        public List<PublicNote> PublicNotes { get; set; }
        public string RetentionNote { get; set; }
        public string LastReceivedIssue { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }

    public class PublicNote
    {
        public string Value { get; set; }
        public int DisplayOrder { get; set; }
    }
}
