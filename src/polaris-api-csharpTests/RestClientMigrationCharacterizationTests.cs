using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            existing.Parameters.Add("limit", "5");
            existing.Headers.Add("X-Test", "header");

            var copied = new PapiRestRequest(existing);
            Assert.AreEqual(existing.Method, copied.Method);
            Assert.AreEqual(existing.Path, copied.Path);
            Assert.AreSame(existing.Body, copied.Body);
            Assert.AreSame(existing.Parameters, copied.Parameters);
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
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "q=harry+potter+%26+stone");
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "sort=MP");
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "page=2");
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "bibsperpage=15");
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "limit=3");
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
            StringAssert.Contains(handler.LastRequest.RequestUri.Query, "ignoresa=True");
            Assert.IsNotNull(handler.LastRequest.Content);
            Assert.IsTrue(handler.LastRequest.Headers.Contains("PolarisDate"));
            Assert.IsTrue(handler.LastRequest.Headers.Contains("Authorization"));
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

            public CapturingHttpMessageHandler(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
