using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

using Clc.Polaris.Api.Validation;
using System;
using System.Threading.Tasks;
using Clc.Rest;
using Clc.Polaris.Api.Models;

using System.Net.Http;

namespace Clc.Polaris.Api
{
	public partial class PapiClient
    {
        

        public IRestResponse<PatronRegistrationCreateResult> PatronRegistrationCreate(PatronRegistrationParams _params)
        {
            var url = "/public/v1/1033/100/1/patron";
            var request = new PapiRestRequest(HttpMethod.Post, url) { BlockStaffOverride = true, Body = _params };
            return Execute<PatronRegistrationCreateResult>(request);
        }
    }
}