using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiClientCategoryTests
    {
        private const string ProtectedIntegrationCategory = "ProtectedIntegration";
        private const string MutatingIntegrationCategory = "MutatingIntegration";

        private static readonly string[] ProtectedIntegrationTests =
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

        private static readonly string[] MutatingIntegrationTests =
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

        [TestMethod]
        public void ProtectedAndMutatingIntegrationTestsKeepTheirSpecificCategories()
        {
            AssertExpectedCategory(ProtectedIntegrationTests, ProtectedIntegrationCategory);
            AssertExpectedCategory(MutatingIntegrationTests, MutatingIntegrationCategory);
        }

        private static void AssertExpectedCategory(IEnumerable<string> testMethodNames, string expectedCategory)
        {
            var missingCategoryMethodNames = testMethodNames
                .Select(name => new { Name = name, Method = GetTestMethod(name) })
                .Where(test => !HasCategory(test.Method, expectedCategory))
                .Select(test => test.Name)
                .ToArray();

            if (missingCategoryMethodNames.Any())
            {
                Assert.Fail(
                    $"Expected these {nameof(PapiClientTests)} methods to keep TestCategory(\"{expectedCategory}\"): " +
                    string.Join(", ", missingCategoryMethodNames));
            }
        }

        private static MethodInfo GetTestMethod(string methodName)
        {
            var method = typeof(PapiClientTests).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Expected {nameof(PapiClientTests)} to contain a public instance test method named {methodName}.");

            return method;
        }

        private static bool HasCategory(MethodInfo method, string expectedCategory)
        {
            return method
                .GetCustomAttributes<TestCategoryAttribute>()
                .SelectMany(attribute => attribute.TestCategories)
                .Contains(expectedCategory);
        }
    }
}
