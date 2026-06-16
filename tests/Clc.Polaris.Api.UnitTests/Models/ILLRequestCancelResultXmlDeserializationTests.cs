using System.IO;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class ILLRequestCancelResultXmlDeserializationTests
    {
        [TestMethod]
        public void ILLRequestCancelResult_DeserializesNilAndPopulatedRows()
        {
            var nil = Deserialize<ILLRequestCancelResult>("<ILLRequestCancelResult xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><PAPIErrorCode>0</PAPIErrorCode><ILLRequestCancelRows i:nil=\"true\" /></ILLRequestCancelResult>");
            var populated = Deserialize<ILLRequestCancelResult>("<ILLRequestCancelResult><PAPIErrorCode>0</PAPIErrorCode><ILLRequestCancelRows><ILLRequestCancelRow><ILLRequestID>1</ILLRequestID><ReturnCode>0</ReturnCode><ErrorMessage /></ILLRequestCancelRow><ILLRequestCancelRow><ILLRequestID>2</ILLRequestID><ReturnCode>1</ReturnCode><ErrorMessage>No</ErrorMessage></ILLRequestCancelRow></ILLRequestCancelRows></ILLRequestCancelResult>");

            Assert.IsNotNull(nil.ILLRequestCancelRows);
            Assert.IsEmpty(nil.ILLRequestCancelRows);
            Assert.AreEqual(2, populated.ILLRequestCancelRows!.Count);
            Assert.AreEqual("No", populated.ILLRequestCancelRows[1].ErrorMessage);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
