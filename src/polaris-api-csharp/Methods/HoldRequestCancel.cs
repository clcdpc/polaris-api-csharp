using Clc.Rest;
using Clc.Polaris.Api.Models;
using System.Net.Http;
using System.Threading.Tasks;
namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        public IRestResponse<HoldRequestCancelResult> HoldRequestCancel(string barcode, int requestId, string password = "", int? userId = null, int? workstationId = null)
        {
            var url = $"/public/v1/1033/100/1/patron/{barcode}/holdrequests/{requestId}/cancelled?wsid={workstationId ?? WorkstationId}&userid={userId ?? UserId}";
            var request = new PapiRestRequest(HttpMethod.Put, url) { Password = password };
            return Execute<HoldRequestCancelResult>(request);
        }
    }
}