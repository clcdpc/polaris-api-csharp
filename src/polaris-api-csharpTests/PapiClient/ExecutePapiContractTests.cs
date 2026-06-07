using Clc.Polaris.Api;
using Clc.Polaris.Api.Tests;
using Clc.Polaris.Api.Models;
using Clc.Rest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Clc.Polaris.Api.Tests.Client
{
    [TestClass]
    [UnitCategory]
    public class ExecutePapiContractTests : PapiClientTestBase
    {
        [TestMethod]
        public void ExecutePapiAsync_IsPublicOnPapiClientOnly()
        {
            var papiClientMethod = typeof(PapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClient.ExecutePapiAsync) && method.IsGenericMethodDefinition);
            var interfaceMethod = typeof(IPapiClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(method => method.Name == nameof(PapiClient.ExecutePapiAsync));

            Assert.IsNotNull(papiClientMethod);
            Assert.IsNull(interfaceMethod);
        }
    }
}
