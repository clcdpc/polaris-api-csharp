using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
    [TestCategory("Unit")]
    [TestCategory("RestClientMigration")]
    public class RestClientMigrationCharacterizationTests
    {
        [TestMethod]
        public void PapiRestRequest_StringConstructor_DefaultsToGetAndPath()
        {
            var request = new PapiRestRequest("/public/foo");

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
        }

        [TestMethod]
        public void PapiRestRequest_MethodConstructor_PreservesAllValues()
        {
            var body = new { Name = "test" };
            var request = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);

            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void PapiRestRequest_RestRequestConstructor_PreservesMethodPathBodyParametersAndHeaders()
        {
            var original = new RestRequest
            {
                Method = HttpMethod.Post,
                Path = "/protected/foo",
                Body = new { Value = 123 },
                Parameters = new Dictionary<string, string> { { "a", "1" } },
                Headers = new Dictionary<string, string> { { "h", "v" } }
            };

            var wrapped = new PapiRestRequest(original);

            Assert.AreEqual(original.Method, wrapped.Method);
            Assert.AreEqual(original.Path, wrapped.Path);
            Assert.AreSame(original.Body, wrapped.Body);
            Assert.AreSame(original.Parameters, wrapped.Parameters);
            Assert.AreSame(original.Headers, wrapped.Headers);
        }

        [TestMethod]
        public void PapiRestRequest_IsPublicAndIsProtected_BasedOnPathPrefix()
        {
            var publicRequest = new PapiRestRequest("/public/v1/foo");
            var protectedRequest = new PapiRestRequest("/protected/v1/foo");

            Assert.IsTrue(publicRequest.IsPublicMethod);
            Assert.IsFalse(publicRequest.IsProtectedMethod);
            Assert.IsTrue(protectedRequest.IsProtectedMethod);
            Assert.IsFalse(protectedRequest.IsPublicMethod);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsRequiredAuthHeaders_AndKeepsMethodAndPath()
        {
            var client = CreateClient();
            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual(HttpMethod.Get, formatted.Method);
            Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
            Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");
        }

        [TestMethod]
        public void PreformatRestRequest_WhenAuthNotRequired_DoesNotAddPapiAuthHeaders()
        {
            var client = CreateClient();
            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate") { AuthRequired = false };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_PublicMethod_AddsStaffOverrideToken_WhenAllowedAndNotBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };

            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsTrue(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.AreEqual("staff-token", formatted.Headers["X-PAPI-AccessToken"]);
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_PublicMethod_DoesNotAddStaffOverrideToken_WhenBlocked()
        {
            var client = CreateClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "staff-token",
                AccessSecret = "staff-secret",
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };

            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate") { BlockStaffOverride = true };
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void ApiKeyValidate_RequestShape_IsGetPublicWithoutBody_WithAuthHeaders()
        {
            var handler = new CapturingHttpMessageHandler(ResponseJson("PAPIErrorCode", "0"));
            var client = CreateClient(handler);

            client.ApiKeyValidate();

            AssertRequest(handler, HttpMethod.Get, "/PAPIService/REST/public/v1/1033/100/1/apikeyvalidate", expectBody: false);
        }

        [TestMethod]
        public void BibSearch_RequestShape_IsGetWithExpectedEncodedQuery_WithoutBody()
        {
            var handler = new CapturingHttpMessageHandler(ResponseJson("PAPIErrorCode", "0"));
            var client = CreateClient(handler);

            client.BibSearch(new BibSearchOptions
            {
                Branch = 7,
                Term = "dogs & cats",
                SortOption = SearchSortOptions.MP,
                Page = 3,
                PageSize = 25,
                Limit = "A B"
            });

            AssertRequest(handler, HttpMethod.Get, "/PAPIService/REST/public/v1/1033/100/7/search/bibs/keyword/KW", expectBody: false);
            var query = handler.LastRequest!.RequestUri!.Query;
            StringAssert.Contains(query, "q=dogs+%26+cats");
            StringAssert.Contains(query, "sort=MP");
            StringAssert.Contains(query, "page=3");
            StringAssert.Contains(query, "bibsperpage=25");
            StringAssert.Contains(query, "limit=A+B");
        }

        [TestMethod]
        public void AuthenticateStaffUser_RequestShape_IsPostProtectedWithBody()
        {
            var handler = new CapturingHttpMessageHandler("{\"PAPIErrorCode\":0,\"AccessToken\":\"t\",\"AccessSecret\":\"s\"}");
            var client = CreateClient(handler);

            client.AuthenticateStaffUser(new PolarisUser("domain", "username", "password"));

            AssertRequest(handler, HttpMethod.Post, "/PAPIService/REST/protected/v1/1033/100/1/authenticator/staff", expectBody: true);
            StringAssert.Contains(handler.LastRequestContent!, "domain");
            StringAssert.Contains(handler.LastRequestContent!, "username");
        }

        [TestMethod]
        public void PatronUpdate_RequestShape_IsPutWithBarcodePathQueryAndBody()
        {
            var handler = new CapturingHttpMessageHandler(ResponseJson("PAPIErrorCode", "0"));
            var client = CreateClient(handler);

            client.PatronUpdate("AB C/+#?=", new PatronUpdateParams { EmailAddress = "user@example.test" }, ignoresa: false);

            AssertRequest(handler, HttpMethod.Put, "/PAPIService/REST/public/v1/1033/100/1/patron/AB+C%2f%2b%23%3f%3d", expectBody: true);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.Query, "ignoresa=False");
            StringAssert.Contains(handler.LastRequestContent!, "user@example.test");
        }

        private static void AssertRequest(CapturingHttpMessageHandler handler, HttpMethod expectedMethod, string expectedPath, bool expectBody)
        {
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedMethod, handler.LastRequest!.Method);
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));

            if (expectBody)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(handler.LastRequestContent));
            }
            else
            {
                Assert.IsTrue(string.IsNullOrWhiteSpace(handler.LastRequestContent));
            }
        }

        private static PapiClient CreateClient(HttpMessageHandler? handler = null)
        {
            var settings = new PapiSettings
            {
                Hostname = "https://example.test",
                AccessId = "access-id",
                AccessKey = "access-key",
                OrganizationId = 1,
                UserId = 1,
                WorkstationId = 1
            };

            return new PapiClient(handler == null ? new HttpClient() : new HttpClient(handler), settings);
        }

        private static string ResponseJson(string key, string value) => $"{{\"{key}\":{value}}}";

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
                LastRequestContent = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return response;
            }
        }
    }
}
