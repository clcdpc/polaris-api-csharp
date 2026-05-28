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
        public void PapiRestRequest_StringConstructor_DefaultsToGetAndSetsPath()
        {
            var request = new PapiRestRequest("/public/foo");

            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.IsTrue(request.IsPublicMethod);
            Assert.IsFalse(request.IsProtectedMethod);
        }

        [TestMethod]
        public void PapiRestRequest_MethodConstructor_PreservesValues()
        {
            var body = new { Test = "value" };
            var request = new PapiRestRequest(HttpMethod.Put, "/public/foo", "pin", body);

            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/public/foo", request.Path);
            Assert.AreEqual("pin", request.Password);
            Assert.AreSame(body, request.Body);
        }

        [TestMethod]
        public void PapiRestRequest_RestRequestConstructor_PreservesCoreRequestData()
        {
            var original = new RestRequest
            {
                Method = HttpMethod.Post,
                Path = "/protected/foo",
                Body = new { Value = 123 },
                Parameters = new Dictionary<string, object> { ["q"] = "x" },
                Headers = new Dictionary<string, string> { ["X-Test"] = "abc" }
            };

            var wrapped = new PapiRestRequest(original);

            Assert.AreEqual(original.Method, wrapped.Method);
            Assert.AreEqual(original.Path, wrapped.Path);
            Assert.AreSame(original.Body, wrapped.Body);
            CollectionAssert.AreEquivalent(original.Parameters.ToList(), wrapped.Parameters.ToList());
            CollectionAssert.AreEquivalent(original.Headers.ToList(), wrapped.Headers.ToList());
            Assert.IsTrue(wrapped.IsProtectedMethod);
            Assert.IsFalse(wrapped.IsPublicMethod);
        }

        [TestMethod]
        public void PreformatRestRequest_AddsPolarisDateAndAuthorization_WhenAuthRequired()
        {
            var client = BuildClient();
            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate");

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual(HttpMethod.Get, formatted.Method);
            Assert.AreEqual("/public/v1/1033/100/1/apikeyvalidate", formatted.Path);
            Assert.IsTrue(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
            StringAssert.StartsWith(formatted.Headers["Authorization"], "PWS access-id:");
        }

        [TestMethod]
        public void PreformatRestRequest_SkipsAuthHeaders_WhenAuthNotRequired()
        {
            var client = BuildClient();
            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate") { AuthRequired = false };

            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_AddsStaffOverrideToken_WhenAllowedAndNotBlocked()
        {
            var client = BuildClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "access-token",
                AccessSecret = "access-secret",
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };

            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate");
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.AreEqual("access-token", formatted.Headers["X-PAPI-AccessToken"]);
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void PreformatRestRequest_DoesNotAddStaffOverrideToken_WhenBlocked()
        {
            var client = BuildClient();
            client.AllowStaffOverrideRequests = true;
            client.Token = new ProtectedToken
            {
                AccessToken = "access-token",
                AccessSecret = "access-secret",
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)
            };

            var request = new PapiRestRequest("/public/v1/1033/100/1/apikeyvalidate") { BlockStaffOverride = true };
            var formatted = (PapiRestRequest)client.PreformatRestRequest(request);

            Assert.IsFalse(formatted.Headers.ContainsKey("X-PAPI-AccessToken"));
            Assert.IsTrue(formatted.Headers.ContainsKey("Authorization"));
        }

        [TestMethod]
        public void RequestShape_ApiKeyValidate_UsesGetPublicUrlAndNoBody()
        {
            var handler = new CapturingHttpMessageHandler(_ => JsonResponse("{\"PAPIErrorCode\":0}"));
            var client = BuildClient(handler);

            var response = client.ApiKeyValidate();

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            AssertCapturedRequest(handler, HttpMethod.Get, "/public/v1/1033/100/1/apikeyvalidate", expectBody: false);
        }

        [TestMethod]
        public void RequestShape_BibSearch_UsesEncodedQueryAndNoBody()
        {
            var handler = new CapturingHttpMessageHandler(_ => JsonResponse("{\"PAPIErrorCode\":0}"));
            var client = BuildClient(handler);

            var response = client.BibSearch(new BibSearchOptions
            {
                Term = "dogs & cats",
                Branch = 7,
                Page = 2,
                PageSize = 25,
                SortOption = SearchSortOptions.AU,
                Limit = "french"
            });

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            AssertCapturedRequest(handler, HttpMethod.Get, "/public/v1/1033/100/7/search/bibs/keyword/kw", expectBody: false);
            var uri = handler.Request!.RequestUri!.ToString();
            StringAssert.Contains(uri, "q=dogs+%26+cats");
            StringAssert.Contains(uri, "sort=AU");
            StringAssert.Contains(uri, "page=2");
            StringAssert.Contains(uri, "bibsperpage=25");
            StringAssert.Contains(uri, "limit=french");
        }

        [TestMethod]
        public void RequestShape_AuthenticateStaffUser_UsesPostAndBody()
        {
            var handler = new CapturingHttpMessageHandler(_ => JsonResponse("{\"PAPIErrorCode\":0,\"AccessToken\":\"token\",\"AccessSecret\":\"secret\"}"));
            var client = BuildClient(handler);

            var response = client.AuthenticateStaffUser(new PolarisUser("domain", "user", "pass"));

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            AssertCapturedRequest(handler, HttpMethod.Post, "/protected/v1/1033/100/1/authenticator/staff", expectBody: true);
        }

        [TestMethod]
        public void RequestShape_PatronUpdate_UsesPutBarcodeIgnoreSaAndBody()
        {
            var handler = new CapturingHttpMessageHandler(_ => JsonResponse("{\"PAPIErrorCode\":0}"));
            var client = BuildClient(handler);

            var response = client.PatronUpdate("29001 0001", new PatronUpdateParams { EmailAddress = "a@b.com" }, ignoresa: false);

            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            AssertCapturedRequest(handler, HttpMethod.Put, "/public/v1/1033/100/1/patron/29001+0001", expectBody: true);
            StringAssert.Contains(handler.Request!.RequestUri!.Query, "ignoresa=False");
        }

        private static void AssertCapturedRequest(CapturingHttpMessageHandler handler, HttpMethod expectedMethod, string expectedPathFragment, bool expectBody)
        {
            Assert.IsNotNull(handler.Request);
            Assert.AreEqual(expectedMethod, handler.Request!.Method);
            StringAssert.Contains(handler.Request.RequestUri!.AbsoluteUri, expectedPathFragment);

            var hasBody = handler.Request.Content != null;
            Assert.AreEqual(expectBody, hasBody);

            Assert.IsTrue(handler.Request.Headers.Contains("Authorization"));
            Assert.IsTrue(handler.Request.Headers.Contains("PolarisDate"));
        }

        private static HttpResponseMessage JsonResponse(string json) => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        private static PapiClient BuildClient(HttpMessageHandler? handler = null)
        {
            var httpClient = handler == null ? new HttpClient() : new HttpClient(handler);
            return new PapiClient(httpClient, new PapiSettings
            {
                Hostname = "https://example.test",
                AccessId = "access-id",
                AccessKey = "access-key",
                OrganizationId = 1,
                UserId = 1,
                WorkstationId = 1
            });
        }

        private sealed class CapturingHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

            public HttpRequestMessage? Request { get; private set; }

            public CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                return Task.FromResult(_responseFactory(request));
            }
        }
    }
}
