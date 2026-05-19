using System;
using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class GetBibsPagedResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public int LastID { get; set; }
        public List<GetBibsPagedRow> GetBibsPagedRows { get; set; }
    }

    public class GetBibsPagedRow
    {
        public int BibliographicRecordID { get; set; }
        public bool IsDisplayInPAC { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? FirstAvailableDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string BibliographicRecordXML { get; set; }
    }
}
