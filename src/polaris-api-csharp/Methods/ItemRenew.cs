
using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        public IRestResponse<ItemRenewResultWrapper> ItemRenew(string barcode, int itemId, string password = "", ItemRenewOptions renewOptions = null)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/itemsout/{itemId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password, Body = renewOptions ?? new ItemRenewOptions() };
            return Execute<ItemRenewResultWrapper>(request);
        }
        public IRestResponse<ItemRenewResultWrapper> ItemRenewAllForPatron(string barcode, string password = "", ItemRenewOptions renewOptions = null)
            => ItemRenew(barcode, 0, password, renewOptions);
    }
}
