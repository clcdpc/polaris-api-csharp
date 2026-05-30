using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

using Clc.Polaris.Api.Validation;
using System;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;

using System.Net.Http;
using Clc.Polaris.Models;

namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        public async Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateAsync(PatronRegistrationParams _params, CancellationToken cancellationToken = default)
        {
            var url = "/public/v1/1033/100/1/patron";
            var request = new PapiRestRequest(HttpMethod.Post, url) { BlockStaffOverride = true, Body = _params };
            return await ExecutePapiAsync<PatronRegistrationCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IRestResponse<PatronRegistrationCreateResult>> PatronRegistrationCreateV2Async(PatronRegistrationData _params, CancellationToken cancellationToken = default)
        {
            var url = "/public/v2/1033/100/1/patron";
            var request = new PapiRestRequest(HttpMethod.Post, url) { BlockStaffOverride = true, Body = _params };
            return await ExecutePapiAsync<PatronRegistrationCreateResult>(request, cancellationToken).ConfigureAwait(false);
        }
    }
}