using System;

namespace Clc.Polaris.Api.Models
{
    public class MaterialTypesGetResult : PapiResponseCommon
    {
        public MaterialTypesRow[] MaterialTypesRows { get; set; } = Array.Empty<MaterialTypesRow>();
    }

    public class MaterialTypesRow
    {
        public int MaterialTypeId { get; set; }
        public string? Description { get; set; }
    }
}
