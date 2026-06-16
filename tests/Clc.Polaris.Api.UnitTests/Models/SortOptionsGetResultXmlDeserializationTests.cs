using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class SortOptionsGetResultXmlDeserializationTests
    {
        [TestMethod]
        public void SortOptionsGetResult_DeserializesRows()
        {
            var result = Deserialize<SortOptionsGetResult>("<SortOptionsGetResult><PAPIErrorCode>0</PAPIErrorCode><SortOptionsRows><SortOptionsRow><Code>TI</Code><Description>Title</Description><Options>asc</Options></SortOptionsRow></SortOptionsRows></SortOptionsGetResult>");

            Assert.AreEqual("TI", result.SortOptionsRows[0].Code);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
