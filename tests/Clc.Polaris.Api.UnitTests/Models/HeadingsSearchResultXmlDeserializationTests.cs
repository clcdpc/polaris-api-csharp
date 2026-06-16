using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class HeadingsSearchResultXmlDeserializationTests
    {
        [TestMethod]
        public void HeadingsSearchResult_DeserializesHexHeadingId()
        {
            var result = Deserialize<HeadingsSearchResult>("""
                <HeadingsSearchResult><PAPIErrorCode>0</PAPIErrorCode><HeadingsSearchRows><HeadingsSearchRow><Position>1</Position><DisplayType>2</DisplayType><DisplayConstant>Author</DisplayConstant><DisplayTerm>Twain</DisplayTerm><GlobalOccurrences>3</GlobalOccurrences><HeadingID>0xABCDEF</HeadingID></HeadingsSearchRow></HeadingsSearchRows></HeadingsSearchResult>
                """);

            Assert.AreEqual(2, result.HeadingsSearchRows[0].DisplayType);
            Assert.AreEqual("0xABCDEF", result.HeadingsSearchRows[0].HeadingID);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
