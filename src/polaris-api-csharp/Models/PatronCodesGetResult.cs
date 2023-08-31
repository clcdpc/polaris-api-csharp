using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PatronCodesGetResult : PapiResponseCommon
    {
        public List<PatronCodeRow> PatronCodesRows { get; set; }

        public override string ToString()
        {
            return string.Join("\r\n", PatronCodesRows?.OrderBy(pc => pc.Description));
        }
    }

    public class PatronCodeRow
    {
        public int PatronCodeID { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{PatronCodeID} - {Description}";
        }
    }
}
