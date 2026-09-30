using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Json.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateResultTests
    {
        [TestMethod]
        [DataRow("TxnGroupQualifier", "TxnGroupQualifer")]
        [DataRow("TxnGroupQualifer", "TxnGroupQualifier")]
        public void CanonicalQualifierTakesPrecedenceRegardlessOfFieldOrder(string first, string second)
        {
            var fields = new Dictionary<string, object>
            {
                [first] = first == "TxnGroupQualifier" ? "canonical" : "legacy",
                [second] = second == "TxnGroupQualifier" ? "canonical" : "legacy"
            };
            var json = JsonSerializer.Serialize(fields);

            var result = JsonSerializer.Deserialize<HoldRequestCreateResult>(json);
            var newtonsoftResult = Newtonsoft.Json.JsonConvert.DeserializeObject<HoldRequestCreateResult>(json);

            Assert.IsNotNull(result);
            Assert.IsNotNull(newtonsoftResult);
            Assert.AreEqual("canonical", result.TxnGroupQualifier);
            Assert.AreEqual("canonical", newtonsoftResult.TxnGroupQualifier);
        }

        [TestMethod]
        public void QueuePositionRetainsSourceCompatibleAlias()
        {
            var result = new HoldRequestCreateResult { QueuePostition = 7 };

            Assert.AreEqual(7, result.QueuePosition);
            Assert.DoesNotContain("QueuePostition", JsonSerializer.Serialize(result));
            Assert.DoesNotContain("QueuePostition", Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }
    }
}
