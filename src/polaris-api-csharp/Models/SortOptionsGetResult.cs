using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class SortOptionsGetResult : PapiResponseCommon
    {
        public List<SortOptionsRow> SortOptionsRows { get; set; } = new();
    }
}
