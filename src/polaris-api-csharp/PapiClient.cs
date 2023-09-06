using Clc.Rest.Auth;
using Clc.Rest.Models;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Validation;
using Clc.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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
                if (UseProtectedTokenCache && (_token == null || _token?.ExpirationDate <= DateTime.Now) && StaffOverrideAccount != null)
                {
                    ProtectedTokenCache.TryGetValue($"{Hostname}{StaffOverrideAccount.Domain}{StaffOverrideAccount.Username}", out _token);
                }

                if ((_token == null || _token?.ExpirationDate <= DateTime.Now) && StaffOverrideAccount != null)
                {
                    var response = AuthenticateStaffUser(StaffOverrideAccount);
                    _token = response?.Data;

                    if (UseProtectedTokenCache && response.Response.IsSuccessStatusCode) { ProtectedTokenCache[$"{Hostname}{StaffOverrideAccount.Domain}{StaffOverrideAccount.Username}"] = new ProtectedToken(_token); }
                }
                return _token;
            }
            set { _token = value; }
        }

        public PapiClient() : this(new HttpClient())
        {

        }

        public PapiClient(HttpClient _client = null) : base(_client)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public PapiClient(IPapiSettings settings, HttpClient _client = null) : this(_client)
        {
            AccessID = settings.AccessId;
            AccessKey = settings.AccessKey;
            Hostname = settings.Hostname;
            StaffOverrideAccount = settings.PolarisOverrideAccount;
        }

        // map BaseUrl to Hostname
        public override string BaseUrl { get => Hostname; set => Hostname = value; }
        public override string PathPrefix { get; set; } = "PAPIService/REST";

        public override RestRequest PreformatRestRequest(RestRequest request)
        {
            var papiRequest = request is PapiRestRequest ? request as PapiRestRequest : new PapiRestRequest(request);

            var password = papiRequest.Password;

            if (papiRequest.AuthRequired)
            {
                if (papiRequest.IsPublicMethod && AllowStaffOverrideRequests && string.IsNullOrWhiteSpace(password) && !papiRequest.BlockStaffOverride && Token != null)
                {
                    password = Token.AccessSecret;
                    papiRequest.Headers.Add("X-PAPI-AccessToken", Token.AccessToken);
                }

                if (papiRequest.IsProtectedMethod && string.IsNullOrWhiteSpace(password) && _token != null)
                {
                    password = Token.AccessSecret;
                }

                var date = DateTime.Now.ToUniversalTime().ToString("R");
                var hash = GetPAPIHash(papiRequest.Method.ToString(), date, BuildUrl(papiRequest), password);
                papiRequest.Headers.Add("PolarisDate", date);
                papiRequest.Headers.Add("Authorization", string.Format("PWS {0}:{1}", AccessID, hash));
            }

            return papiRequest;
        }

        private string GetPAPIHash(string httpMethod, string date, string uri, string password)
        {
            var hashString = httpMethod + uri + date + password;
            byte[] computedHash = new HMACSHA1(Encoding.UTF8.GetBytes(AccessKey)).ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }
    }
}
