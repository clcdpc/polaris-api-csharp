using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class CollectionsGetResult : PapiResponseCommon
    {
        public List<CollectionsRow> CollectionsRows { get; set; } = new();
    }

    public class CollectionsRow
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }

        public override string ToString() => $"{ID} - {Abbreviation} - {Name}";
    }
}
