using System.IO;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class MultipartGetResultXmlDeserializationTests
    {
        [TestMethod]
        public void MultipartGetResult_DeserializesRows()
        {
            var result = Deserialize<MultipartGetResult>("""
                <MultipartGetResult><PAPIErrorCode>0</PAPIErrorCode><MultipartRows><MultipartRow><Type>1</Type><Value>A</Value><ItemBarcode>1</ItemBarcode></MultipartRow><MultipartRow><Type>2</Type><Value>B</Value><ItemBarcode>2</ItemBarcode></MultipartRow><MultipartRow><Type>3</Type><Value>C</Value><ItemBarcode>3</ItemBarcode></MultipartRow></MultipartRows></MultipartGetResult>
                """);

            Assert.AreEqual(3, result.MultipartRows.Count);
            Assert.AreEqual(2, result.MultipartRows[1].Type);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
