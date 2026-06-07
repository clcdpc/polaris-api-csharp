using Clc.Rest.Models;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient : RestClient, IPapiClient
    {
        /// <summary>
        /// Your PAPI Access ID
        /// </summary>
        public string AccessID { get; set; } = string.Empty;

        /// <summary>
        /// Your PAPI Access Key
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// The base URL of your PAPI service
        /// </summary>
        public string Hostname { get; set; } = string.Empty;

        public int UserId { get; set; } = 1;
        public int WorkstationId { get; set; } = 1;
        public int OrganizationId { get; set; } = 1;

        public bool AllowStaffOverrideRequests { get; set; } = true;

        /// <summary>
        /// The staff credentials used for protected methods and public method overrides
        /// </summary>
        public PolarisUser? StaffOverrideAccount { get; set; }

        public bool UseProtectedTokenCache { get; set; } = true;
        private static ConcurrentDictionary<string, ProtectedToken> ProtectedTokenCache { get; set; } = new ConcurrentDictionary<string, ProtectedToken>();
        private static ConcurrentDictionary<string, SemaphoreSlim> ProtectedTokenCacheLocks { get; set; } = new ConcurrentDictionary<string, SemaphoreSlim>();

        private ProtectedToken? _token;

        /// <summary>
        /// Used for protected methods and public method overrides
        /// </summary>
        public ProtectedToken? Token
        {
            get
            {
                if (IsProtectedTokenMissingOrExpired(_token))
                {
                    _token = null;
                    return null;
                }

                return _token;
            }
            set { _token = value; }
        }

        /// <summary>
        /// Initializes a new PAPI client. Configure TLS behavior on the injected <see cref="HttpClient"/>
        /// by supplying an <see cref="HttpClientHandler"/> with the desired settings.
        /// </summary>
        public PapiClient(HttpClient? client, IPapiSettings? settings) : base(null, client)
        {
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
            var papiRequest = request is PapiRestRequest existingRequest ? existingRequest : new PapiRestRequest(request);

            var password = papiRequest.Password;

            if (papiRequest.AuthRequired)
            {
                papiRequest.Headers.Remove("X-PAPI-AccessToken");

                var token = Token;
                var accessSecret = token?.AccessSecret;
                var accessToken = token?.AccessToken;

                if (papiRequest.IsPublicMethod && AllowStaffOverrideRequests && string.IsNullOrWhiteSpace(password) && !papiRequest.BlockStaffOverride)
                {
                    if (!string.IsNullOrWhiteSpace(accessSecret) && !string.IsNullOrWhiteSpace(accessToken))
                    {
                        password = accessSecret;
                        papiRequest.Headers["X-PAPI-AccessToken"] = accessToken;
                    }
                }

                if (papiRequest.IsProtectedMethod && string.IsNullOrWhiteSpace(password))
                {
                    if (!string.IsNullOrWhiteSpace(accessSecret))
                    {
                        password = accessSecret;
                    }
                }

                var date = DateTime.Now.ToUniversalTime().ToString("R");
                var requestUri = BuildRequestUri(papiRequest);
                var hash = GetPAPIHash(papiRequest.Method.ToString(), date, requestUri.AbsoluteUri, password);
                papiRequest.Headers["PolarisDate"] = date;
                papiRequest.Headers["Authorization"] = string.Format("PWS {0}:{1}", AccessID, hash);
            }

            return papiRequest;
        }

        private enum ProtectedTokenAcquisitionStatus
        {
            ValidTokenAvailable,
            NoTokenNeeded,
            NoStaffCredentialsConfigured,
            StaffAuthenticationFailed,
            InvalidTokenReturned
        }

        private sealed class ProtectedTokenAcquisitionResult
        {
            public ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus status, ProtectedToken? token = null)
            {
                Status = status;
                Token = token;
            }

            public ProtectedTokenAcquisitionStatus Status { get; }
            public ProtectedToken? Token { get; }
            public bool HasValidToken => Status == ProtectedTokenAcquisitionStatus.ValidTokenAvailable && IsProtectedTokenUsable(Token);
        }

        private async Task<IRestResponse<T>> ExecutePapiAsync<T>(PapiRestRequest request, CancellationToken cancellationToken = default)
        {
            var pathContainsProtectedTokenPlaceholder = RequestPathContainsProtectedTokenPlaceholder(request);
            var requiresProtectedToken = RequiresProtectedToken(request, pathContainsProtectedTokenPlaceholder);

            if (requiresProtectedToken)
            {
                var tokenResult = await EnsureProtectedTokenAsync(cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                if (!tokenResult.HasValidToken)
                {
                    ThrowProtectedTokenRequired(tokenResult.Status, pathContainsProtectedTokenPlaceholder);
                }
            }

            if (pathContainsProtectedTokenPlaceholder)
            {
                ReplaceProtectedTokenPlaceholderInPath(request);
            }

            return await ExecuteAsync<T>(request, cancellationToken).ConfigureAwait(false);
        }

        private bool RequiresProtectedToken(PapiRestRequest request, bool pathContainsProtectedTokenPlaceholder)
        {
            if (IsStaffAuthenticatorRequest(request))
            {
                return false;
            }

            if (pathContainsProtectedTokenPlaceholder)
            {
                return true;
            }

            if (request.IsProtectedMethod && string.IsNullOrWhiteSpace(request.Password))
            {
                return true;
            }

            return request.IsPublicMethod &&
                request.AuthRequired &&
                AllowStaffOverrideRequests &&
                string.IsNullOrWhiteSpace(request.Password) &&
                !request.BlockStaffOverride &&
                StaffOverrideAccount != null;
        }

        private static bool IsStaffAuthenticatorRequest(PapiRestRequest? request)
        {
            var path = request?.Path;

            return request?.Method == HttpMethod.Post &&
                !string.IsNullOrWhiteSpace(path) &&
                path.StartsWith("/protected/", StringComparison.OrdinalIgnoreCase) &&
                path.EndsWith("/authenticator/staff", StringComparison.OrdinalIgnoreCase) &&
                path.IndexOf(ProtectedToken.Placeholder, StringComparison.Ordinal) < 0;
        }

        private static void ThrowProtectedTokenRequired(ProtectedTokenAcquisitionStatus status, bool pathContainsProtectedTokenPlaceholder)
        {
            var reason = status switch
            {
                ProtectedTokenAcquisitionStatus.NoStaffCredentialsConfigured => "No staff override credentials are configured.",
                ProtectedTokenAcquisitionStatus.StaffAuthenticationFailed => "Staff authentication did not succeed.",
                ProtectedTokenAcquisitionStatus.InvalidTokenReturned => "Staff authentication did not return a usable protected access token.",
                _ => "A usable protected access token is not available."
            };

            var placeholderReason = pathContainsProtectedTokenPlaceholder
                ? " ProtectedToken.Placeholder cannot be replaced without a valid protected access token."
                : string.Empty;

            throw new InvalidOperationException($"A valid protected access token is required for this request. {reason}{placeholderReason}");
        }

        private static bool RequestPathContainsProtectedTokenPlaceholder(PapiRestRequest request)
        {
            return request?.Path?.IndexOf(ProtectedToken.Placeholder, StringComparison.Ordinal) >= 0;
        }

        private void ReplaceProtectedTokenPlaceholderInPath(PapiRestRequest request)
        {
            var token = Token;
            var accessToken = token?.AccessToken;
            if (IsProtectedTokenMissingOrExpired(token) || string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidOperationException("A valid protected access token is required to replace ProtectedToken.Placeholder in the request path.");
            }

            request.Path = request.Path.Replace(ProtectedToken.Placeholder, accessToken, StringComparison.Ordinal);
        }

        private async Task<ProtectedTokenAcquisitionResult> EnsureProtectedTokenAsync(CancellationToken cancellationToken = default)
        {
            var token = Token;
            if (IsProtectedTokenUsable(token))
            {
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.ValidTokenAvailable, token);
            }

            if (token != null)
            {
                _token = null;
            }

            if (StaffOverrideAccount == null)
            {
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.NoStaffCredentialsConfigured);
            }

            var cacheKey = BuildProtectedTokenCacheKey();
            if (TryLoadProtectedTokenFromCache(cacheKey))
            {
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.ValidTokenAvailable, _token);
            }

            if (!UseProtectedTokenCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return await AuthenticateAndLoadProtectedTokenAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            }

            var cacheLock = ProtectedTokenCacheLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
            await cacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                token = Token;
                if (IsProtectedTokenUsable(token))
                {
                    return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.ValidTokenAvailable, token);
                }

                if (token != null)
                {
                    _token = null;
                }

                if (TryLoadProtectedTokenFromCache(cacheKey))
                {
                    return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.ValidTokenAvailable, _token);
                }

                return await AuthenticateAndLoadProtectedTokenAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                cacheLock.Release();
            }
        }

        private string? BuildProtectedTokenCacheKey()
        {
            if (StaffOverrideAccount == null ||
                string.IsNullOrWhiteSpace(Hostname) ||
                string.IsNullOrWhiteSpace(AccessID) ||
                string.IsNullOrWhiteSpace(AccessKey) ||
                string.IsNullOrWhiteSpace(StaffOverrideAccount.Domain) ||
                string.IsNullOrWhiteSpace(StaffOverrideAccount.Username) ||
                string.IsNullOrWhiteSpace(StaffOverrideAccount.Password))
            {
                return null;
            }

            return $"{Hostname.Trim()}|{AccessID.Trim()}|{StaffOverrideAccount.Domain.Trim()}|{StaffOverrideAccount.Username.Trim()}|{BuildProtectedTokenCredentialFingerprint(AccessKey, StaffOverrideAccount.Password)}";
        }

        private static string BuildProtectedTokenCredentialFingerprint(string accessKey, string staffPassword)
        {
            var credentialMaterial = $"access-key:{accessKey.Length}:{accessKey}|staff-password:{staffPassword.Length}:{staffPassword}";
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(credentialMaterial));
            return Convert.ToHexString(hash);
        }

        private static bool IsProtectedTokenMissingOrExpired(ProtectedToken? token)
        {
            return token == null || !token.ExpirationDate.HasValue || token.ExpirationDate <= DateTime.Now;
        }

        private static bool IsProtectedTokenUsable(ProtectedToken? token)
        {
            return !IsProtectedTokenMissingOrExpired(token) &&
                !string.IsNullOrWhiteSpace(token?.AccessToken) &&
                !string.IsNullOrWhiteSpace(token.AccessSecret);
        }

        private bool TryLoadProtectedTokenFromCache(string? cacheKey)
        {
            if (!UseProtectedTokenCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return false;
            }

            if (!ProtectedTokenCache.TryGetValue(cacheKey, out var cachedToken))
            {
                return false;
            }

            if (!IsProtectedTokenUsable(cachedToken))
            {
                ProtectedTokenCache.TryRemove(cacheKey, out _);
                _token = null;
                return false;
            }

            _token = new ProtectedToken(cachedToken);
            return true;
        }

        private async Task<ProtectedTokenAcquisitionResult> AuthenticateAndLoadProtectedTokenAsync(string? cacheKey, CancellationToken cancellationToken)
        {
            var staffOverrideAccount = StaffOverrideAccount;
            if (staffOverrideAccount == null)
            {
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.NoStaffCredentialsConfigured);
            }

            var response = await AuthenticateStaffUserAsync(staffOverrideAccount, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var responseData = response?.Data;
            if (response?.Response == null || !response.Response.IsSuccessStatusCode)
            {
                ClearFailedProtectedToken(cacheKey);
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.StaffAuthenticationFailed);
            }

            if (!IsProtectedTokenUsable(responseData))
            {
                ClearFailedProtectedToken(cacheKey);
                return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.InvalidTokenReturned);
            }

            _token = responseData;

            if (UseProtectedTokenCache && !string.IsNullOrWhiteSpace(cacheKey))
            {
                ProtectedTokenCache[cacheKey] = new ProtectedToken(responseData!);
            }

            return new ProtectedTokenAcquisitionResult(ProtectedTokenAcquisitionStatus.ValidTokenAvailable, _token);
        }

        private void ClearFailedProtectedToken(string? cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                ProtectedTokenCache.TryRemove(cacheKey, out _);
            }

            _token = null;
        }

        private string GetPAPIHash(string httpMethod, string date, string uri, string password)
        {
            var hashString = httpMethod + uri + date + password;
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(AccessKey));
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }
    }
}
