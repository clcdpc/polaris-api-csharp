using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class MaterialTypesGetResult : PapiResponseCommon
    {
        public MaterialTypesRow[] MaterialTypesRows { get; set; }
    }

    public class MaterialTypesRow
    {
        public int MaterialTypeId { get; set; }
        public string Description { get; set; }
    }
}
