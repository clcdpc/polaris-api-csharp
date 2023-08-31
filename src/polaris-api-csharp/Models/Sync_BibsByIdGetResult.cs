using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class Sync_BibsByIdGetResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public GetBibsByIdRow[] GetBibsByIDRows { get; set; }
    }

    public class GetBibsByIdRow
    {
        public int BibliographicRecordID { get; set; }
        public string BibliographicRecordXML { get; set; }
        public bool IsDisplayInPAC { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime FirstAvailableDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }

}
