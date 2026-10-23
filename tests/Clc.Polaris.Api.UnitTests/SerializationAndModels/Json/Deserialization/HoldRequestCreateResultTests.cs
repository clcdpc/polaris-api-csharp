using System.Text.Json;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Json.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateResultTests
    {
        [TestMethod]
        public void DocumentedMisspelledQualifierMapsDirectly()
        {
            const string json = """
                {
                  "TxnGroupQualifer": "group"
                }
                """;

            var result = JsonSerializer.Deserialize<HoldRequestCreateResult>(json);
            var newtonsoftResult = Newtonsoft.Json.JsonConvert.DeserializeObject<HoldRequestCreateResult>(json);

            Assert.IsNotNull(result);
            Assert.IsNotNull(newtonsoftResult);
            Assert.AreEqual("group", result.TxnGroupQualifer);
            Assert.AreEqual("group", newtonsoftResult.TxnGroupQualifer);
            Assert.IsNotNull(typeof(HoldRequestCreateResult).GetProperty("TxnGroupQualifer"));
            Assert.IsNull(typeof(HoldRequestCreateResult).GetProperty("TxnGroupQualifier"));
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
