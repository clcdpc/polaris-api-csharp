using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiClientRequestMutationTests
    {
        [TestMethod]
        public void PapiRestRequest_CopyConstructor_CopiesHeadersWithoutSharingCollection()
        {
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.Headers["X-Test"] = "original";

            var copy = new PapiRestRequest(request);

            request.Headers["X-Test"] = "changed";
            request.Headers["X-New"] = "new";

            Assert.AreEqual("original", copy.Headers["X-Test"]);
            Assert.IsFalse(copy.Headers.ContainsKey("X-New"));
        }

        [TestMethod]
        public void PapiRestRequest_CopyConstructor_CopiesQueryParametersWithoutSharingCollection()
        {
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.QueryParameters["first"] = "original";

            var copy = new PapiRestRequest(request);

            request.QueryParameters["first"] = "changed";
            request.QueryParameters["second"] = "new";

            Assert.AreEqual("original", copy.QueryParameters["first"]);
            Assert.IsFalse(copy.QueryParameters.ContainsKey("second"));
        }

        [TestMethod]
        public void PapiRestRequest_CopyConstructor_PreservesPapiSpecificProperties()
        {
            var request = PapiRestRequest.Post("/protected/v1/1033/100/1/test", new { Value = 1 }, "password");
            request.AuthRequired = false;
            request.JsonSerializerIgnoreNulls = false;
            request.BlockStaffOverride = true;
            request.HashString = "hash-string";

            var copy = new PapiRestRequest(request);

            Assert.AreEqual(request.Password, copy.Password);
            Assert.AreEqual(request.AuthRequired, copy.AuthRequired);
            Assert.AreEqual(request.JsonSerializerIgnoreNulls, copy.JsonSerializerIgnoreNulls);
            Assert.AreEqual(request.BlockStaffOverride, copy.BlockStaffOverride);
            Assert.AreEqual(request.HashString, copy.HashString);
        }

        [TestMethod]
        public async Task ExecutePapiAsync_DoesNotMutateCallerRequestPath_WhenReplacingProtectedTokenPlaceholder()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "protected-token",
                AccessSecret = "protected-secret",
                ExpirationDate = DateTime.UtcNow.AddMinutes(5)
            };

            var request = PapiRestRequest.Get($"/protected/v1/1033/100/1/{ProtectedToken.Placeholder}/patron/barcode");
            request.QueryParameters["patronid"] = "12345";

            var originalPath = request.Path;

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(originalPath, request.Path);
            Assert.IsNotNull(handler.LastRequest);
            StringAssert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/protected-token/");
        }

        [TestMethod]
        public async Task ExecutePapiAsync_DoesNotMutateCallerRequestHeaders_WhenSigningRequest()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.Headers["X-Caller"] = "preserve";

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual("preserve", request.Headers["X-Caller"]);
            Assert.IsFalse(request.Headers.ContainsKey("PolarisDate"));
            Assert.IsFalse(request.Headers.ContainsKey("Authorization"));
            Assert.IsFalse(request.Headers.ContainsKey("X-PAPI-AccessToken"));
        }

        [TestMethod]
        public async Task ExecutePapiAsync_DoesNotMutateCallerRequestQueryParameters()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateClient(handler);

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.QueryParameters["first"] = "original";

            await client.ExecutePapiAsync<PapiResponseCommon>(request);

            Assert.AreEqual(1, request.QueryParameters.Count);
            Assert.AreEqual("original", request.QueryParameters["first"]);
        }

        private static PapiClient CreateClient(HttpMessageHandler handler)
        {
            return new PapiClient(new HttpClient(handler), null)
            {
                AccessID = "access-id",
                AccessKey = "access-key",
                Hostname = "https://example.test"
            };
        }

        private sealed class CaptureHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"PAPIErrorCode\":0}")
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                return Task.FromResult(response);
            }
        }
    }
}