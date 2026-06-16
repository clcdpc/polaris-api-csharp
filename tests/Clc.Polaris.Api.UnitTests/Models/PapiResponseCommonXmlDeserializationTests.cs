using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class PapiResponseCommonXmlDeserializationTests
    {
        [TestMethod]
        public void PapiResponseCommon_DeserializesRequestsUpdateStatusSample()
        {
            var result = Deserialize<PapiResponseCommon>("<PapiResponseCommon><PAPIErrorCode>0</PAPIErrorCode><ErrorMessage /></PapiResponseCommon>");

            Assert.AreEqual(0, result.PAPIErrorCode);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
