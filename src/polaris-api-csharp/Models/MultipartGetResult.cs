using System.Collections.Generic;

namespace Clc.Polaris.Api.Models
{
    public class MultipartGetResult : PapiResponseCommon
    {
        public List<MultipartRow> MultipartRows { get; set; } = new();
    }
}
