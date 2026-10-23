using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Xml.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class BibSearchResultTests
    {
        [TestMethod]
        public void DocumentedPrimaryMaterialTypeDeserializesAsInt32()
        {
            using var reader = new StringReader("""
                <BibSearchResult>
                  <PAPIErrorCode>1</PAPIErrorCode>
                  <TotalRecordsFound>1</TotalRecordsFound>
                  <BibSearchRows>
                    <BibSearchRow>
                      <ControlNumber>9001</ControlNumber>
                      <PrimaryTypeOfMaterial>36</PrimaryTypeOfMaterial>
                    </BibSearchRow>
                  </BibSearchRows>
                </BibSearchResult>
                """);

            var result = (BibSearchResult)new XmlSerializer(typeof(BibSearchResult)).Deserialize(reader)!;

            Assert.AreEqual(9001, result.BibSearchRows[0].ControlNumber);
            Assert.AreEqual(36, result.BibSearchRows[0].PrimaryTypeOfMaterial);
        }

        [TestMethod]
        public void MissingPrimaryMaterialTypeRemainsUnknown()
        {
            using var reader = new StringReader("""<BibSearchRow><ControlNumber>9001</ControlNumber></BibSearchRow>""");

            var result = (BibSearchRow)new XmlSerializer(typeof(BibSearchRow)).Deserialize(reader)!;

            Assert.IsNull(result.PrimaryTypeOfMaterial);
        }
    }
}
