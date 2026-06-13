using Clc.Polaris.Api.Models;
using Clc.Rest;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {


        public async Task<IRestResponse<PapiResponseCommon>> PatronAccountCreateTitleListAsync(string barcode, string listName, string password = "", CancellationToken cancellationToken = default)
        {
            var url = $"/public/v1/1033/100/{OrganizationId}/patron/{EncodeBarcodePathSegment(barcode)}/patronaccountcreatetitlelist";
            var body = new PatronAccountCreateTitleListData { RecordStoreName = listName };
            var request = PapiRestRequest.Post(url, body: body, password: password);
            return await ExecutePapiAsync<PapiResponseCommon>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
