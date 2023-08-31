using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public IRestResponse<GetBarcodeAndPatronIDResult> Patron_GetBarcodeFromId(int patronId)
        {
            var url = $"/protected/v1/1033/100/1/{Token.AccessToken}/patron/barcode?patronid={patronId}";
            var request = new PapiRestRequest(url);
            return Execute<GetBarcodeAndPatronIDResult>(request);
        }
    }
}