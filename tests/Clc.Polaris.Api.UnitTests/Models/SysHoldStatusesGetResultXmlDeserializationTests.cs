using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class SysHoldStatusesGetResultXmlDeserializationTests
    {
        [TestMethod]
        public void SysHoldStatusesGetResult_DeserializesRows()
        {
            var result = Deserialize<SysHoldStatusesGetResult>("<SysHoldStatusesGetResult><PAPIErrorCode>0</PAPIErrorCode><SysHoldStatusesRows><SysHoldStatusesRow><SysHoldStatusID>1</SysHoldStatusID><Description>Active</Description><Name>Active</Name></SysHoldStatusesRow></SysHoldStatusesRows></SysHoldStatusesGetResult>");

            Assert.AreEqual(1, result.SysHoldStatusesRows[0].SysHoldStatusID);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
