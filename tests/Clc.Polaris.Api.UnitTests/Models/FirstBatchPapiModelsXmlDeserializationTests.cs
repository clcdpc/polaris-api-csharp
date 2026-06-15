using System.IO;
using System.Xml.Serialization;

namespace Clc.Polaris.Api.UnitTests.Models
{
    [TestClass]
    [UnitTest]
    public sealed class FirstBatchPapiModelsXmlDeserializationTests
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

        [TestMethod]
        public void MultipartGetResult_DeserializesRows()
        {
            var result = Deserialize<MultipartGetResult>("""
                <MultipartGetResult><PAPIErrorCode>0</PAPIErrorCode><MultipartRows><MultipartRow><Type>1</Type><Value>A</Value><ItemBarcode>1</ItemBarcode></MultipartRow><MultipartRow><Type>2</Type><Value>B</Value><ItemBarcode>2</ItemBarcode></MultipartRow><MultipartRow><Type>3</Type><Value>C</Value><ItemBarcode>3</ItemBarcode></MultipartRow></MultipartRows></MultipartGetResult>
                """);

            Assert.AreEqual(3, result.MultipartRows.Count);
            Assert.AreEqual(2, result.MultipartRows[1].Type);
        }

        [TestMethod]
        public void HeadingsSearchResult_DeserializesHexHeadingId()
        {
            var result = Deserialize<HeadingsSearchResult>("""
                <HeadingsSearchResult><PAPIErrorCode>0</PAPIErrorCode><HeadingsSearchRows><HeadingsSearchRow><Position>1</Position><DisplayType>2</DisplayType><DisplayConstant>Author</DisplayConstant><DisplayTerm>Twain</DisplayTerm><GlobalOccurrences>3</GlobalOccurrences><HeadingID>0xABCDEF</HeadingID></HeadingsSearchRow></HeadingsSearchRows></HeadingsSearchResult>
                """);

            Assert.AreEqual(2, result.HeadingsSearchRows[0].DisplayType);
            Assert.AreEqual("0xABCDEF", result.HeadingsSearchRows[0].HeadingID);
        }

        [TestMethod]
        public void LookupResults_DeserializeRows()
        {
            var sort = Deserialize<SortOptionsGetResult>("<SortOptionsGetResult><PAPIErrorCode>0</PAPIErrorCode><SortOptionsRows><SortOptionsRow><Code>TI</Code><Description>Title</Description><Options>asc</Options></SortOptionsRow></SortOptionsRows></SortOptionsGetResult>");
            var statuses = Deserialize<SysHoldStatusesGetResult>("<SysHoldStatusesGetResult><PAPIErrorCode>0</PAPIErrorCode><SysHoldStatusesRows><SysHoldStatusesRow><SysHoldStatusID>1</SysHoldStatusID><Description>Active</Description><Name>Active</Name></SysHoldStatusesRow></SysHoldStatusesRows></SysHoldStatusesGetResult>");

            Assert.AreEqual("TI", sort.SortOptionsRows[0].Code);
            Assert.AreEqual(1, statuses.SysHoldStatusesRows[0].SysHoldStatusID);
        }

        [TestMethod]
        public void SAMobilePhoneCarriersGetResult_DeserializesDisplayValues()
        {
            var result = Deserialize<SAMobilePhoneCarriersGetResult>("""
                <SAMobilePhoneCarriersGetResult><PAPIErrorCode>0</PAPIErrorCode><SAMobilePhoneCarriersGetRows><SAMobilePhoneCarriersGetRow><CarrierID>1</CarrierID><CarrierName>A</CarrierName><Email2SMSEmailAddress>a.example</Email2SMSEmailAddress><NumberOfDigits>10</NumberOfDigits><Display>true</Display></SAMobilePhoneCarriersGetRow><SAMobilePhoneCarriersGetRow><CarrierID>2</CarrierID><CarrierName>B</CarrierName><Email2SMSEmailAddress>b.example</Email2SMSEmailAddress><NumberOfDigits>7</NumberOfDigits><Display>false</Display></SAMobilePhoneCarriersGetRow></SAMobilePhoneCarriersGetRows></SAMobilePhoneCarriersGetResult>
                """);

            Assert.AreEqual(true, result.SAMobilePhoneCarriersGetRows[0].Display);
            Assert.AreEqual(false, result.SAMobilePhoneCarriersGetRows[1].Display);
        }

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

        [TestMethod]
        public void PapiResponseCommon_DeserializesRequestsUpdateStatusSample()
        {
            var result = Deserialize<PapiResponseCommon>("<PapiResponseCommon><PAPIErrorCode>0</PAPIErrorCode><ErrorMessage /></PapiResponseCommon>");

            Assert.AreEqual(0, result.PAPIErrorCode);
        }

        private static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
