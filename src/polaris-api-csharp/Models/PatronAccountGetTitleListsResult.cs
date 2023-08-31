using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PatronAccountGetTitleListsResult : PapiResponseCommon
    {
        public List<PatronAccountTitleListsRow> PatronAccountTitleListsRows { get; set; }

        public override string ToString() => string.Join("\r\n", PatronAccountTitleListsRows?.Select(r => r));
    }

    public class PatronAccountTitleListsRow
    {
        public string RecordStoreName { get; set; }
        public bool Sorted { get; set; }
        public int Count { get; set; }
        public int RecordStoreId { get; set; }

        public override string ToString() => $"{RecordStoreId} - {RecordStoreName} - {Count}";
    }
}
