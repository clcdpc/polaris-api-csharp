using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class BibGetByTypeResultXmlDeserializationTests
    {
        [TestMethod]
        public void BibGetByTypeResult_DeserializesRows()
        {
            var result = Deserialize<BibGetByTypeResult>("""
                <BibGetByTypeResult><PAPIErrorCode>0</PAPIErrorCode><BibGetByTypeRows><BibGetByTypeRow><ElementID>35</ElementID><Occurence>1</Occurence><Label>Title</Label><Value>Example</Value><Alternate>false</Alternate></BibGetByTypeRow></BibGetByTypeRows></BibGetByTypeResult>
                """);

            Assert.AreEqual(0, result.PAPIErrorCode);
            Assert.AreEqual(35, result.BibGetByTypeRows[0].ElementID);
            Assert.AreEqual(1, result.BibGetByTypeRows[0].Occurrence);
            Assert.AreEqual(false, result.BibGetByTypeRows[0].Alternate);
        }

        [TestMethod]
        public void BibGetByTypeResult_DeserializesFailureWithNilRows()
        {
            var result = Deserialize<BibGetByTypeResult>("""
                <BibGetByTypeResult xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><PAPIErrorCode>1</PAPIErrorCode><ErrorMessage>Not found</ErrorMessage><BibGetByTypeRows i:nil="true" /></BibGetByTypeResult>
                """);

            Assert.AreEqual(1, result.PAPIErrorCode);
            Assert.AreEqual("Not found", result.ErrorMessage);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
