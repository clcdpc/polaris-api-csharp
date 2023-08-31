using System;

namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// The result of an item renewal.
    /// </summary>
    public class ItemsOutActionResult : PapiResponseCommon
	{
		/// <summary>
		/// The result of the item renewal.
		/// </summary>
		public ItemRenewResultWrapper ItemRenewResult { get; set; }
	}
}