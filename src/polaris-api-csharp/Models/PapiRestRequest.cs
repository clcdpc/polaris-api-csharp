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
        public bool BlockStaffOverride { get; set; } = false;
        public string? HashString { get; set; }

        public bool IsPublicMethod => Path.StartsWith("/public", StringComparison.OrdinalIgnoreCase);
        public bool IsProtectedMethod => Path.StartsWith("/protected", StringComparison.OrdinalIgnoreCase);

        public static PapiRestRequest Get(string path, string password = "", object? body = null) =>
            new(HttpMethod.Get, path, password, body);

        public static PapiRestRequest Delete(string path, string password = "", object? body = null) =>
            new(HttpMethod.Delete, path, password, body);

        public static PapiRestRequest Post(string path, object? body = null, string password = "") =>
            new(HttpMethod.Post, path, password, body);

        public static PapiRestRequest Put(string path, object? body = null, string password = "") =>
            new(HttpMethod.Put, path, password, body);

        public static PapiRestRequest Create(HttpMethod method, string path, object? body = null, string password = "") =>
            new(method, path, password, body);

        public PapiRestRequest()
        {

        }

        public PapiRestRequest(HttpMethod method, string url, string password = "", object? body = null) : base()
        {
            Method = method;
            Path = url;
            Password = password;
            Body = body;
        }

        public PapiRestRequest(string url, string password = "", object? body = null) : this(HttpMethod.Get, url, password, body)
        {
        }

        public PapiRestRequest(RestRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            Method = request.Method;
            Path = request.Path;
            Body = request.Body;

            foreach (var parameter in request.QueryParameters)
            {
                QueryParameters[parameter.Key] = parameter.Value;
            }

            foreach (var header in request.Headers)
            {
                Headers[header.Key] = header.Value;
            }

            if (request is PapiRestRequest papiRequest)
            {
                Password = papiRequest.Password;
                AuthRequired = papiRequest.AuthRequired;
                JsonSerializerIgnoreNulls = papiRequest.JsonSerializerIgnoreNulls;
                BlockStaffOverride = papiRequest.BlockStaffOverride;
                HashString = papiRequest.HashString;
            }
        }

        public override string ToString() => $"{Method} {Path}";
    }
}
