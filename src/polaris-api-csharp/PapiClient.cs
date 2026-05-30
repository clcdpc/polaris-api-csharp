using Clc.Rest.Models;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient : RestClient, IPapiClient
    {
        /// <summary>
        /// Your PAPI Access ID
        /// </summary>
        public string AccessID { get; set; }

        /// <summary>
        /// Your PAPI Access Key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// The base URL of your PAPI service
        /// </summary>
        public string Hostname { get; set; }

        public int UserId { get; set; } = 1;
        public int WorkstationId { get; set; } = 1;
        public int OrganizationId { get; set; } = 1;

        public bool AllowStaffOverrideRequests { get; set; } = true;

        /// <summary>
        /// The staff credentials used for protected methods and public method overrides
        /// </summary>
        public PolarisUser StaffOverrideAccount { get; set; }

        public bool UseProtectedTokenCache { get; set; } = true;
        private static ConcurrentDictionary<string, ProtectedToken> ProtectedTokenCache { get; set; } = new ConcurrentDictionary<string, ProtectedToken>();

        public ProtectedToken _token;

        /// <summary>
        /// Used for protected methods and public method overrides
        /// </summary>
        public ProtectedToken Token
        {
            get
            {
                TryGetCachedProtectedToken();
                return _token;
            }
            set { _token = value; }
        }

        public PapiClient(HttpClient client, IPapiSettings settings) : base(null, client)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (settings != null)
            {
                AccessID = settings.AccessId;
                AccessKey = settings.AccessKey;
                Hostname = settings.Hostname;
                OrganizationId = settings.OrganizationId;
                UserId = settings.UserId;
                WorkstationId = settings.WorkstationId;
                StaffOverrideAccount = settings.PolarisOverrideAccount;
            } 
        }

        public PapiClient() : this(null, null) { }
        public PapiClient(IPapiSettings settings) : this(null, settings) { }

        // map BaseUrl to Hostname
        public override string BaseUrl { get => Hostname; set => Hostname = value; }
        public override string PathPrefix { get; set; } = "PAPIService/REST";

        public override RestRequest PreformatRestRequest(RestRequest request)
        {
            var papiRequest = request is PapiRestRequest ? request as PapiRestRequest : new PapiRestRequest(request);

            var password = papiRequest.Password;

            if (papiRequest.AuthRequired)
            {
                papiRequest.Headers.Remove("X-PAPI-AccessToken");

                if (papiRequest.IsPublicMethod && AllowStaffOverrideRequests && string.IsNullOrWhiteSpace(password) && !papiRequest.BlockStaffOverride)
                {
                    var token = Token;
                    if (token != null)
                    {
                        password = token.AccessSecret;
                        papiRequest.Headers["X-PAPI-AccessToken"] = token.AccessToken;
                    }
                }

                if (papiRequest.IsProtectedMethod && string.IsNullOrWhiteSpace(password) && _token != null)
                {
                    password = _token.AccessSecret;
                }

                var date = DateTime.Now.ToUniversalTime().ToString("R");
                var hash = GetPAPIHash(papiRequest.Method.ToString(), date, BuildUrlForPapiHash(papiRequest), password);
                papiRequest.Headers["PolarisDate"] = date;
                papiRequest.Headers["Authorization"] = string.Format("PWS {0}:{1}", AccessID, hash);
            }

            return papiRequest;
        }

        private string BuildUrlForPapiHash(RestRequest request)
        {
            var url = BuildUrl(request);
            if (request.QueryParameters.Count == 0)
            {
                return url;
            }

            var query = new StringBuilder();
            foreach (KeyValuePair<string, object> parameter in request.QueryParameters)
            {
                if (query.Length > 0)
                {
                    query.Append('&');
                }

                query.Append(Uri.EscapeDataString(parameter.Key));
                query.Append('=');
                query.Append(Uri.EscapeDataString(parameter.Value?.ToString() ?? string.Empty));
            }

            return $"{url}{(url.Contains("?") ? "&" : "?")}{query}";
        }

        private void TryGetCachedProtectedToken()
        {
            if (UseProtectedTokenCache && (_token == null || _token.ExpirationDate <= DateTime.Now) && StaffOverrideAccount != null)
            {
                ProtectedTokenCache.TryGetValue($"{Hostname}{StaffOverrideAccount.Domain}{StaffOverrideAccount.Username}", out _token);
            }
        }

        private async Task EnsureProtectedTokenAsync(CancellationToken cancellationToken = default)
        {
            TryGetCachedProtectedToken();

            if ((_token == null || _token.ExpirationDate <= DateTime.Now) && StaffOverrideAccount != null)
            {
                var response = await AuthenticateStaffUserAsync(StaffOverrideAccount, cancellationToken).ConfigureAwait(false);
                _token = response?.Data;

                if (UseProtectedTokenCache && response?.Response.IsSuccessStatusCode == true && _token != null)
                {
                    ProtectedTokenCache[$"{Hostname}{StaffOverrideAccount.Domain}{StaffOverrideAccount.Username}"] = new ProtectedToken(_token);
                }
            }
        }

        private async Task<IRestResponse<T>> ExecutePapiAsync<T>(RestRequest request, CancellationToken cancellationToken = default)
        {
            if (request is PapiRestRequest papiRequest
                && papiRequest.AuthRequired
                && (papiRequest.IsProtectedMethod || (papiRequest.IsPublicMethod && AllowStaffOverrideRequests && string.IsNullOrWhiteSpace(papiRequest.Password) && !papiRequest.BlockStaffOverride))
                && !papiRequest.Path.EndsWith("/authenticator/staff", StringComparison.OrdinalIgnoreCase))
            {
                await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            }

            return await ExecuteAsync<T>(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IRestResponse<T>> PostAsync<T>(string url, object body = null, CancellationToken cancellationToken = default)
        {
            return await ExecutePapiAsync<T>(new PapiRestRequest(HttpMethod.Post, url) { Body = body }, cancellationToken).ConfigureAwait(false);
        }

        private string GetPAPIHash(string httpMethod, string date, string uri, string password)
        {
            var hashString = httpMethod + uri + date + password;
            byte[] computedHash = new HMACSHA1(Encoding.UTF8.GetBytes(AccessKey)).ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }
    }
}
