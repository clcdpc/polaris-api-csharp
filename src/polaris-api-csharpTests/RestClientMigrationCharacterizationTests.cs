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
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
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
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        }


        [TestMethod]
        public void PreformatRestRequest_HashesPublicAuthenticatedGetWithoutQueryParameters_AsLegacyUri()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate",
                string.Empty);
            Assert.AreEqual(0, formatted.QueryParameters.Count);
        }

        [TestMethod]
        public void PreformatRestRequest_HashesPublicAuthenticatedGetWithQueryParameters_UsingRestClientEscaping()
        {
            var client = CreateClient();
            var request = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            request.QueryParameters.Add("q", "harry potter & stone");
            request.QueryParameters.Add("sort", "MP");
            request.QueryParameters.Add("empty", null!);

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=harry%20potter%20%26%20stone&sort=MP&empty=",
                string.Empty);
        }

        [TestMethod]
        public void PreformatRestRequest_HashesBodyRequestsWithQueryParameters_WithoutChangingBodies()
        {
            var client = CreateClient();
            var putBody = new PatronUpdateParams { EmailAddress = "patron@example.test" };
            var put = new PapiRestRequest(HttpMethod.Put, "/public/v1/1033/100/1/patron/ABC", "pin", putBody);
            put.QueryParameters.Add("ignoresa", true);
            var postBody = new PolarisUser { Domain = "main", Username = "staff", Password = "secret" };
            var post = new PapiRestRequest(HttpMethod.Post, "/protected/v1/1033/100/1/authenticator/staff", body: postBody);
            post.QueryParameters.Add("wsid", 3);

            var formattedPut = (PapiRestRequest)client.PreformatRestRequest(put);
            var formattedPost = (PapiRestRequest)client.PreformatRestRequest(post);

            Assert.AreSame(putBody, formattedPut.Body);
            AssertPapiAuthorization(
                formattedPut,
                "PUT",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/patron/ABC?ignoresa=True",
                "pin");
            Assert.AreSame(postBody, formattedPost.Body);
            AssertPapiAuthorization(
                formattedPost,
                "POST",
                "https://example.test/PAPIService/REST/protected/v1/1033/100/1/authenticator/staff?wsid=3",
                string.Empty);
        }

        [TestMethod]
        public void PreformatRestRequest_ProtectedRequest_UsesProtectedTokenSecret()
        {
            var client = CreateClient();
            client.Token = new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };
            var request = new PapiRestRequest(HttpMethod.Get, "/protected/v1/1033/100/1/protected-token/search/patrons/Boolean");
            request.QueryParameters.Add("q", "name = Smith");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/protected/v1/1033/100/1/protected-token/search/patrons/Boolean?q=name%20%3D%20Smith",
                "protected-secret");
        }

        [TestMethod]
        public void PreformatRestRequest_StaffOverrideAllowedAndUnblocked_AddsTokenHeaderAndUsesTokenSecret()
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

            Assert.AreEqual("staff-token", formatted.Headers["X-PAPI-AccessToken"]);
            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate",
                "staff-secret");
        }

        [TestMethod]
        public void PreformatRestRequest_StaffOverrideBlocked_DoesNotAddTokenHeaderOrUseTokenSecret()
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
            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate",
                string.Empty);
        }

        [TestMethod]
        public void PreformatRestRequest_StaffOverrideDisabled_DoesNotAddTokenHeaderOrUseTokenSecret()
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
            AssertPapiAuthorization(
                formatted,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate",
                string.Empty);
        }

        [TestMethod]
        public void PreformatRestRequest_CanBeCalledTwice_WithoutDuplicatingHeadersOrChangingBody()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddHours(1)
            };
            var body = new { Value = "unchanged" };
            var request = new PapiRestRequest(HttpMethod.Post, "/public/v1/1033/100/1/foo", body: body);
            request.Headers.Add("X-Test", "original");

            var first = (PapiRestRequest)client.PreformatRestRequest(request);
            var second = (PapiRestRequest)client.PreformatRestRequest(first);

            Assert.AreSame(first, second);
            Assert.AreSame(body, second.Body);
            Assert.AreEqual("original", second.Headers["X-Test"]);
            Assert.AreEqual("staff-token", second.Headers["X-PAPI-AccessToken"]);
            Assert.AreEqual(4, second.Headers.Count);
            AssertPapiAuthorization(
                second,
                "POST",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/foo",
                "staff-secret");
        }

        [TestMethod]
        public void PreformatRestRequest_AuthorizationChanges_WhenQueryParameterValuesChange()
        {
            var client = CreateClient();
            var first = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            first.QueryParameters.Add("q", "alpha");
            var second = new PapiRestRequest(HttpMethod.Get, "/public/v1/1033/100/1/search/bibs/keyword/KW");
            second.QueryParameters.Add("q", "beta");

            var formattedFirst = (PapiRestRequest)client.PreformatRestRequest(first);
            var formattedSecond = (PapiRestRequest)client.PreformatRestRequest(second);

            AssertPapiAuthorization(
                formattedFirst,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=alpha",
                string.Empty);
            AssertPapiAuthorization(
                formattedSecond,
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/search/bibs/keyword/KW?q=beta",
                string.Empty);
            Assert.AreNotEqual(formattedFirst.Headers["Authorization"], formattedSecond.Headers["Authorization"]);
        }

        [TestMethod]
        public void ApiKeyValidate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = client.ApiKeyValidate();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/apikeyvalidate");
            Assert.AreEqual(string.Empty, handler.LastRequest.RequestUri.Query);
            Assert.IsNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
        }

        [TestMethod]
        public void BibSearch_RequestShape_IsStable()
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

            var response = client.BibSearch(options);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpMethod.Get, handler.LastRequest!.Method);
            StringAssert.Contains(handler.LastRequest.RequestUri!.AbsolutePath, "/public/v1/1033/100/1/search/bibs/keyword/KW");
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
        public void AuthenticateStaffUser_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0,\"AccessToken\":\"t\",\"AccessSecret\":\"s\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}");
            var client = CreateClient(handler);

            var response = client.AuthenticateStaffUser(new PolarisUser
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
        public void PatronUpdate_RequestShape_IsStable()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = client.PatronUpdate("AB C/+#?=", new PatronUpdateParams { EmailAddress = "patron@example.test" }, "1234", ignoresa: true);

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
        public void PatronSearch_RequestShape_PreservesEncodedQuerySemantics()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);
            client.Token = new ProtectedToken { AccessToken = "token", AccessSecret = "secret", ExpirationDate = DateTime.UtcNow.AddHours(1) };

            var response = client.PatronSearch("name = Smith & status: active", page: 3, pageSize: 25, sortBy: PatronSortKeys.PATNL, orgId: 9);

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
        public void PatronMessages_RequestShape_PreservesBooleanLikeQueryValue()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0}");
            var client = CreateClient(handler);

            var response = client.PatronMessagesGet("ABC 123", unreadOnly: true, password: "1234");

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
        public void ExecutedRequest_AuthorizationHashesTheSameQueryParameterUriSentByRestClient()
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

            var response = client.BibSearch(options);

            Assert.IsNotNull(response);
            Assert.IsNotNull(handler.LastRequest);
            var date = handler.LastRequest.Headers.GetValues("PolarisDate").Single();
            var expectedHash = ComputePapiHash("GET", handler.LastRequest.RequestUri!.AbsoluteUri, date, string.Empty, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", handler.LastRequest.Headers.GetValues("Authorization").Single());
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



        private static void AssertPapiAuthorization(PapiRestRequest request, string httpMethod, string expectedUri, string password)
        {
            Assert.IsTrue(request.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(request.Headers.ContainsKey("Authorization"));
            var date = request.Headers["PolarisDate"];
            var expectedHash = ComputePapiHash(httpMethod, expectedUri, date, password, "access-key");
            Assert.AreEqual($"PWS access-id:{expectedHash}", request.Headers["Authorization"]);
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

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
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
