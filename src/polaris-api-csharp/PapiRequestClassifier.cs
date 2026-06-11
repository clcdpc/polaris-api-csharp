using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using System;
using System.Net.Http;

namespace Clc.Polaris.Api
{
    internal static class PapiRequestClassifier
    {
        internal static bool RequiresProtectedToken(
            PapiRestRequest request,
            bool pathContainsProtectedTokenPlaceholder,
            bool allowStaffOverrideRequests,
            PolarisUser? staffOverrideAccount)
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

            return IsStaffOverridePatronRequest(request, allowStaffOverrideRequests, staffOverrideAccount);
        }

        internal static bool IsStaffOverridePatronRequest(
            PapiRestRequest request,
            bool allowStaffOverrideRequests,
            PolarisUser? staffOverrideAccount)
        {
            return request.IsPublicMethod &&
                request.AuthRequired &&
                allowStaffOverrideRequests &&
                string.IsNullOrWhiteSpace(request.Password) &&
                !request.BlockStaffOverride &&
                staffOverrideAccount != null &&
                HasPatronBarcodeRouteSegment(request.Path);
        }

        internal static bool HasPatronBarcodeRouteSegment(string path)
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

        internal static bool IsStaffAuthenticatorRequest(PapiRestRequest? request)
        {
            var path = request?.Path;

            return request?.Method == HttpMethod.Post &&
                !string.IsNullOrWhiteSpace(path) &&
                path.StartsWith("/protected/", StringComparison.OrdinalIgnoreCase) &&
                path.EndsWith("/authenticator/staff", StringComparison.OrdinalIgnoreCase) &&
                path.IndexOf(ProtectedToken.Placeholder, StringComparison.Ordinal) < 0;
        }

        internal static bool PathContainsProtectedTokenPlaceholder(PapiRestRequest request)
        {
            return request.Path.Contains(ProtectedToken.Placeholder, StringComparison.Ordinal);
        }
    }
}
