using System;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiClientConfigurationValidationTests
    {
        [TestMethod]
        public void UserId_SetToZero_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.UserId = 0);
        }

        [TestMethod]
        public void UserId_SetToNegative_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.UserId = -1);
        }

        [TestMethod]
        public void UserId_SetToOne_Succeeds()
        {
            var client = new PapiClient
            {
                UserId = 1
            };

            Assert.AreEqual(1, client.UserId);
        }

        [TestMethod]
        public void WorkstationId_SetToZero_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.WorkstationId = 0);
        }

        [TestMethod]
        public void WorkstationId_SetToNegative_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.WorkstationId = -1);
        }

        [TestMethod]
        public void WorkstationId_SetToOne_Succeeds()
        {
            var client = new PapiClient
            {
                WorkstationId = 1
            };

            Assert.AreEqual(1, client.WorkstationId);
        }

        [TestMethod]
        public void OrganizationId_SetToZero_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.OrganizationId = 0);
        }

        [TestMethod]
        public void OrganizationId_SetToNegative_ThrowsArgumentOutOfRangeException()
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => client.OrganizationId = -1);
        }

        [TestMethod]
        public void OrganizationId_SetToOne_Succeeds()
        {
            var client = new PapiClient
            {
                OrganizationId = 1
            };

            Assert.AreEqual(1, client.OrganizationId);
        }

        [TestMethod]
        public void Constructor_InvalidUserIdSetting_ThrowsArgumentOutOfRangeException()
        {
            var settings = CreateSettings();
            settings.UserId = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PapiClient(settings));
        }

        [TestMethod]
        public void Constructor_InvalidWorkstationIdSetting_ThrowsArgumentOutOfRangeException()
        {
            var settings = CreateSettings();
            settings.WorkstationId = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PapiClient(settings));
        }

        [TestMethod]
        public void Constructor_InvalidOrganizationIdSetting_ThrowsArgumentOutOfRangeException()
        {
            var settings = CreateSettings();
            settings.OrganizationId = 0;

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PapiClient(settings));
        }

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

            Assert.Contains(nameof(PapiClient.Hostname), exception.Message);
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

            Assert.Contains(nameof(PapiClient.AccessID), exception.Message);
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

            Assert.Contains(nameof(PapiClient.AccessKey), exception.Message);
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

            Assert.Contains(nameof(PapiClient.Hostname), exception.Message);
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

            Assert.Contains(nameof(PapiClient.Hostname), exception.Message);
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
            Assert.StartsWith("PWS access-id:", formattedRequest.Headers["Authorization"]);
        }

        private static PapiSettings CreateSettings()
        {
            return new PapiSettings
            {
                AccessId = "access-id",
                AccessKey = "access-key",
                Hostname = "https://example.org",
                OrganizationId = 1,
                UserId = 1,
                WorkstationId = 1
            };
        }
    }
}
