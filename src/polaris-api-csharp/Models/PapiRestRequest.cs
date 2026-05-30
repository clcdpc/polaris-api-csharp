using Clc.Rest.Models;
using System;
using System.Net.Http;

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

        public static PapiRestRequest Get(string path, string password = "", object body = null) =>
            new PapiRestRequest(HttpMethod.Get, path, password, body);

        public static PapiRestRequest Delete(string path, string password = "", object body = null) =>
            new PapiRestRequest(HttpMethod.Delete, path, password, body);

        public static PapiRestRequest Post(string path, object body = null, string password = "") =>
            new PapiRestRequest(HttpMethod.Post, path, password, body);

        public static PapiRestRequest Put(string path, object body = null, string password = "") =>
            new PapiRestRequest(HttpMethod.Put, path, password, body);

        public static PapiRestRequest Create(HttpMethod method, string path, object body = null, string password = "") =>
            new PapiRestRequest(method, path, password, body);

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
            QueryParameters = request.QueryParameters;
            Headers = request.Headers;
        }

        public override string ToString() => $"{Method} {Path} {Body}";
    }
}
