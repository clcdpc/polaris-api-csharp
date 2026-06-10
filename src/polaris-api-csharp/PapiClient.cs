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

        private static ConcurrentDictionary<string, ProtectedToken> ProtectedTokenCache { get; } = new ConcurrentDictionary<string, ProtectedToken>();
        private static SemaphoreSlim ProtectedTokenCacheLock { get; } = new SemaphoreSlim(1, 1);

        private ProtectedToken? _token;
        private static readonly TimeSpan ProtectedTokenExpirationSkew = TimeSpan.FromMinutes(1);

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
            internal set { _token = value; }
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
                ValidateConfigurationForAuthenticatedRequest();

                papiRequest.Headers.Remove("X-PAPI-AccessToken");

                var token = Token;
                var accessSecret = token?.AccessSecret;
                var accessToken = token?.AccessToken;

                if (IsStaffOverridePatronRequest(papiRequest))
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

                var date = DateTime.UtcNow.ToString("R");
                var requestUri = BuildRequestUri(papiRequest);
                var hash = PapiSignature.ComputeHash(AccessKey, papiRequest.Method.ToString(), requestUri.AbsoluteUri, date, password);
                papiRequest.Headers["PolarisDate"] = date;
                papiRequest.Headers["Authorization"] = string.Format("PWS {0}:{1}", AccessID, hash);
            }

            return papiRequest;
        }

        private void ValidateConfigurationForAuthenticatedRequest()
        {
            if (string.IsNullOrWhiteSpace(Hostname))
            {
                throw new InvalidOperationException($"{nameof(Hostname)} must be configured before executing authenticated PAPI requests.");
            }

            if (!Uri.TryCreate(Hostname, UriKind.Absolute, out var hostnameUri))
            {
                throw new InvalidOperationException($"{nameof(Hostname)} must be an absolute URL before executing authenticated PAPI requests.");
            }

            if (hostnameUri.Scheme != Uri.UriSchemeHttp && hostnameUri.Scheme != Uri.UriSchemeHttps)
            {
                throw new InvalidOperationException($"{nameof(Hostname)} must use the http or https scheme before executing authenticated PAPI requests.");
            }

            if (string.IsNullOrWhiteSpace(AccessID))
            {
                throw new InvalidOperationException($"{nameof(AccessID)} must be configured before executing authenticated PAPI requests.");
            }

            if (string.IsNullOrWhiteSpace(AccessKey))
            {
                throw new InvalidOperationException($"{nameof(AccessKey)} must be configured before executing authenticated PAPI requests.");
            }
        }

        /// <summary>
        /// Executes a caller-supplied <see cref="PapiRestRequest"/> through the normal PAPI pipeline as an
        /// escape hatch for unsupported or custom PAPI endpoints. Callers must pass a PAPI request whose
        /// path is a relative PAPI path, such as <c>/public/v1/1033/100/1/...</c> or
        /// <c>/protected/v1/1033/100/1/...</c>. Absolute URLs and protocol-relative URLs are rejected.
        /// Requests executed by this method still use the standard <see cref="PapiClient"/> processing,
        /// including protected-token acquisition, <see cref="ProtectedToken.Placeholder"/> replacement,
        /// staff override behavior, PolarisDate, Authorization signing, and URL construction/path prefix behavior.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="request">The custom PAPI request to execute.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>The REST response returned by the PAPI endpoint.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="request"/> has an invalid custom PAPI path.</exception>
        public async Task<IRestResponse<T>> ExecutePapiAsync<T>(PapiRestRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCustomPapiRequest(request);

            var executionRequest = new PapiRestRequest(request);

            var pathContainsProtectedTokenPlaceholder = RequestPathContainsProtectedTokenPlaceholder(executionRequest);
            var requiresProtectedToken = RequiresProtectedToken(executionRequest, pathContainsProtectedTokenPlaceholder);

            var protectedToken = requiresProtectedToken
                ? await GetProtectedTokenOrThrowAsync(pathContainsProtectedTokenPlaceholder, cancellationToken).ConfigureAwait(false)
                : null;

            if (pathContainsProtectedTokenPlaceholder)
            {
                ReplaceProtectedTokenPlaceholderInPath(executionRequest, protectedToken!);
            }

            return await ExecuteAsync<T>(executionRequest, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateCustomPapiRequest(PapiRestRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.Path))
            {
                throw new ArgumentException("PAPI request path must not be null, empty, or whitespace.", nameof(request));
            }

            if (request.Path.Contains("://", StringComparison.Ordinal))
            {
                throw new ArgumentException("PAPI request path must be a relative path, not an absolute URL.", nameof(request));
            }

            if (request.Path.StartsWith("//", StringComparison.Ordinal))
            {
                throw new ArgumentException("PAPI request path must not be a protocol-relative URL.", nameof(request));
            }

            if (!request.Path.StartsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("PAPI request path must begin with '/'.", nameof(request));
            }

            if (request.AuthRequired &&
                !request.Path.StartsWith("/public/", StringComparison.OrdinalIgnoreCase) &&
                !request.Path.StartsWith("/protected/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Authenticated custom PAPI request paths must begin with '/public/' or '/protected/'.", nameof(request));
            }
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

            return IsStaffOverridePatronRequest(request);
        }

        private bool IsStaffOverridePatronRequest(PapiRestRequest request)
        {
            return request.IsPublicMethod &&
                request.AuthRequired &&
                AllowStaffOverrideRequests &&
                string.IsNullOrWhiteSpace(request.Password) &&
                !request.BlockStaffOverride &&
                StaffOverrideAccount != null &&
                HasPatronBarcodeRouteSegment(request.Path);
        }

        private static bool HasPatronBarcodeRouteSegment(string path)
        {
            var pathWithoutQuery = path.Split('?', 2)[0];
            var segments = pathWithoutQuery.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < segments.Length - 1; i++)
            {
                if (segments[i].Equals("patron", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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

        private static InvalidOperationException CreateProtectedTokenRequiredException(string reason, bool pathContainsProtectedTokenPlaceholder)
        {
            var placeholderReason = pathContainsProtectedTokenPlaceholder
                ? " ProtectedToken.Placeholder cannot be replaced without a valid protected access token."
                : string.Empty;

            return new InvalidOperationException($"A valid protected access token is required for this request. {reason}{placeholderReason}");
        }

        private static bool RequestPathContainsProtectedTokenPlaceholder(PapiRestRequest request)
        {
            return request.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal);
        }

        private static void ReplaceProtectedTokenPlaceholderInPath(PapiRestRequest request, ProtectedToken token)
        {
            if (!IsProtectedTokenUsable(token))
            {
                throw new InvalidOperationException("A valid protected access token is required to replace ProtectedToken.Placeholder in the request path.");
            }

            request.Path = request.Path.Replace(ProtectedToken.Placeholder, token.AccessToken, StringComparison.Ordinal);
        }

        private async Task<ProtectedToken> GetProtectedTokenOrThrowAsync(bool pathContainsProtectedTokenPlaceholder, CancellationToken cancellationToken = default)
        {
            var token = Token;
            if (IsProtectedTokenUsable(token))
            {
                return token!;
            }

            if (token != null)
            {
                _token = null;
            }

            if (StaffOverrideAccount == null)
            {
                throw CreateProtectedTokenRequiredException("No staff override credentials are configured.", pathContainsProtectedTokenPlaceholder);
            }

            var cacheKey = BuildProtectedTokenCacheKey();

            if (TryLoadProtectedTokenFromCache(cacheKey, out var cachedToken))
            {
                return cachedToken!;
            }

            if (!UseProtectedTokenCache || string.IsNullOrWhiteSpace(cacheKey))
            {
                return await AuthenticateAndLoadProtectedTokenOrThrowAsync(null, pathContainsProtectedTokenPlaceholder, cancellationToken).ConfigureAwait(false);
            }

            await ProtectedTokenCacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                token = Token;
                if (IsProtectedTokenUsable(token))
                {
                    return token!;
                }

                if (token != null)
                {
                    _token = null;
                }

                if (TryLoadProtectedTokenFromCache(cacheKey, out cachedToken))
                {
                    return cachedToken!;
                }

                if (UseProtectedTokenCache)
                {
                    PruneProtectedTokenCache();
                }

                return await AuthenticateAndLoadProtectedTokenOrThrowAsync(cacheKey, pathContainsProtectedTokenPlaceholder, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ProtectedTokenCacheLock.Release();
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
            if (token == null || !token.ExpirationDate.HasValue)
            {
                return true;
            }

            var expirationUtc = NormalizeProtectedTokenExpirationUtc(token.ExpirationDate.Value);

            return expirationUtc <= DateTime.UtcNow.Add(ProtectedTokenExpirationSkew);
        }

        private static DateTime NormalizeProtectedTokenExpirationUtc(DateTime expirationDate)
        {
            return expirationDate.Kind switch
            {
                DateTimeKind.Utc => expirationDate,
                DateTimeKind.Local => expirationDate.ToUniversalTime(),
                _ => DateTime.SpecifyKind(expirationDate, DateTimeKind.Utc)
            };
        }

        private static bool IsProtectedTokenUsable(ProtectedToken? token)
        {
            return !IsProtectedTokenMissingOrExpired(token) &&
                !string.IsNullOrWhiteSpace(token?.AccessToken) &&
                !string.IsNullOrWhiteSpace(token.AccessSecret);
        }

        private bool TryLoadProtectedTokenFromCache(string? cacheKey, out ProtectedToken? protectedToken)
        {
            protectedToken = null;

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

            protectedToken = new ProtectedToken(cachedToken);
            _token = protectedToken;
            return true;
        }

        internal static int PruneProtectedTokenCache()
        {
            var removedCount = 0;

            foreach (var cachedToken in ProtectedTokenCache)
            {
                if (!IsProtectedTokenUsable(cachedToken.Value) &&
                    ProtectedTokenCache.TryRemove(cachedToken.Key, out _))
                {
                    removedCount++;
                }
            }

            return removedCount;
        }

        internal static void ClearProtectedTokenCache()
        {
            ProtectedTokenCache.Clear();
        }

        internal static void AddProtectedTokenToCacheForTesting(string cacheKey, ProtectedToken token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);
            ArgumentNullException.ThrowIfNull(token);

            ProtectedTokenCache[cacheKey] = new ProtectedToken(token);
        }

        internal static bool ProtectedTokenCacheContainsKey(string cacheKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);

            return ProtectedTokenCache.ContainsKey(cacheKey);
        }

        private async Task<ProtectedToken> AuthenticateAndLoadProtectedTokenOrThrowAsync(string? cacheKey, bool pathContainsProtectedTokenPlaceholder, CancellationToken cancellationToken)
        {
            var staffOverrideAccount = StaffOverrideAccount;
            if (staffOverrideAccount == null)
            {
                throw CreateProtectedTokenRequiredException("No staff override credentials are configured.", pathContainsProtectedTokenPlaceholder);
            }

            var response = await AuthenticateStaffUserAsync(staffOverrideAccount, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var responseData = response?.Data;
            if (response?.Response == null || !response.Response.IsSuccessStatusCode)
            {
                ClearFailedProtectedToken(cacheKey);
                throw CreateProtectedTokenRequiredException("Staff authentication did not succeed.", pathContainsProtectedTokenPlaceholder);
            }

            if (!IsProtectedTokenUsable(responseData))
            {
                ClearFailedProtectedToken(cacheKey);
                throw CreateProtectedTokenRequiredException("Staff authentication did not return a usable protected access token.", pathContainsProtectedTokenPlaceholder);
            }

            var protectedToken = responseData!;
            _token = protectedToken;

            if (UseProtectedTokenCache && !string.IsNullOrWhiteSpace(cacheKey))
            {
                ProtectedTokenCache[cacheKey] = new ProtectedToken(protectedToken);
            }

            return protectedToken;
        }

        private void ClearFailedProtectedToken(string? cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                ProtectedTokenCache.TryRemove(cacheKey, out _);
            }

            _token = null;
        }

    }
}
