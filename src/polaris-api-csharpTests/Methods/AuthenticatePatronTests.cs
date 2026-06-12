using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitTest]
    public class AuthenticatePatronTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task AuthenticatePatron_SendsPostRequestWithJsonBody()
        {
            var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0, AccessToken = "mock-token", AccessSecret = "mock-secret", PatronID = 123 }));

            var client = CreateClient(handler);
            var barcode = "21945001234567";
            var password = "mypassword";

            var response = await client.AuthenticatePatronAsync(barcode, password, TestContext.CancellationToken);

            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(HttpMethod.Post, handler.LastRequest.Method);
            var expectedPath = "/PAPIService/REST/public/v1/1033/100/1/authenticator/patron";
            Assert.AreEqual(expectedPath, handler.LastRequest.RequestUri!.AbsolutePath);

            Assert.IsNotNull(handler.LastRequestContent);
            Assert.IsTrue(handler.LastRequestContent.Contains($"\"Barcode\":\"{barcode}\"") || handler.LastRequestContent.Contains($"\"barcode\":\"{barcode}\""), "Body should contain barcode");
            Assert.IsTrue(handler.LastRequestContent.Contains($"\"Password\":\"{password}\"") || handler.LastRequestContent.Contains($"\"password\":\"{password}\""), "Body should contain password");

            Assert.IsNotNull(response.Data);
            Assert.AreEqual(0, response.Data.PAPIErrorCode);
            Assert.AreEqual("mock-token", response.Data.AccessToken);
            Assert.AreEqual("mock-secret", response.Data.AccessSecret);
            Assert.AreEqual(123, response.Data.PatronID);
        }
    }
}
