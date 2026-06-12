using System;
using System.Security.Cryptography;
using System.Text;

namespace Clc.Polaris.Api
{
    internal static class PapiSignature
    {
        internal static string ComputeHash(string accessKey, string httpMethod, string uri, string date, string password)
        {
            var hashString = httpMethod + uri + date + password;

            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));

            return Convert.ToBase64String(computedHash);
        }
    }
}
