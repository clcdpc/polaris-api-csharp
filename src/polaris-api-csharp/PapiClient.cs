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
using System.Threading.Tasks;
using System.Collections.Generic;

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
        private static ConcurrentDictionary<string, SemaphoreSlim> ProtectedTokenLocks { get; set; } = new ConcurrentDictionary<string, SemaphoreSlim>();

        private ProtectedToken _token;

        /// <summary>
        /// Used for protected methods and public method overrides
        /// </summary>
        public ProtectedToken Token
        {
            get { return _token; }
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


        private async Task<IRestResponse<T>> ExecutePapiAsync<T>(PapiRestRequest request, CancellationToken cancellationToken = default)
        {
            if (request.AuthRequired &&
                ((request.IsPublicMethod && AllowStaffOverrideRequests && string.IsNullOrWhiteSpace(request.Password) && !request.BlockStaffOverride) || request.IsProtectedMethod))
            {
                await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
            }

            return await ExecuteAsync<T>(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task<ProtectedToken> EnsureProtectedTokenAsync(CancellationToken cancellationToken = default)
        {
            if (StaffOverrideAccount == null || !IsProtectedTokenExpired(_token))
            {
                return _token;
            }

            var cacheKey = BuildProtectedTokenCacheKey();

            if (TryLoadProtectedTokenFromCache(cacheKey))
            {
                return _token;
            }

            if (!UseProtectedTokenCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                await LoadProtectedTokenAsync(cacheKey, cancellationToken).ConfigureAwait(false);
                return _token;
            }

            var tokenLock = ProtectedTokenLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
            await tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (!IsProtectedTokenExpired(_token) || TryLoadProtectedTokenFromCache(cacheKey))
                {
                    return _token;
                }

                await LoadProtectedTokenAsync(cacheKey, cancellationToken).ConfigureAwait(false);
                return _token;
            }
            finally
            {
                tokenLock.Release();
            }
        }

        private string BuildProtectedTokenCacheKey()
        {
            if (StaffOverrideAccount == null ||
                string.IsNullOrWhiteSpace(Hostname) ||
                string.IsNullOrWhiteSpace(StaffOverrideAccount.Domain) ||
                string.IsNullOrWhiteSpace(StaffOverrideAccount.Username))
            {
                return null;
            }

            return $"{Hostname}{StaffOverrideAccount.Domain}{StaffOverrideAccount.Username}";
        }

        private bool TryLoadProtectedTokenFromCache(string cacheKey)
        {
            if (!UseProtectedTokenCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return false;
            }

            if (ProtectedTokenCache.TryGetValue(cacheKey, out var cachedToken) && !IsProtectedTokenExpired(cachedToken))
            {
                _token = cachedToken;
                return true;
            }

            return false;
        }

        private async Task LoadProtectedTokenAsync(string cacheKey, CancellationToken cancellationToken)
        {
            var currentToken = _token;
            var response = await AuthenticateStaffUserAsync(StaffOverrideAccount, cancellationToken).ConfigureAwait(false);

            if (response?.Response == null || !response.Response.IsSuccessStatusCode || response.Data == null)
            {
                _token = currentToken;
                return;
            }

            _token = response.Data;

            if (UseProtectedTokenCache && !string.IsNullOrWhiteSpace(cacheKey))
            {
                ProtectedTokenCache[cacheKey] = new ProtectedToken(_token);
            }
        }

        private static bool IsProtectedTokenExpired(ProtectedToken token)
        {
            return token == null || token.ExpirationDate <= DateTime.Now;
        }

        private async Task<IRestResponse<T>> ExecuteStaffAuthenticationPostAsync<T>(string url, object body = null, CancellationToken cancellationToken = default)
        {
            // Staff authentication obtains the protected token, so this intentionally bypasses ExecutePapiAsync<T> token preloading.
            return await ExecuteAsync<T>(new PapiRestRequest(HttpMethod.Post, url) { Body = body }, cancellationToken).ConfigureAwait(false);
        }

        private string GetPAPIHash(string httpMethod, string date, string uri, string password)
        {
            var hashString = httpMethod + uri + date + password;
            byte[] computedHash = new HMACSHA1(Encoding.UTF8.GetBytes(AccessKey)).ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }
    }
}
