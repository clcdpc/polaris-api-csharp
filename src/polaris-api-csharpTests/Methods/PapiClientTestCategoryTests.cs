using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class PapiClientTestCategoryTests
    {
        private const string IntegrationCategory = "Integration";
        private const string ReadOnlyIntegrationCategory = "ReadOnlyIntegration";
        private const string ProtectedIntegrationCategory = "ProtectedIntegration";
        private const string MutatingIntegrationCategory = "MutatingIntegration";

        private static readonly string[] ReadOnlyIntegrationTests =
        {
            nameof(PapiClientTests.ApiKeyValidateTest),
            nameof(PapiClientTests.ApiVersionGetTest),
            nameof(PapiClientTests.BibGetTest),
            nameof(PapiClientTests.BibGetTest_PassBranchId),
            nameof(PapiClientTests.BibSearchTest),
            nameof(PapiClientTests.CollectionsGetTest),
            nameof(PapiClientTests.DatesClosedGetTest),
            nameof(PapiClientTests.HoldingsGetTest),
            nameof(PapiClientTests.ItemStatusesGetAsyncTest),
            nameof(PapiClientTests.LimitFiltersGetTest),
            nameof(PapiClientTests.MARCTypeOfMaterialsGetAsyncTest),
            nameof(PapiClientTests.OrganizationsGetTest),
            nameof(PapiClientTests.PatronAccountGetTest),
            nameof(PapiClientTests.PatronBasicDataGetTest),
            nameof(PapiClientTests.PatronCirculateBlocksGetTest),
            nameof(PapiClientTests.PatronCodesGetTest),
            nameof(PapiClientTests.PatronHoldRequestsGetTest),
            nameof(PapiClientTests.PatronILLRequestsGetTest),
            nameof(PapiClientTests.PatronItemsOutGetTest),
            nameof(PapiClientTests.PatronMessagesGetTest),
            nameof(PapiClientTests.PatronPreferencesGetTest),
            nameof(PapiClientTests.PatronReadingHistoryGetTest),
            nameof(PapiClientTests.PatronSavedSearchesGetTest),
            nameof(PapiClientTests.PatronTitleListGetTitlesTest),
            nameof(PapiClientTests.PatronValidateTest),
            nameof(PapiClientTests.PickupBranchesGetTest),
            nameof(PapiClientTests.ShelfLocationsGetTest),
            nameof(PapiClientTests.AuthenticateStaffUserTest),
            nameof(PapiClientTests.HoldRequestGetListTest),
            nameof(PapiClientTests.Patron_GetBarcodeFromIdTest),
            nameof(PapiClientTests.PatronRenewBlocksGetTest),
            nameof(PapiClientTests.PatronSearchTest),
            nameof(PapiClientTests.RecordSetRecordsGetTest),
            nameof(PapiClientTests.SA_GetValueByOrgTest),
            nameof(PapiClientTests.Synch_BibsByIdGetTest),
        };

        private static readonly string[] ProtectedReadOnlyIntegrationTests =
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
            nameof(PapiClientTests.HoldRequestCancelTest),
            nameof(PapiClientTests.HoldRequestCreateTest),
            nameof(PapiClientTests.HoldRequestCreateTest2),
            nameof(PapiClientTests.HoldRequestReactivateTest),
            nameof(PapiClientTests.HoldRequestReplyTest),
            nameof(PapiClientTests.HoldRequestSuspendTest),
            nameof(PapiClientTests.ItemRenewTest),
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
            nameof(PapiClientTests.PatronUpdateUserNameTest),
            nameof(PapiClientTests.RecordSetContentAddTest),
            nameof(PapiClientTests.RecordSetContentAddTest_List),
            nameof(PapiClientTests.RecordSetContentRemoveTest),
            nameof(PapiClientTests.RecordSetContentRemoveTest_List),
        };

        [TestMethod]
        public void SpecializedIntegrationCategoriesAlwaysIncludeIntegrationCategory()
        {
            var missingIntegrationCategory = GetPapiClientTestMethods()
                .Where(method => MethodHasAnyCategory(method, ReadOnlyIntegrationCategory, ProtectedIntegrationCategory, MutatingIntegrationCategory))
                .Where(method => !MethodOrClassHasCategory(method, IntegrationCategory))
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingIntegrationCategory.Any(),
                $"Expected methods with specialized integration categories to also have TestCategory(\"{IntegrationCategory}\") at method level or class level: {string.Join(", ", missingIntegrationCategory)}");
        }

        [TestMethod]
        public void ReadOnlyAndMutatingIntegrationCategoriesAreMutuallyExclusive()
        {
            var conflictingMethods = GetPapiClientTestMethods()
                .Where(method => MethodHasCategory(method, ReadOnlyIntegrationCategory) && MethodHasCategory(method, MutatingIntegrationCategory))
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                conflictingMethods.Any(),
                $"Expected no methods to have both TestCategory(\"{ReadOnlyIntegrationCategory}\") and TestCategory(\"{MutatingIntegrationCategory}\"): {string.Join(", ", conflictingMethods)}");
        }

        [TestMethod]
        public void MutatingIntegrationTestsAreNotParallelized()
        {
            var documentedExceptions = Array.Empty<string>();
            var missingDoNotParallelize = GetPapiClientTestMethods()
                .Where(method => MethodHasCategory(method, MutatingIntegrationCategory))
                .Where(method => !documentedExceptions.Contains(method.Name))
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingDoNotParallelize.Any(),
                $"Expected MutatingIntegration methods to have [DoNotParallelize] unless documented in {nameof(documentedExceptions)}: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void StaffOverrideRequiredTestsHaveProtectedIntegrationCategory()
        {
            var staffRequiredMethods = GetPapiClientTestsSourceMethods()
                .Where(method => method.Value.Contains("IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings)", StringComparison.Ordinal))
                .Select(method => method.Key)
                .ToArray();

            AssertMethodsHaveCategory(staffRequiredMethods, ProtectedIntegrationCategory);
        }

        [TestMethod]
        public void KnownReadOnlyIntegrationTestsHaveReadOnlyIntegrationCategory()
        {
            AssertMethodsHaveCategory(ReadOnlyIntegrationTests, ReadOnlyIntegrationCategory);
        }

        [TestMethod]
        public void KnownProtectedReadOnlyIntegrationTestsHaveProtectedIntegrationCategory()
        {
            AssertMethodsHaveCategory(ProtectedReadOnlyIntegrationTests, ProtectedIntegrationCategory);
        }

        [TestMethod]
        public void KnownMutatingIntegrationTestsHaveMutatingIntegrationCategory()
        {
            AssertMethodsHaveCategory(MutatingIntegrationTests, MutatingIntegrationCategory);
        }

        [TestMethod]
        public void KnownMutatingIntegrationTestsDoNotHaveReadOnlyIntegrationCategory()
        {
            var mutatingTestsWithReadOnlyCategory = MutatingIntegrationTests
                .Where(methodName => MethodHasCategory(methodName, ReadOnlyIntegrationCategory))
                .ToArray();

            Assert.IsFalse(
                mutatingTestsWithReadOnlyCategory.Any(),
                $"Expected known mutating integration tests not to have TestCategory(\"{ReadOnlyIntegrationCategory}\"): {string.Join(", ", mutatingTestsWithReadOnlyCategory)}");
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

        private static IEnumerable<MethodInfo> GetPapiClientTestMethods()
        {
            return typeof(PapiClientTests)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any());
        }

        private static bool MethodOrClassHasCategory(MethodInfo method, string expectedCategory)
        {
            return MethodHasCategory(method, expectedCategory)
                || typeof(PapiClientTests)
                    .GetCustomAttributes<TestCategoryAttribute>(inherit: false)
                    .SelectMany(attribute => attribute.TestCategories)
                    .Contains(expectedCategory);
        }

        private static bool MethodHasAnyCategory(MethodInfo method, params string[] expectedCategories)
        {
            var methodCategories = GetMethodCategories(method);
            return expectedCategories.Any(methodCategories.Contains);
        }

        private static bool MethodHasCategory(string methodName, string expectedCategory)
        {
            var method = typeof(PapiClientTests).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Expected to find public test method {nameof(PapiClientTests)}.{methodName}.");

            return MethodHasCategory(method, expectedCategory);
        }

        private static bool MethodHasCategory(MethodInfo method, string expectedCategory)
        {
            return GetMethodCategories(method).Contains(expectedCategory);
        }

        private static string[] GetMethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories)
                .ToArray();
        }

        private static Dictionary<string, string> GetPapiClientTestsSourceMethods([CallerFilePath] string categoryTestSourcePath = "")
        {
            var testSourcePath = Path.Combine(Path.GetDirectoryName(categoryTestSourcePath)!, "PapiClientTests.cs");
            var testSource = File.ReadAllText(testSourcePath);
            var sourceMethods = new Dictionary<string, string>();
            var matches = Regex.Matches(testSource, @"public\s+(?:async\s+)?(?:Task|void)\s+(?<name>\w+)\s*\([^)]*\)\s*\{");

            foreach (Match match in matches)
            {
                var bodyStart = match.Index;
                var bodyEnd = FindMatchingCloseBrace(testSource, testSource.IndexOf('{', match.Index));
                sourceMethods[match.Groups["name"].Value] = testSource[bodyStart..(bodyEnd + 1)];
            }

            return sourceMethods;
        }

        private static int FindMatchingCloseBrace(string source, int openBraceIndex)
        {
            var depth = 0;
            for (var index = openBraceIndex; index < source.Length; index++)
            {
                if (source[index] == '{')
                {
                    depth++;
                }
                else if (source[index] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return index;
                    }
                }
            }

            Assert.Fail($"Could not find matching close brace in {nameof(PapiClientTests)} source.");
            return -1;
        }
    }
}
