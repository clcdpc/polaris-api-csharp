using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class PreformatRestRequestAuthorizationHashTests : RestClientMigrationTestBase
    {
                [TestMethod]
                public void PreformatRestRequest_PublicAuthenticatedGetWithoutQueryParameters_HashesOriginalUrl()
                {
                    var client = CreateClient();
                    var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, string.Empty);
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                    Assert.AreSame(request.Body, formatted.Body);
                }

                [TestMethod]
                public void PreformatRestRequest_ProtectedGet_UsesProtectedTokenSecretForHash()
                {
                    var client = CreateClient();
                    client.Token = new ProtectedToken
                    {
                        AccessToken = "protected-token",
                        AccessSecret = "protected-secret",
                        ExpirationDate = DateTime.Now.AddHours(1)
                    };
                    var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/1/protected-token/search/patrons/Boolean";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, "protected-secret");
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                    Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
                }

                [TestMethod]
                public void PreformatRestRequest_PublicAuthenticatedGetWithQueryParameters_HashesEffectiveOutgoingUrl()
                {
                    var client = CreateClient();
                    var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                    request.QueryParameters.Add("q", "harry potter & stone");
                    request.QueryParameters.Add("limit", "branch:1");
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&limit=branch%3A1";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, string.Empty);
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                }

                [TestMethod]
                public void PreformatRestRequest_AuthenticatedPutWithBodyAndQueryParameters_HashesQueryAndPreservesBody()
                {
                    var client = CreateClient();
                    var body = new PatronUpdateParams { EmailAddress = "patron@example.test" };
                    var request = new PapiRestRequest(HttpMethod.Put, "/public/v1/1033/100/1/patron/ABC123", "1234", body);
                    request.QueryParameters.Add("ignoresa", true);
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC123?ignoresa=True";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "PUT", expectedUri, date, "1234");
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                    Assert.AreSame(body, formatted.Body);
                }

                [TestMethod]
                public void PreformatRestRequest_AuthenticatedPostWithBodyAndQueryParameters_HashesQueryAndPreservesBody()
                {
                    var client = CreateClient();
                    var body = new { Amount = 12.34m };
                    var request = new PapiRestRequest(HttpMethod.Post, "/protected/v1/1033/100/1/token/patron/123/account/payment", "protected-secret", body);
                    request.QueryParameters.Add("wsid", 7);
                    request.QueryParameters.Add("userid", 8);
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"];
                    var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/1/token/patron/123/account/payment?wsid=7&userid=8";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "POST", expectedUri, date, "protected-secret");
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                    Assert.AreSame(body, formatted.Body);
                }

                [TestMethod]
                public void PreformatRestRequest_HashesQueryParameters_WithOutgoingUriEncoding()
                {
                    var client = CreateClient();
                    var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
                    request.QueryParameters.Add("q", "harry potter & stone");
        
                    var formatted = (PapiRestRequest)client.PreformatRestRequest(request);
        
                    var date = formatted.Headers["PolarisDate"].ToString();
                    var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone";
                    var expectedHash = PapiSignature.ComputeHash("access-key", "GET", expectedUri, date, string.Empty);
                    Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
                }
    }
}
