using Clc.Polaris.Api.Configuration;
using Clc.Rest.Models;
using System.Net.Http;

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

        private int _userId = 1;
        private int _workstationId = 1;
        private int _organizationId = 1;

        public int UserId
        {
            get => _userId;
            set
            {
                Require.Positive(value);
                _userId = value;
            }
        }

        public int WorkstationId
        {
            get => _workstationId;
            set
            {
                Require.Positive(value);
                _workstationId = value;
            }
        }

        public int OrganizationId
        {
            get => _organizationId;
            set
            {
                Require.Positive(value);
                _organizationId = value;
            }
        }

        public bool AllowStaffOverrideRequests { get; set; } = true;

        /// <summary>
        /// The staff credentials used for protected methods and public method overrides
        /// </summary>
        public PolarisUser? StaffOverrideAccount { get; set; }

        public bool UseProtectedTokenCache { get; set; } = true;

        private ProtectedToken? _token;

        /// <summary>
        /// Used for protected methods and public method overrides
        /// </summary>
        public ProtectedToken? Token
        {
            get
            {
                if (!ProtectedTokenCache.IsUsable(_token))
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

                if (PapiRequestClassifier.IsStaffOverridePatronRequest(papiRequest, AllowStaffOverrideRequests, StaffOverrideAccount))
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
            PapiRequestValidator.ValidateExecutableRequest(request);

            var executionRequest = new PapiRestRequest(request);

            var pathContainsProtectedTokenPlaceholder = PapiRequestClassifier.PathContainsProtectedTokenPlaceholder(executionRequest);
            var requiresProtectedToken = PapiRequestClassifier.RequiresProtectedToken(executionRequest, pathContainsProtectedTokenPlaceholder, AllowStaffOverrideRequests, StaffOverrideAccount);

            var protectedToken = requiresProtectedToken
                ? await GetProtectedTokenOrThrowAsync(pathContainsProtectedTokenPlaceholder, cancellationToken).ConfigureAwait(false)
                : null;

            if (pathContainsProtectedTokenPlaceholder)
            {
                ReplaceProtectedTokenPlaceholderInPath(executionRequest, protectedToken!);
            }

            return await ExecuteAsync<T>(executionRequest, cancellationToken).ConfigureAwait(false);
        }

        private static InvalidOperationException CreateProtectedTokenRequiredException(string reason, bool pathContainsProtectedTokenPlaceholder)
        {
            var placeholderReason = pathContainsProtectedTokenPlaceholder
                ? " ProtectedToken.Placeholder cannot be replaced without a valid protected access token."
                : string.Empty;

            return new InvalidOperationException($"A valid protected access token is required for this request. {reason}{placeholderReason}");
        }

        private static void ReplaceProtectedTokenPlaceholderInPath(PapiRestRequest request, ProtectedToken token)
        {
            if (!ProtectedTokenCache.IsUsable(token))
            {
                throw new InvalidOperationException("A valid protected access token is required to replace ProtectedToken.Placeholder in the request path.");
            }

            request.Path = request.Path.Replace(ProtectedToken.Placeholder, token.AccessToken, StringComparison.Ordinal);
        }

        private async Task<ProtectedToken> GetProtectedTokenOrThrowAsync(bool pathContainsProtectedTokenPlaceholder, CancellationToken cancellationToken = default)
        {
            var token = Token;
            if (token != null)
            {
                return token;
            }

            if (StaffOverrideAccount == null)
            {
                throw CreateProtectedTokenRequiredException("No staff override credentials are configured.", pathContainsProtectedTokenPlaceholder);
            }

            var cacheKey = ProtectedTokenCache.BuildKey(Hostname, AccessID, AccessKey, StaffOverrideAccount);

            var protectedToken = await ProtectedTokenCache.GetOrCreateAsync(cacheKey, UseProtectedTokenCache, ct => AuthenticateProtectedTokenOrThrowAsync(pathContainsProtectedTokenPlaceholder, ct), cancellationToken).ConfigureAwait(false);

            _token = protectedToken;
            return protectedToken;
        }

        private async Task<ProtectedToken> AuthenticateProtectedTokenOrThrowAsync(bool pathContainsProtectedTokenPlaceholder, CancellationToken cancellationToken)
        {
            var staffOverrideAccount = StaffOverrideAccount ?? throw CreateProtectedTokenRequiredException("No staff override credentials are configured.", pathContainsProtectedTokenPlaceholder);

            var response = await AuthenticateStaffUserAsync(staffOverrideAccount, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var responseData = response?.Data;
            if (response?.Response == null || !response.Response.IsSuccessStatusCode)
            {
                throw CreateProtectedTokenRequiredException("Staff authentication did not succeed.", pathContainsProtectedTokenPlaceholder);
            }

            if (!ProtectedTokenCache.IsUsable(responseData))
            {
                throw CreateProtectedTokenRequiredException("Staff authentication did not return a usable protected access token.", pathContainsProtectedTokenPlaceholder);
            }

            return responseData!;
        }

        private static string EncodeBarcodePathSegment(string barcode) => WebUtility.UrlEncode(barcode);
    }
}
