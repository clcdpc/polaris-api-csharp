using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class BibGetByTypeResult : PapiResponseCommon
    {
        public List<BibGetByTypeRow> BibGetByTypeRows { get; set; }
    }

    public class BibGetByTypeRow
    {
        public int ElementID { get; set; }
        public int Occurence { get; set; }
        public int Occurrence { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Alternate { get; set; }
    }
}
