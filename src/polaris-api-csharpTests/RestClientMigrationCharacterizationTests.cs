using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("RestClientMigration")]
    public class RestClientMigrationCharacterizationTests
    {
        [TestMethod]
        public void PapiRestRequest_Constructors_PreserveCurrentBehavior()
        {
            var defaultGet = new PapiRestRequest("/public/foo");
            Assert.AreEqual(HttpMethod.Get, defaultGet.Method);
            Assert.AreEqual("/public/foo", defaultGet.Path);

            var body = new { Name = "Example" };
            var put = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);
            Assert.AreEqual(HttpMethod.Put, put.Method);
            Assert.AreEqual("/public/foo", put.Path);
            Assert.AreEqual("pin", put.Password);
            Assert.AreSame(body, put.Body);

            var existing = new RestRequest
            {
                Method = HttpMethod.Post,
                Path = "/protected/foo",
                Body = new { Value = "v" },
            };
            existing.QueryParameters.Add("limit", "5");
            existing.Headers.Add("X-Test", "header");

            var copied = new PapiRestRequest(existing);
            Assert.AreEqual(existing.Method, copied.Method);
            Assert.AreEqual(existing.Path, copied.Path);
            Assert.AreSame(existing.Body, copied.Body);
            Assert.AreSame(existing.QueryParameters, copied.QueryParameters);
            Assert.AreSame(existing.Headers, copied.Headers);
        }

        [TestMethod]
        public void PapiRestRequest_PathClassification_IsStable()
        {
            var publicRequest = new PapiRestRequest("/public/foo");
            Assert.IsTrue(publicRequest.IsPublicMethod);
            Assert.IsFalse(publicRequest.IsProtectedMethod);

            var protectedRequest = new PapiRestRequest("/protected/foo");
            Assert.IsFalse(protectedRequest.IsPublicMethod);
            Assert.IsTrue(protectedRequest.IsProtectedMethod);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsPapiHeaders_WhenAuthRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual(HttpMethod.Get, formatted.Method);
            Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
            Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddPapiHeaders_WhenAuthNotRequired()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
            {
                AuthRequired = false
            };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_PublicAuthenticatedGetWithoutQueryParameters_HashesOriginalUrl()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
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
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/protected/v1/1033/100/1/protected-token/search/patrons/Boolean";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, "protected-secret", "access-key");
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
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
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
            var expectedHash = ComputePapiHash("PUT", expectedUri, date, "1234", "access-key");
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
            var expectedHash = ComputePapiHash("POST", expectedUri, date, "protected-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
            Assert.AreSame(body, formatted.Body);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsStaffOverrideToken_WhenAllowedAndUnblocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsTrue(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.AreEqual("staff-token", formatted.Headers["X-PAPI-AccessToken"]);
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, "staff-secret", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddStaffOverrideToken_WhenBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate")
            {
                BlockStaffOverride = true
            };
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddStaffOverrideToken_WhenDisabled()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = false;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };

            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = formatted.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_RemovesStaleStaffOverrideToken_WhenOverrideBecomesBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var first = (PapiRestRequest)client.PreformatRestRequest(request);
            Assert.AreEqual("staff-token", first.Headers["X-PAPI-AccessToken"]);

            request.BlockStaffOverride = true;
            var second = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreSame(first, second);
            Assert.IsFalse(second.Headers.ContainsKey("X-PAPI-AccessToken"));
            var date = second.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate";
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", second.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_WhenCalledTwice_ReplacesPapiHeadersWithoutDuplicatingOrChangingBody()
        {
            var client = CreateClient();
            var body = new { Value = "unchanged" };
            var request = new PapiRestRequest(HttpMethod.Put, "/public/v1/1033/100/1/patron/ABC123", "1234", body);
            request.Headers.Add("X-Custom", "keep");
            request.QueryParameters.Add("ignoresa", false);

            var first = (PapiRestRequest)client.PreformatRestRequest(request);
            var firstHeaderCount = first.Headers.Count;
            var second = (PapiRestRequest)client.PreformatRestRequest(first);

            Assert.AreSame(first, second);
            Assert.AreEqual(firstHeaderCount, second.Headers.Count);
            Assert.AreEqual("keep", second.Headers["X-Custom"]);
            Assert.AreSame(body, second.Body);
            Assert.AreEqual(1, second.Headers.Keys.Count(key => key == "PolarisDate"));
            Assert.AreEqual(1, second.Headers.Keys.Count(key => key == "Authorization"));
            var date = second.Headers["PolarisDate"];
            var expectedUri = "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC123?ignoresa=False";
            var expectedHash = ComputePapiHash("PUT", expectedUri, date, "1234", "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", second.Headers["Authorization"]);
        }

        [TestMethod]
        public void PreformatRestRequest_AuthorizationChanges_WhenQueryParameterValueChanges()
        {
            var client = CreateClient();
            var first = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            first.QueryParameters.Add("q", "harry potter");
            var second = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            second.QueryParameters.Add("q", "lord of the rings");

            var firstFormatted = (PapiRestRequest)client.PreformatRestRequest(first);
            var secondFormatted = (PapiRestRequest)client.PreformatRestRequest(second);

            var firstDate = firstFormatted.Headers["PolarisDate"];
            var secondDate = secondFormatted.Headers["PolarisDate"];
            var firstExpectedHash = ComputePapiHash("GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter", firstDate, string.Empty, "access-key");
            var secondExpectedHash = ComputePapiHash("GET", "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=lord%20of%20the%20rings", secondDate, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{firstExpectedHash}", firstFormatted.Headers["Authorization"]);
            Assert.AreEqual($"PWS access-id:{secondExpectedHash}", secondFormatted.Headers["Authorization"]);
            Assert.AreNotEqual(firstFormatted.Headers["Authorization"], secondFormatted.Headers["Authorization"]);
        }

        [TestMethod]
        public async Task ApiKeyValidate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.ApiKeyValidateAsync();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/apikeyvalidate");
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }


        [TestMethod]
        public async Task ApiKeyValidateAsync_PassesCancellationToken()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            using var cts = new CancellationTokenSource();

            var response = await client.ApiKeyValidateAsync(cts.Token);

            Assert.IsNotNull(response);
            Assert.IsTrue(handler.LastCancellationToken.CanBeCanceled);
        }

        [TestMethod]
        public async Task BibSearch_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            var options = new BibSearchOptions
            {
                Branch = 1,
                SearchType = BibSearchTypes.keyword,
                Qualifier = SearchQualifiers.KW,
                Term = "harry potter & stone",
                SortOption = SearchSortOptions.MP,
                Page = 2,
                PageSize = 15,
                Limit = "3"
            };

            var response = await client.BibSearchAsync(options);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            Assert.AreEqual("https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&page=2&bibsperpage=15&limit=3", handler.LastRequest.RequestUri.AbsoluteUri);
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("harry potter & stone", query["q"]);
            Assert.AreEqual("MP", query["sort"]);
            Assert.AreEqual("2", query["page"]);
            Assert.AreEqual("15", query["bibsperpage"]);
            Assert.AreEqual("3", query["limit"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public async Task AuthenticateStaffUser_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0,\"AccessToken\":\"t\",\"AccessSecret\":\"s\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}");
            var client = CreateClient(handler);

            var response = await client.AuthenticateStaffUserAsync(new PolarisUser
            {
                Domain = "main",
                Username = "staff",
                Password = "secret"
            });

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent, "main");
            StringAssert.Contains(handler.LastRequestContent, "staff");
            StringAssert.Contains(handler.LastRequestContent, "secret");
        }

        [TestMethod]
        public async Task PatronUpdate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.PatronUpdateAsync("AB C/+#?=", new PatronUpdateParams { EmailAddress = "patron@example.test" }, "1234", ignoresa: true);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Put, handler.LastRequest!.Method);
            var encodedBarcode = WebUtility.UrlEncode("AB C/+#?=");
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, $"/public/v1/1033/100/1/patron/{encodedBarcode}");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("True", query["ignoresa"]);
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsNotNull(handler.LastRequestContent);
            StringAssert.Contains(handler.LastRequestContent, "patron@example.test");
        }


        [TestMethod]
        public async Task PatronSearch_RequestShape_PreservesEncodedQuerySemantics()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "token", AccessSecret = "secret", ExpirationDate = DateTime.UtcNow.AddHours(1) };

            var response = await client.PatronSearchAsync("name = Smith & status: active", page: 3, pageSize: 25, sortBy: PatronSortKeys.PATNL, orgId: 9);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/protected/v1/1033/100/9/token/search/patrons/Boolean");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("name = Smith & status: active", query["q"]);
            Assert.AreEqual("25", query["patronsperpage"]);
            Assert.AreEqual("3", query["page"]);
            Assert.AreEqual("PATNL", query["sort"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public async Task PatronMessages_RequestShape_PreservesBooleanLikeQueryValue()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = await client.PatronMessagesGetAsync("ABC 123", unreadOnly: true, password: "1234");

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/patron/ABC+123/messages");
            var query = ParseQuery(handler.LastRequest.RequestUri.Query);
            Assert.AreEqual("1", query["unreadonly"]);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
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
            var expectedHash = ComputePapiHash("GET", expectedUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", formatted.Headers["Authorization"]);
        }

        private static PapiClient CreateClient(HttpMessageHandler? handler = null)
        {
            var settings = new TestPapiSettings();
            var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
            return new PapiClient(httpClient, settings)
            {
                AllowStaffOverrideRequests = false,
                UseProtectedTokenCache = false
            };
        }


        private static Dictionary<string, string> ParseQuery(string query)
        {
            return query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .ToDictionary(parts => WebUtility.UrlDecode(parts[0]), parts => parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty);
        }

        private static string ComputePapiHash(string httpMethod, string uri, string date, string password, string accessKey)
        {
            var hashString = httpMethod + uri + date + password;
            var computedHash = HMACSHA1.HashData(Encoding.UTF8.GetBytes(accessKey), Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(computedHash);
        }

        private sealed class TestPapiSettings : IPapiSettings
        {
            public string AccessId { get; set; } = "access-id";
            public string AccessKey { get; set; } = "access-key";
            public string Hostname { get; set; } = "https://example.test";
            public int UserId { get; set; } = 1;
            public int WorkstationId { get; set; } = 1;
            public int OrganizationId { get; set; } = 1;
            public PolarisUser? PolarisOverrideAccount { get; set; }
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestContent { get; private set; }
            public CancellationToken LastCancellationToken { get; private set; }

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                LastCancellationToken = cancellationToken;
                LastRequestContent = request.Content == null
                    ? null
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
