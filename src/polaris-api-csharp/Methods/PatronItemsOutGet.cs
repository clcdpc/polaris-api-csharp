using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public IRestResponse<PatronItemsOutGetResult> PatronItemsOutGet(string barcode, PatronItemsOutGetStatus status = PatronItemsOutGetStatus.All, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/itemsout/{status}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronItemsOutGetResult>(request);
        }
    }
}