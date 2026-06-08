using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Clc.Polaris.Api.Models;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Tests.TestInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Methods.RequestShape
{
    [TestClass]
    [UnitCategory]
    public class NotificationQueueGetTests : PapiClientTestBase
    {

        [TestMethod]
        public async Task NotificationQueueGet_RequestsCorrectUrl()
        {
            var handler = new CaptureHttpMessageHandler();
            var client = CreateUrlEncodingClient(handler);
            client.Token = new ProtectedToken
            {
                AccessToken = "token-segment",
                AccessSecret = "token-secret",
                ExpirationDate = DateTime.Now.AddHours(1)
            };

            await client.NotificationQueueGetAsync(1);

            var expectedPath = $"/PAPIService/REST/protected/v1/1033/24/1/token-segment/notification/";
            Assert.IsNotNull(handler.LastRequest);
            Assert.AreEqual(expectedPath, handler.LastRequest!.RequestUri!.AbsolutePath);
        }
    }
}
