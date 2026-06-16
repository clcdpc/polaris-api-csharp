using System.Collections.Generic;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.Models
{
    public class BibGetByTypeResult : PapiResponseCommon
    {
        public List<BibGetByTypeRow> BibGetByTypeRows { get; set; } = new();
    }
}
