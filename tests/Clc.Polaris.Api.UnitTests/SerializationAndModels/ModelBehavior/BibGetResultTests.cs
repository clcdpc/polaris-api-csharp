namespace Clc.Polaris.Api.UnitTests.SerializationAndModels.ModelBehavior
{
    [TestClass]
    [UnitTest]
    public class BibGetResultTests
    {
        [TestMethod]
        public void NullableIntProperties_MapToExpectedElementIds()
        {
            var sut = new BibGetResult
            {
                BibGetRows =
                [
                    new() { ElementID = 7, Value = "70" },
                    new() { ElementID = 8, Value = "80" },
                    new() { ElementID = 11, Value = "110" },
                    new() { ElementID = 15, Value = "150" },
                    new() { ElementID = 16, Value = "160" }
                ]
            };

            Assert.AreEqual(70, sut.SystemItemsTotal);
            Assert.AreEqual(80, sut.CurrentHolds);
            Assert.AreEqual(110, sut.ControlNumber);
            Assert.AreEqual(150, sut.LocalItemsAvailable);
            Assert.AreEqual(160, sut.SystemItemsAvailable);
        }

        [TestMethod]
        [DataRow("123", 123)]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        [DataRow("2147483647", 2147483647)]
        [DataRow("-2147483648", -2147483648)]
        public void SystemItemsTotal_ParsesValidIntegers(string value, int expected)
        {
            var sut = CreateWithSystemItemsTotalRows(value);

            Assert.AreEqual(expected, sut.SystemItemsTotal);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("not-a-number")]
        [DataRow("12.3")]
        [DataRow("2147483648")]
        public void SystemItemsTotal_ReturnsNullForInvalidValues(string value)
        {
            var sut = CreateWithSystemItemsTotalRows(value);

            Assert.IsNull(sut.SystemItemsTotal);
        }

        [TestMethod]
        public void SystemItemsTotal_ReturnsNullWhenRowMissing()
        {
            var sut = new BibGetResult
            {
                BibGetRows =
                [
                    new() { ElementID = 8, Value = "42" }
                ]
            };

            Assert.IsNull(sut.SystemItemsTotal);
        }

        [TestMethod]
        public void SystemItemsTotal_UsesFirstRowValue_WhenMultipleRowsExist()
        {
            var sut = CreateWithSystemItemsTotalRows("123", "456");

            Assert.AreEqual(123, sut.SystemItemsTotal);
        }

        [TestMethod]
        public void SystemItemsTotal_UsesFirstRowValue_WhenFirstRowInvalidAndSecondRowValid()
        {
            var sut = CreateWithSystemItemsTotalRows("not-a-number", "123");

            Assert.IsNull(sut.SystemItemsTotal);
        }

        private static BibGetResult CreateWithSystemItemsTotalRows(params string[] values)
        {
            var rows = new List<BibGetRow>();
            foreach (var value in values)
            {
                rows.Add(new BibGetRow { ElementID = 7, Value = value });
            }

            return new BibGetResult { BibGetRows = rows };
        }
    }
}
