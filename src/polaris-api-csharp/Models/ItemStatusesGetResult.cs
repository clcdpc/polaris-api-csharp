using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Api.Models
{
    public class ItemStatusesGetResult : PapiResponseCommon
    {
        public ItemStatusRow[] ItemStatusesRows { get; set; }
    }

    public class ItemStatusRow
    {
        public int ItemStatusId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string BannerText { get; set; }
    }
}
