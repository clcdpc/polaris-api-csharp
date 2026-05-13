using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class PatronTitleListGetTitlesResult
    {
        public int PAPIErrorCode { get; set; }
        public object ErrorMessage { get; set; }
        public PatronTitleListTitleRow[] PatronTitleListTitleRows { get; set; }

        public override string ToString() => string.Join("\r\n", (object[])PatronTitleListTitleRows ?? Array.Empty<object>());
    }

    public class PatronTitleListTitleRow
    {
        public int Position { get; set; }
        public int RecordID { get; set; }
        public string Name { get; set; }
        public int LocalControlNumber { get; set; }

        public override string ToString() => $"{Position} - {RecordID} - {Name}";
    }
}
