using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
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

        private static readonly string[] KnownReadOnlyIntegrationTests =
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

        private static readonly string[] KnownProtectedIntegrationTests =
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

        private static readonly string[] KnownMutatingIntegrationTests =
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
        public void SpecializedIntegrationCategoriesRequireIntegrationCategory()
        {
            var missingIntegration = GetPapiClientTestMethods()
                .Where(method =>
                    (MethodHasCategory(method, ReadOnlyIntegrationCategory)
                        || MethodHasCategory(method, ProtectedIntegrationCategory)
                        || MethodHasCategory(method, MutatingIntegrationCategory))
                    && !MethodOrClassHasCategory(method, IntegrationCategory))
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingIntegration.Any(),
                $"Expected specialized integration tests to also be marked with TestCategory(\"{IntegrationCategory}\") at method or class level: {string.Join(", ", missingIntegration)}");
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
                $"Expected no test to have both TestCategory(\"{ReadOnlyIntegrationCategory}\") and TestCategory(\"{MutatingIntegrationCategory}\"): {string.Join(", ", conflictingMethods)}");
        }

        [TestMethod]
        public void MutatingIntegrationTestsAreNotParallelized()
        {
            var missingDoNotParallelize = GetPapiClientTestMethods()
                .Where(method => MethodHasCategory(method, MutatingIntegrationCategory))
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingDoNotParallelize.Any(),
                $"Expected mutating integration tests to be marked with {nameof(DoNotParallelizeAttribute)} unless a documented exception is added to this test: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void StaffOverrideIntegrationTestsHaveProtectedIntegrationCategory()
        {
            var methodsRequiringStaffOverride = PapiClientTestSourceMethods()
                .Where(method => method.Body.Contains("IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings)"))
                .Select(method => method.Name)
                .ToArray();

            AssertMethodsHaveCategory(methodsRequiringStaffOverride, ProtectedIntegrationCategory);
        }

        [TestMethod]
        public void KnownReadOnlyTestsHaveReadOnlyIntegrationCategory()
        {
            AssertMethodsHaveCategory(KnownReadOnlyIntegrationTests, ReadOnlyIntegrationCategory);
        }

        [TestMethod]
        public void KnownStaffRequiredReadOnlyTestsHaveProtectedIntegrationCategory()
        {
            AssertMethodsHaveCategory(KnownProtectedIntegrationTests, ProtectedIntegrationCategory);
        }

        [TestMethod]
        public void KnownMutatingTestsHaveMutatingIntegrationCategory()
        {
            AssertMethodsHaveCategory(KnownMutatingIntegrationTests, MutatingIntegrationCategory);
        }

        [TestMethod]
        public void KnownMutatingTestsDoNotHaveReadOnlyIntegrationCategory()
        {
            var incorrectlyReadOnly = KnownMutatingIntegrationTests
                .Where(methodName => MethodHasCategory(methodName, ReadOnlyIntegrationCategory))
                .ToArray();

            Assert.IsFalse(
                incorrectlyReadOnly.Any(),
                $"Expected known mutating tests not to be marked with TestCategory(\"{ReadOnlyIntegrationCategory}\"): {string.Join(", ", incorrectlyReadOnly)}");
        }

        private static IEnumerable<MethodInfo> GetPapiClientTestMethods()
        {
            return typeof(PapiClientTests)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any());
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

            return MethodHasCategory(method, expectedCategory);
        }

        private static bool MethodHasCategory(MethodInfo method, string expectedCategory)
        {
            return method
                .GetCustomAttributes<TestCategoryAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories)
                .Contains(expectedCategory);
        }

        private static bool MethodOrClassHasCategory(MethodInfo method, string expectedCategory)
        {
            return MethodHasCategory(method, expectedCategory)
                || typeof(PapiClientTests)
                    .GetCustomAttributes<TestCategoryAttribute>(inherit: false)
                    .SelectMany(attribute => attribute.TestCategories)
                    .Contains(expectedCategory);
        }

        private static IEnumerable<(string Name, string Body)> PapiClientTestSourceMethods()
        {
            var source = File.ReadAllText(FindPapiClientTestsSourcePath());
            var methodMatches = Regex.Matches(
                source,
                @"public\s+async\s+Task\s+(?<name>\w+)\s*\(",
                RegexOptions.CultureInvariant);

            foreach (Match methodMatch in methodMatches)
            {
                var openingBraceIndex = source.IndexOf('{', methodMatch.Index + methodMatch.Length);
                Assert.AreNotEqual(-1, openingBraceIndex, $"Expected to find opening brace for {methodMatch.Groups["name"].Value}.");

                var closingBraceIndex = FindClosingBrace(source, openingBraceIndex);
                yield return (methodMatch.Groups["name"].Value, source.Substring(openingBraceIndex, closingBraceIndex - openingBraceIndex + 1));
            }
        }

        private static int FindClosingBrace(string source, int openingBraceIndex)
        {
            var depth = 0;

            for (var i = openingBraceIndex; i < source.Length; i++)
            {
                if (source[i] == '{')
                {
                    depth++;
                }
                else if (source[i] == '}')
                {
                    depth--;

                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }

            Assert.Fail($"Expected to find closing brace matching character index {openingBraceIndex}.");
            return -1;
        }

        private static string FindPapiClientTestsSourcePath()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, "Methods", "PapiClientTests.cs");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                candidate = Path.Combine(directory.FullName, "src", "polaris-api-csharpTests", "Methods", "PapiClientTests.cs");
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            Assert.Fail("Expected to find Methods/PapiClientTests.cs while walking up from the test output directory.");
            return string.Empty;
        }
    }
}
