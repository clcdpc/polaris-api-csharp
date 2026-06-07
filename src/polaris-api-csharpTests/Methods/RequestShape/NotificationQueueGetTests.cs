using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
    [UnitCategory]
    public class NotificationQueueGetTests : RequestShapeTestBase
    {
                [TestMethod]
                public async Task NotificationQueueGet_RequestsCorrectUrl()
                {
                    var handler = new CapturingHttpMessageHandler();
                    var client = CreateRequestShapeClient(handler);
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
