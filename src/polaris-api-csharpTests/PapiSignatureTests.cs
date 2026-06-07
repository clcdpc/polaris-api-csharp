using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiSignatureTests
    {
        [TestMethod]
        public void ComputeHash_ReturnsExpectedPapiSignature_ForKnownVector()
        {
            var hash = PapiSignature.ComputeHash(
                "access-key",
                "GET",
                "https://example.test/PAPIService/REST/public/v1/1033/100/1/custom",
                "Mon, 01 Jan 2024 00:00:00 GMT",
                string.Empty);

            Assert.AreEqual("6s8zdodfxSEnTlU6LUU+P8nSb9s=", hash);
        }
    }
}
