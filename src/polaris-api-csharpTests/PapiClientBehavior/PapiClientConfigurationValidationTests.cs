using Clc.Polaris.Api.Configuration;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public sealed class PapiClientConfigurationValidationTests
    {
        [TestMethod]
        [DataRow(nameof(PapiClient.UserId), 0)]
        [DataRow(nameof(PapiClient.UserId), -1)]
        [DataRow(nameof(PapiClient.WorkstationId), 0)]
        [DataRow(nameof(PapiClient.WorkstationId), -1)]
        [DataRow(nameof(PapiClient.OrganizationId), 0)]
        [DataRow(nameof(PapiClient.OrganizationId), -1)]
        public void PositiveIdProperty_SetToInvalidValue_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = new PapiClient();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => SetPositiveIdProperty(client, propertyName, value));
        }

        [TestMethod]
        [DataRow(nameof(PapiClient.UserId))]
        [DataRow(nameof(PapiClient.WorkstationId))]
        [DataRow(nameof(PapiClient.OrganizationId))]
        public void PositiveIdProperty_SetToOne_Succeeds(string propertyName)
        {
            var client = new PapiClient();

            SetPositiveIdProperty(client, propertyName, 1);

            Assert.AreEqual(1, GetPositiveIdProperty(client, propertyName));
        }

        [TestMethod]
        [DataRow(nameof(PapiSettings.UserId))]
        [DataRow(nameof(PapiSettings.WorkstationId))]
        [DataRow(nameof(PapiSettings.OrganizationId))]
        public void Constructor_InvalidPositiveIdSetting_ThrowsArgumentOutOfRangeException(string propertyName)
        {
            var settings = CreateSettings();
            SetPositiveIdSetting(settings, propertyName, 0);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PapiClient(settings));
        }

        [TestMethod]
        [DataRow("", "access-id", "access-key", nameof(PapiClient.Hostname))]
        [DataRow("https://example.org", "", "access-key", nameof(PapiClient.AccessID))]
        [DataRow("https://example.org", "access-id", "", nameof(PapiClient.AccessKey))]
        [DataRow("example.org", "access-id", "access-key", nameof(PapiClient.Hostname))]
        [DataRow("ftp://example.org", "access-id", "access-key", nameof(PapiClient.Hostname))]
        public void PreformatRestRequest_AuthenticatedRequestWithInvalidConfiguration_ThrowsInvalidOperationException(
            string hostname,
            string accessId,
            string accessKey,
            string expectedConfigurationName)
        {
            var client = CreateClient(hostname, accessId, accessKey);
            var request = CreateAuthenticatedRequest();

            var exception = Assert.ThrowsExactly<InvalidOperationException>(() => client.PreformatRestRequest(request));

            Assert.Contains(expectedConfigurationName, exception.Message);
        }

        [TestMethod]
        public void PreformatRestRequest_AuthenticatedRequestWithValidConfiguration_AddsAuthorizationHeaders()
        {
            var client = CreateClient(
                hostname: "https://example.org",
                accessId: "access-id",
                accessKey: "access-key");

            var request = CreateAuthenticatedRequest();

            var formattedRequest = client.PreformatRestRequest(request);

            Assert.IsTrue(formattedRequest.Headers.ContainsKey("PolarisDate"));
            Assert.IsTrue(formattedRequest.Headers.ContainsKey("Authorization"));
            Assert.StartsWith("PWS access-id:", formattedRequest.Headers["Authorization"]);
        }

        private static PapiClient CreateClient(string hostname, string accessId, string accessKey)
        {
            return new PapiClient
            {
                AccessID = accessId,
                AccessKey = accessKey,
                Hostname = hostname
            };
        }

        private static PapiRestRequest CreateAuthenticatedRequest()
        {
            var request = PapiRestRequest.Get("/public/v1/1033/100/1/test");
            request.AuthRequired = true;

            return request;
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

        private static void SetPositiveIdProperty(PapiClient client, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(PapiClient.UserId):
                    client.UserId = value;
                    break;

                case nameof(PapiClient.WorkstationId):
                    client.WorkstationId = value;
                    break;

                case nameof(PapiClient.OrganizationId):
                    client.OrganizationId = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PapiClient property.");
            }
        }

        private static int GetPositiveIdProperty(PapiClient client, string propertyName)
        {
            return propertyName switch
            {
                nameof(PapiClient.UserId) => client.UserId,
                nameof(PapiClient.WorkstationId) => client.WorkstationId,
                nameof(PapiClient.OrganizationId) => client.OrganizationId,
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PapiClient property.")
            };
        }

        private static void SetPositiveIdSetting(PapiSettings settings, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(PapiSettings.UserId):
                    settings.UserId = value;
                    break;

                case nameof(PapiSettings.WorkstationId):
                    settings.WorkstationId = value;
                    break;

                case nameof(PapiSettings.OrganizationId):
                    settings.OrganizationId = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported PapiSettings property.");
            }
        }
    }
}
