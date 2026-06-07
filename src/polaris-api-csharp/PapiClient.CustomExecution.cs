using Clc.Polaris.Api.Models;
using Clc.Rest;
using Clc.Rest.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// Executes a custom PAPI request through the normal <see cref="PapiClient"/> request pipeline.
        /// </summary>
        /// <typeparam name="T">The expected response body model.</typeparam>
        /// <param name="request">
        /// The custom PAPI request to execute. Use a relative PAPI path beginning with <c>/</c>, such as
        /// <c>/public/v1/1033/100/1/...</c> or <c>/protected/v1/1033/100/1/...</c>.
        /// </param>
        /// <param name="cancellationToken">A token that can cancel protected-token acquisition and HTTP execution.</param>
        /// <returns>The REST response returned by the PAPI endpoint.</returns>
        /// <remarks>
        /// This method is an escape hatch for unsupported PAPI endpoints that do not yet have typed wrapper methods.
        /// The request still uses <see cref="PapiClient"/> authentication, signing, staff override, protected token,
        /// URL-building, and path-prefix behavior.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="request"/> does not contain a safe relative PAPI path, or when an authenticated
        /// request path is outside the <c>/public/</c> and <c>/protected/</c> PAPI scopes.
        /// </exception>
        public Task<IRestResponse<T>> ExecutePapiAsync<T>(PapiRestRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCustomPapiRequest(request);
            return ExecutePapiCoreAsync<T>(request, cancellationToken, ProtectedTokenPreloadMode.Auto);
        }

        private static void ValidateCustomPapiRequest(PapiRestRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.Path))
            {
                throw new ArgumentException("Custom PAPI request Path must not be null, empty, or whitespace.", nameof(request));
            }

            if (request.Path.StartsWith("//", StringComparison.Ordinal) ||
                Uri.IsWellFormedUriString(request.Path, UriKind.Absolute) ||
                !request.Path.StartsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("Custom PAPI request Path must be a relative PAPI path beginning with '/'.", nameof(request));
            }

            if (request.AuthRequired &&
                !request.Path.StartsWith("/public/", StringComparison.OrdinalIgnoreCase) &&
                !request.Path.StartsWith("/protected/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Authenticated custom PAPI request Path must start with '/public/' or '/protected/'.", nameof(request));
            }
        }
    }
}
