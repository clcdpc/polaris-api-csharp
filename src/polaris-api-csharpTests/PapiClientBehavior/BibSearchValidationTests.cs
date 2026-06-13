namespace Clc.Polaris.Api.Tests.PapiClientBehavior
{
    [TestClass]
    [UnitTest]
    public sealed class BibSearchValidationTests : PapiClientTestBase
    {
        [TestMethod]
        public async Task BibSearchAsync_NullOptions_ThrowsArgumentNullException()
        {
            var client = CreateClient();

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
                await client.BibSearchAsync(null!, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task BibSearchAsync_InvalidTerm_ThrowsArgumentException(string? term)
        {
            var client = CreateClient();
            var options = CreateValidOptions();
            options.Term = term;

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await client.BibSearchAsync(options, cancellationToken: TestContext.CancellationToken));
        }

        [TestMethod]
        [DataRow(nameof(BibSearchOptions.Branch), 0)]
        [DataRow(nameof(BibSearchOptions.Branch), -1)]
        [DataRow(nameof(BibSearchOptions.Page), 0)]
        [DataRow(nameof(BibSearchOptions.Page), -1)]
        [DataRow(nameof(BibSearchOptions.PageSize), 0)]
        [DataRow(nameof(BibSearchOptions.PageSize), -1)]
        public async Task BibSearchAsync_InvalidPositiveOption_ThrowsArgumentOutOfRangeException(string propertyName, int value)
        {
            var client = CreateClient();
            var options = CreateValidOptions();

            SetBibSearchOptionValue(options, propertyName, value);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(async () =>
                await client.BibSearchAsync(options, cancellationToken: TestContext.CancellationToken));
        }

        private static BibSearchOptions CreateValidOptions()
        {
            return new BibSearchOptions
            {
                Term = "test",
                Branch = 1,
                Page = 1,
                PageSize = 10
            };
        }

        private static void SetBibSearchOptionValue(BibSearchOptions options, string propertyName, int value)
        {
            switch (propertyName)
            {
                case nameof(BibSearchOptions.Branch):
                    options.Branch = value;
                    break;

                case nameof(BibSearchOptions.Page):
                    options.Page = value;
                    break;

                case nameof(BibSearchOptions.PageSize):
                    options.PageSize = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported BibSearchOptions property.");
            }
        }
    }
}
