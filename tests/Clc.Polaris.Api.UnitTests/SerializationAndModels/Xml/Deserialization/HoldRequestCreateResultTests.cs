using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.Xml.Deserialization
{
    [TestClass]
    [UnitTest]
    public sealed class HoldRequestCreateResultTests
    {
        [TestMethod]
        public void DocumentedResponseMapsGuidMisspelledQualifierAndQueuePosition()
        {
            var requestGuid = Guid.Parse("7f279d6e-cfde-4ae3-be96-ee12305cdf11");
            var xml = $"""
                <HoldRequestResult>
                  <PAPIErrorCode>0</PAPIErrorCode>
                  <RequestGUID>{requestGuid}</RequestGUID>
                  <TxnGroupQualifer>group</TxnGroupQualifer>
                  <TxnQualifier>qualifier</TxnQualifier>
                  <StatusType>3</StatusType>
                  <StatusValue>5</StatusValue>
                  <QueuePosition>2</QueuePosition>
                  <QueueTotal>3</QueueTotal>
                </HoldRequestResult>
                """;
            using var reader = new StringReader(xml);

            var result = (HoldRequestCreateResult)new XmlSerializer(typeof(HoldRequestCreateResult)).Deserialize(reader)!;

            Assert.AreEqual(requestGuid, result.RequestGuid);
            Assert.AreEqual("group", result.TxnGroupQualifer);
            Assert.AreEqual("qualifier", result.TxnQualifier);
            Assert.AreEqual(3, result.StatusType);
            Assert.AreEqual(5, result.StatusValue);
            Assert.AreEqual(2, result.QueuePosition);
            Assert.AreEqual(3, result.QueueTotal);
        }

        [TestMethod]
        public void SuccessfulReplyWithEmptyRequestGuidPreservesStatusAndNullIdentity()
        {
            using var reader = new StringReader("""
                <HoldRequestResult>
                  <PAPIErrorCode>0</PAPIErrorCode>
                  <RequestGUID />
                  <TxnGroupQualifer />
                  <TxnQualifier />
                  <StatusType>2</StatusType>
                  <StatusValue>1</StatusValue>
                  <QueuePosition>2</QueuePosition>
                  <QueueTotal>3</QueueTotal>
                </HoldRequestResult>
                """);

            var result = (HoldRequestReplyResult)new XmlSerializer(typeof(HoldRequestReplyResult)).Deserialize(reader)!;

            Assert.IsNull(result.RequestGuid);
            Assert.AreEqual(2, result.StatusType);
            Assert.AreEqual(1, result.StatusValue);
            Assert.AreEqual(2, result.QueuePosition);
            Assert.AreEqual(3, result.QueueTotal);
        }
    }
}
