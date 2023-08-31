using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        

        public IRestResponse<PatronValidateResult> PatronValidate(string barcode, string password = "")
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}";
            var request = new PapiRestRequest(url) { Password = password };
            return Execute<PatronValidateResult>(request);
        }
    }
}