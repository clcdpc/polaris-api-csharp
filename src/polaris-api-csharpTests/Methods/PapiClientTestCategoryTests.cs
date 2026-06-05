using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiClientTestCategoryTests
    {
        private const string ProtectedIntegrationCategory = "ProtectedIntegration";
        private const string MutatingIntegrationCategory = "MutatingIntegration";

        [TestMethod]
        public void KnownStaffRequiredReadOnlyTestsHaveProtectedIntegrationCategory()
        {
            var protectedIntegrationTests = new[]
            {
                nameof(PapiClientTests.AuthenticateStaffUserTest),
                nameof(PapiClientTests.HoldRequestGetListTest),
                nameof(PapiClientTests.Patron_GetBarcodeFromIdTest),
                nameof(PapiClientTests.PatronRenewBlocksGetTest),
                nameof(PapiClientTests.PatronSearchTest),
                nameof(PapiClientTests.RecordSetRecordsGetTest),
                nameof(PapiClientTests.SA_GetValueByOrgTest),
                nameof(PapiClientTests.Synch_BibsByIdGetTest),
            };

            AssertMethodsHaveCategory(protectedIntegrationTests, ProtectedIntegrationCategory);
        }

        [TestMethod]
        public void KnownMutatingTestsHaveMutatingIntegrationCategory()
        {
            var mutatingIntegrationTests = new[]
            {
                nameof(PapiClientTests.CreatePatronBlocksTest_FreeTextBlock),
                nameof(PapiClientTests.CreatePatronBlocksTest_SystemBlock),
                nameof(PapiClientTests.CreatePatronBlocksTest_LibraryAssignedBlock),
                nameof(PapiClientTests.NotificationUpdateTest),
                nameof(PapiClientTests.PatronAccountCreateCreditTest),
                nameof(PapiClientTests.TestTitleListCreate_Get_Delete),
                nameof(PapiClientTests.PatronAccountDepositCreditTest),
                nameof(PapiClientTests.PatronAccountPayTest),
                nameof(PapiClientTests.PatronAccountPayAllTest),
                nameof(PapiClientTests.PatronAccountRefundCreditTest),
                nameof(PapiClientTests.PatronAccountVoidTest),
                nameof(PapiClientTests.PatronMessageDeleteTest),
                nameof(PapiClientTests.PatronMessageUpdateStatusTest),
                nameof(PapiClientTests.PatronReadingHistoryClearTest),
                nameof(PapiClientTests.PatronTitleListAddTitleTest),
                nameof(PapiClientTests.PatronTitleListCopyAllTitlesTest),
                nameof(PapiClientTests.PatronTitleListCopyTitleTest),
                nameof(PapiClientTests.PatronTitleListDeleteAllTitlesTest),
                nameof(PapiClientTests.PatronTitleListDeleteTitleTest),
                nameof(PapiClientTests.PatronTitleListMoveTitleTest),
                nameof(PapiClientTests.PatronUpdateTest),
                nameof(PapiClientTests.RecordSetContentAddTest),
                nameof(PapiClientTests.RecordSetContentAddTest_List),
                nameof(PapiClientTests.RecordSetContentRemoveTest),
                nameof(PapiClientTests.RecordSetContentRemoveTest_List),
            };

            AssertMethodsHaveCategory(mutatingIntegrationTests, MutatingIntegrationCategory);
        }

        private static void AssertMethodsHaveCategory(IEnumerable<string> methodNames, string expectedCategory)
        {
            var missingCategory = methodNames
                .Where(methodName => !MethodHasCategory(methodName, expectedCategory))
                .ToArray();

            Assert.IsFalse(
                missingCategory.Any(),
                $"Expected these {nameof(PapiClientTests)} methods to be marked with TestCategory(\"{expectedCategory}\"): {string.Join(", ", missingCategory)}");
        }

        private static bool MethodHasCategory(string methodName, string expectedCategory)
        {
            var method = typeof(PapiClientTests).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Expected to find public test method {nameof(PapiClientTests)}.{methodName}.");

            return method
                .GetCustomAttributes<TestCategoryAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories)
                .Contains(expectedCategory);
        }
    }
}
