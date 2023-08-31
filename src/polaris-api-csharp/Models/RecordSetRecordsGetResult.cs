using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public partial class RecordSetRecordsGetResult : PapiResponseCommon
    {
        public List<RecordSetRecordsGetRow> RecordSetRecordsGetRows { get; set; }

        public IEnumerable<int> Ids { get { return RecordSetRecordsGetRows.Select(r => r.RecordID); } }

        public override string ToString()
        {
            return string.Join(",", RecordSetRecordsGetRows.OrderBy(r => r.RecordID).Select(r => r.RecordID.ToString()));
        }
    }

    public partial class RecordSetRecordsGetRow
    {
        public int RecordID { get; set; }

        public override string ToString()
        {
            return RecordID.ToString();
        }
    }
}