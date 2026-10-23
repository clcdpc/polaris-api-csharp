using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Json.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class BibSearchResultTests
    {
        [TestMethod]
        [DataRow(33)]
        [DataRow(36)]
        [DataRow(41)]
        [DataRow(int.MaxValue)]
        public void PrimaryMaterialTypeRemainsNullableInt32(int materialType)
        {
            var json = JsonSerializer.Serialize(new
            {
                PAPIErrorCode = 1,
                TotalRecordsFound = 1,
                BibSearchRows = new[] { new { ControlNumber = 9001, PrimaryTypeOfMaterial = materialType } }
            });

            var result = JsonSerializer.Deserialize<BibSearchResult>(json);
            var newtonsoftResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BibSearchResult>(json);

            Assert.IsNotNull(result);
            Assert.IsNotNull(newtonsoftResult);
            Assert.AreEqual(materialType, result.BibSearchRows[0].PrimaryTypeOfMaterial);
            Assert.AreEqual(materialType, newtonsoftResult.BibSearchRows[0].PrimaryTypeOfMaterial);
        }

        [TestMethod]
        public void MissingMaterialTypeRemainsUnknown()
        {
            var result = JsonSerializer.Deserialize<BibSearchRow>("""{"ControlNumber":9001}""");
            var newtonsoftResult = Newtonsoft.Json.JsonConvert.DeserializeObject<BibSearchRow>("""{"ControlNumber":9001}""");

            Assert.IsNotNull(result);
            Assert.IsNotNull(newtonsoftResult);
            Assert.IsNull(result.PrimaryTypeOfMaterial);
            Assert.IsNull(newtonsoftResult.PrimaryTypeOfMaterial);
        }

        [TestMethod]
        public void OutOfRangeMaterialTypeIsRejected()
        {
            const string json = """{"ControlNumber":9001,"PrimaryTypeOfMaterial":2147483648}""";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<BibSearchRow>(json));
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() =>
                Newtonsoft.Json.JsonConvert.DeserializeObject<BibSearchRow>(json));
        }
    }
}
