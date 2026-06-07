using Clc.Polaris.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Clc.Polaris.Api.Tests
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
