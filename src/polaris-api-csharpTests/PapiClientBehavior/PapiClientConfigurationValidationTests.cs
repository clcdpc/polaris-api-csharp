using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiClientConfigurationValidationTests
    {
        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithoutHostname_ThrowsInvalidOperationException()
        {
            var client = new PapiClient
            {
                AccessID = "access-id",
                AccessKey = "access-key",
                Hostname = ""
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            StringAssert.Contains(exception.Message, nameof(PapiClient.Hostname));
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithoutAccessID_ThrowsInvalidOperationException()
        {
            var client = new PapiClient
            {
                AccessID = "",
                AccessKey = "access-key",
                Hostname = "https://example.org"
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            StringAssert.Contains(exception.Message, nameof(PapiClient.AccessID));
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithoutAccessKey_ThrowsInvalidOperationException()
        {
            var client = new PapiClient
            {
                AccessID = "access-id",
                AccessKey = "",
                Hostname = "https://example.org"
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            StringAssert.Contains(exception.Message, nameof(PapiClient.AccessKey));
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithRelativeHostname_ThrowsInvalidOperationException()
        {
            var client = new PapiClient
            {
                AccessID = "access-id",
                AccessKey = "access-key",
                Hostname = "example.org"
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            StringAssert.Contains(exception.Message, nameof(PapiClient.Hostname));
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithUnsupportedHostnameScheme_ThrowsInvalidOperationException()
        {
            var client = new PapiClient
            {
                AccessID = "access-id",
                AccessKey = "access-key",
                Hostname = "ftp://example.org"
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            StringAssert.Contains(exception.Message, nameof(PapiClient.Hostname));
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithValidConfiguration_AddsAuthorizationHeaders()
        {
            var client = new PapiClient
            {
                AccessID = "access-id",
                AccessKey = "access-key",
                Hostname = "https://example.org"
            };

            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            var formattedRequest = client.PreformatRestRequest(request);

            Assert.IsTrue(formattedRequest.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formattedRequest.Headers.ContainsKey("Authorization"));
            StringAssert.StartsWith(formattedRequest.Headers["Authorization"], "PWS access-id:");
        }
    }
}
