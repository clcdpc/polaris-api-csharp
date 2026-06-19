using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Xml.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class SAMobilePhoneCarriersGetResultTests
    {
        [TestMethod]
        public void SAMobilePhoneCarriersGetResult_DeserializesDisplayValues()
        {
            var result = Deserialize<SAMobilePhoneCarriersGetResult>("""
                <SAMobilePhoneCarriersGetResult><PAPIErrorCode>0</PAPIErrorCode><SAMobilePhoneCarriersGetRows><SAMobilePhoneCarriersGetRow><CarrierID>1</CarrierID><CarrierName>A</CarrierName><Email2SMSEmailAddress>a.example</Email2SMSEmailAddress><NumberOfDigits>10</NumberOfDigits><Display>true</Display></SAMobilePhoneCarriersGetRow><SAMobilePhoneCarriersGetRow><CarrierID>2</CarrierID><CarrierName>B</CarrierName><Email2SMSEmailAddress>b.example</Email2SMSEmailAddress><NumberOfDigits>7</NumberOfDigits><Display>false</Display></SAMobilePhoneCarriersGetRow></SAMobilePhoneCarriersGetRows></SAMobilePhoneCarriersGetResult>
                """);

            Assert.AreEqual(true, result.SAMobilePhoneCarriersGetRows[0].Display);
            Assert.AreEqual(false, result.SAMobilePhoneCarriersGetRows[1].Display);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
