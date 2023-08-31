using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class LimitFiltersGetResult : PapiResponseCommon
    {
        public LimitFiltersRow[] LimitFiltersRows { get; set; }
    }

    public class LimitFiltersRow
    {
        public string Description { get; set; }
        public string CCLFilter { get; set; }
        public int DisplayOrder { get; set; }

        public override string ToString()
        {
            return $"{Description} | {CCLFilter}";
        }
    }
}
