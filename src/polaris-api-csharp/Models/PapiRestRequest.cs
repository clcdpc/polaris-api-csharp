using Clc.Rest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Models
{
    public class PapiRestRequest : RestRequest
    {
        public string Password { get; set; } = "";
        public bool AuthRequired { get; set; } = true;
        public bool JsonSerializerIgnoreNulls { get; set; } = true;
        public bool BlockStaffOverride = false;
        public string HashString { get; set; }

        public bool IsPublicMethod => Path.StartsWith("/public", StringComparison.OrdinalIgnoreCase);
        public bool IsProtectedMethod => Path.StartsWith("/protected", StringComparison.OrdinalIgnoreCase);

        public PapiRestRequest()
        {

        }

        public PapiRestRequest(HttpMethod method, string url, string password = "", object body = null) : base()
        {
            Method = method;
            Path = url;
            Password = password;
            Body = body;
        }

        public PapiRestRequest(string url, string password = "", object body = null) : this(HttpMethod.Get, url, password, body)
        {
        }

        public PapiRestRequest(RestRequest request)
        {
            Method = request.Method;
            Path = request.Path;
            Body = request.Body;
            Parameters = request.Parameters;
            Headers = request.Headers;
        }

        public override string ToString() => $"{Method} {Path} {Body}";
    }
}
