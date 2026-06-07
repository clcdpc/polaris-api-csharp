using Clc.Polaris.Api.Models;
using Clc.Rest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api
{
    public partial class PapiClient
    {
        /// <summary>
        /// Executes a custom PAPI request through the normal <see cref="PapiClient"/> request pipeline.
        /// Use this escape hatch for unsupported PAPI endpoints while still relying on centralized
        /// PAPI authentication, signing, staff override token behavior, protected token behavior,
        /// and URL construction.
        /// </summary>
        /// <typeparam name="T">The response body type to deserialize.</typeparam>
        /// <param name="request">
        /// A PAPI request using a relative PAPI path, such as
        /// <c>/public/v1/1033/100/1/...</c> or <c>/protected/v1/1033/100/1/...</c>.
        /// Absolute URLs, protocol-relative URLs, and paths that do not begin with <c>/</c> are rejected.
        /// </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>The deserialized REST response.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="request"/> does not use a safe relative PAPI path, or when an
        /// authenticated custom request targets a path outside <c>/public/</c> or <c>/protected/</c>.
        /// </exception>
        public Task<IRestResponse<T>> ExecutePapiAsync<T>(PapiRestRequest request, CancellationToken cancellationToken = default)
        {
            ValidateCustomPapiRequest(request);
            return ExecutePapiAsync<T>(request, cancellationToken, ProtectedTokenPreloadMode.Auto);
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

            if (request.AuthRequired
                && !request.Path.StartsWith("/public/", StringComparison.OrdinalIgnoreCase)
                && !request.Path.StartsWith("/protected/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Authenticated custom PAPI request paths must begin with '/public/' or '/protected/'.", nameof(request));
            }
        }
    }
}
