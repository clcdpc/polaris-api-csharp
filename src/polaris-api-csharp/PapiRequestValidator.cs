
namespace Clc.Polaris.Api
{
    internal static class PapiRequestValidator
    {
        internal static void ValidateExecutableRequest(PapiRestRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

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

            if (!request.Path.StartsWith('/'))
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
    }
}
