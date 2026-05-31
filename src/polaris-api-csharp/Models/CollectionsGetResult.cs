using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class CollectionsGetResult : PapiResponseCommon
    {
        public List<CollectionsRow> CollectionsRows { get; set; } = new List<CollectionsRow>();
    }

    public class CollectionsRow
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }

        public override string ToString() => $"{ID} - {Abbreviation} - {Name}";
    }
}
