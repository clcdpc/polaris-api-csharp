using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Clc.Polaris.Api.LiveIntegrationTests
{
    [TestClass]
    public class PapiClientIntegrationTestCategoryTests
    {
        private const string LegacyIntegrationCategory = "Integration";
        private const string LegacyProtectedIntegrationCategory = "ProtectedIntegration";

        private static readonly string[] IntegrationCategoryNames =
        [
            LiveTestCategories.ReadOnly,
            LiveTestCategories.RequiresStaffOverride,
            LiveTestCategories.Mutating,
            LiveTestCategories.RequiresStaffOverride,
        ];

        private static readonly string[] ProtectedIntegrationCategoryNames =
        [
            LiveTestCategories.RequiresStaffOverride,
            LiveTestCategories.RequiresStaffOverride,
        ];

        private static readonly string[] MutatingCategoryNames =
        [
            LiveTestCategories.Mutating,
            LiveTestCategories.RequiresStaffOverride,
        ];

        private static readonly string[] ReadOnlyCategoryNames =
        [
            LiveTestCategories.ReadOnly,
            LiveTestCategories.RequiresStaffOverride,
        ];

        private static readonly string[] KnownReadOnlyLiveTests =
        [
            "ApiKeyValidateTest",
            "ApiVersionGetTest",
            "BibGetAsync_DefaultBranchReturnsConfiguredBib",
            "BibGetAsync_ExplicitBranchReturnsConfiguredBib",
            "BibSearchTest",
            "BibSearchAsync_KeywordWithSpacesPagingAndBranchReturnsRows",
            "CollectionsGetTest",
            "CollectionsGetAsync_ReturnsRowsForConfiguredBranch",
            "DatesClosedGetAsync_ReturnsRowsForConfiguredBranch",
            "HoldingsGetAsync_ReturnsHoldingsForConfiguredBib",
            "ItemStatusesGetAsyncTest",
            "LimitFiltersGetTest",
            "LimitFiltersGetAsync_ReturnsRowsForConfiguredBranch",
            "MARCTypeOfMaterialsGetAsyncTest",
            "MaterialTypesGetTest",
            "MaterialTypesGetAsync_ReturnsRowsForConfiguredBranch",
            "OrganizationsGetTest",
            "PatronAccountGetTest",
            "PatronBasicDataGetTest",
            "PatronCirculateBlocksGetTest",
            "PatronCirculateBlocksGetAsync_ComparisonOrganizationIdMatchesOrganizationOne",
            "PatronCodesGetTest",
            "PatronHoldRequestsGetTest",
            "PatronILLRequestsGetTest",
            "PatronItemsOutGetAsync_ComparisonOrganizationIdMatchesOrganizationOne",
            "PatronMessagesGetTest",
            "PatronPreferencesGetTest",
            "PatronReadingHistoryGetTest",
            "PatronSavedSearchesGetTest",
            "PatronValidateTest",
            "PatronValidateAsync_ComparisonOrganizationIdMatchesOrganizationOne",
            "PickupBranchesGetTest",
            "PickupBranchesGetAsync_ReturnsConfiguredPickupBranchWhenProvided",
            "ShelfLocationsGetTest",
            "ShelfLocationsGetAsync_ReturnsRowsForConfiguredBranch",
        ];

        private static readonly string[] KnownStaffReadOnlyLiveTests =
        [
            "AuthenticateStaffUserTest",
            "AuthenticateStaffUser_ConfiguredOverrideAccountSucceeds",
            "HoldRequestGetListTest",
            "Patron_GetBarcodeFromIdTest",
            "PatronAccountGet_StaffOverrideWithoutPatronPasswordCanMakeBackToBackCalls",
            "PatronRenewBlocksGetTest",
            "PatronSearchTest",
            "PatronSearch_SameClientCanMakeBackToBackProtectedCallsAndFindConfiguredPatron",
            "RecordSetRecordsGetTest",
            "RecordSetRecordsGetAsync_ComparisonOrganizationIdMatchesOrganizationOne",
            "SA_GetValueByOrgTest",
            "Synch_BibsByIdGetAsync_ReturnsConfiguredBib",
        ];

        private static readonly string[] KnownMutatingLiveTests =
        [
            "HoldRequestLifecycle_CleansExistingConfiguredHoldThenCreatesSuspendsReactivatesAndCancels",
            "HoldRequestReplyTest",
            "ItemRenewTest",
            "PatronMessageDeleteTest",
            "PatronMessageUpdateStatusTest",
            "PatronReadingHistoryClearTest",
            "PatronTitleListLifecycle_CanCreateReadAndDeleteList",
            "PatronTitleListLifecycle_CanPopulateCopyMoveClearAndDeleteLists",
            "PatronUpdateTest",
            "PatronUpdateUserNameTest",
        ];

        private static readonly string[] KnownStaffMutatingLiveTests =
        [
            "CreatePatronBlocksTest_FreeTextBlock",
            "CreatePatronBlocksTest_SystemBlock",
            "CreatePatronBlocksTest_LibraryAssignedBlock",
            "ItemCheckOutCheckInLifecycle_ChecksOutVerifiesItemsOutAndChecksIn",
            "NotificationUpdateTest",
            "PatronAccountCreditAndDeposit_CreateRowsVisibleInAccountReadback",
            "PatronAccountPayTest",
            "PatronAccountPayAllTest",
            "PatronAccountRefundCreditTest",
            "PatronAccountVoidTest",
            "RecordSetLifecycle_CanAddAndRemoveConfiguredRecordWithoutChangingInitialMembership",
        ];

        [TestMethod]
        public void LiveIntegrationTestsInheritIntegrationTestBase()
        {
            var incorrectlyBasedMethods = GetAllTestMethods()
                .Where(method => MethodCategories(method).Intersect(IntegrationCategoryNames).Any())
                .Where(method => method.DeclaringType == null || !typeof(IntegrationTestBase).IsAssignableFrom(method.DeclaringType))
                .Select(method => $"{method.DeclaringType?.FullName}.{method.Name}")
                .OrderBy(methodName => methodName)
                .ToArray();

            Assert.IsEmpty(
                incorrectlyBasedMethods,
                $"Expected every live PAPI client integration test method to be declared on a class that inherits {nameof(IntegrationTestBase)}: {string.Join(", ", incorrectlyBasedMethods)}");
        }

        [TestMethod]
        public void LivePapiClientIntegrationTestsHaveExactlyOneIntegrationCategory()
        {
            var incorrectlyCategorized = GetPapiClientIntegrationTestMethods()
                .Select(method => new
                {
                    method.Name,
                    Categories = MethodCategories(method).Intersect(IntegrationCategoryNames).ToArray(),
                })
                .Where(method => method.Categories.Length != 1)
                .Select(method => $"{method.Name} ({method.Categories.Length}: {string.Join(", ", method.Categories)})")
                .ToArray();

            Assert.IsEmpty(
                incorrectlyCategorized,
                $"Expected every live PAPI client integration test method to have exactly one integration category ({string.Join(", ", IntegrationCategoryNames)}): {string.Join(", ", incorrectlyCategorized)}");
        }

        [TestMethod]
        public void PapiClientIntegrationTestsDoNotUseLegacyIntegrationCategories()
        {
            var legacyCategories = new[]
            {
                LegacyIntegrationCategory,
                LegacyProtectedIntegrationCategory,
            };

            var methodsWithLegacyCategories = GetPapiClientIntegrationTestMethods()
                .Select(method => new
                {
                    method.Name,
                    Categories = MethodCategories(method).Intersect(legacyCategories).ToArray(),
                })
                .Where(method => method.Categories.Length != 0)
                .Select(method => $"{method.Name} ({string.Join(", ", method.Categories)})")
                .ToArray();

            Assert.IsEmpty(
                methodsWithLegacyCategories,
                $"Expected no PAPI client integration test method to use legacy raw integration category traits: {string.Join(", ", methodsWithLegacyCategories)}");
        }

        [TestMethod]
        public void StaffOverrideIntegrationTestsHaveProtectedIntegrationCategory()
        {
            var unprotectedMethods = PapiClientIntegrationTestSourceMethods()
                .Where(method => method.Value.Contains("IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings)"))
                .Select(method => GetPapiClientIntegrationTestMethod(method.Key))
                .Where(method => !MethodCategories(method).Intersect(ProtectedIntegrationCategoryNames).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsEmpty(
                unprotectedMethods,
                $"Expected tests requiring a staff override account to use {LiveTestCategories.RequiresStaffOverride} or {LiveTestCategories.RequiresStaffOverride}: {string.Join(", ", unprotectedMethods)}");
        }

        [TestMethod]
        public void MutatingLiveTestsAreNotParallelized()
        {
            var missingDoNotParallelize = GetPapiClientIntegrationTestMethods()
                .Where(method => MethodCategories(method).Intersect(MutatingCategoryNames).Any())
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsEmpty(
                missingDoNotParallelize,
                $"Expected mutating integration tests to be marked with {nameof(DoNotParallelizeAttribute)}: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void ReadOnlyLiveTestsAreParallelizedUnlessDocumentedException()
        {
            var documentedExceptions = Array.Empty<string>();

            var unexpectedlyNonParallelized = GetPapiClientIntegrationTestMethods()
                .Where(method => !documentedExceptions.Contains(method.Name))
                .Where(method => MethodCategories(method).Intersect(ReadOnlyCategoryNames).Any())
                .Where(method => method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsEmpty(
                unexpectedlyNonParallelized,
                $"Expected read-only integration tests not to be marked with {nameof(DoNotParallelizeAttribute)} unless documented in this test: {string.Join(", ", unexpectedlyNonParallelized)}");
        }

        [TestMethod]
        public void KnownIntegrationTestsHaveExpectedCategories()
        {
            AssertMethodsHaveCategory(KnownReadOnlyLiveTests, LiveTestCategories.ReadOnly);
            AssertMethodsHaveCategory(KnownStaffReadOnlyLiveTests, LiveTestCategories.RequiresStaffOverride);
            AssertMethodsHaveCategory(KnownMutatingLiveTests, LiveTestCategories.Mutating);
            AssertMethodsHaveCategory(KnownStaffMutatingLiveTests, LiveTestCategories.RequiresStaffOverride);
        }

        [TestMethod]
        public void EveryIntegrationTestIsListedInKnownIntegrationTests()
        {
            var knownIntegrationTests = KnownReadOnlyLiveTests
                .Concat(KnownStaffReadOnlyLiveTests)
                .Concat(KnownMutatingLiveTests)
                .Concat(KnownStaffMutatingLiveTests)
                .ToHashSet();

            var unlistedIntegrationTests = GetPapiClientIntegrationTestMethods()
                .Select(method => method.Name)
                .Where(methodName => !knownIntegrationTests.Contains(methodName))
                .OrderBy(methodName => methodName)
                .ToArray();

            Assert.IsEmpty(
                unlistedIntegrationTests,
                $"Expected every live PAPI client integration test to be listed in one of the known integration test arrays: {string.Join(", ", unlistedIntegrationTests)}");
        }

        [TestMethod]
        public void KnownIntegrationTestListsDoNotContainDuplicateMethodNames()
        {
            var duplicateMethodNames = KnownReadOnlyLiveTests
                .Concat(KnownStaffReadOnlyLiveTests)
                .Concat(KnownMutatingLiveTests)
                .Concat(KnownStaffMutatingLiveTests)
                .GroupBy(methodName => methodName)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .OrderBy(methodName => methodName)
                .ToArray();

            Assert.IsEmpty(
                duplicateMethodNames,
                $"Expected known integration test arrays not to duplicate method names across categories: {string.Join(", ", duplicateMethodNames)}");
        }

        private static IEnumerable<MethodInfo> GetPapiClientIntegrationTestMethods()
        {
            return GetAllTestMethods()
                .Where(method => method.DeclaringType != null && typeof(IntegrationTestBase).IsAssignableFrom(method.DeclaringType));
        }

        private static IEnumerable<MethodInfo> GetAllTestMethods()
        {
            return typeof(IntegrationTestBase).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any());
        }

        private static MethodInfo GetPapiClientIntegrationTestMethod(string methodName)
        {
            var method = GetPapiClientIntegrationTestMethods().SingleOrDefault(method => method.Name == methodName);

            Assert.IsNotNull(method, $"Expected to find public PAPI client integration test method {methodName}.");

            return method;
        }

        private static void AssertMethodsHaveCategory(IEnumerable<string> methodNames, string expectedCategory)
        {
            var missingCategory = methodNames
                .Where(methodName => !MethodCategories(GetPapiClientIntegrationTestMethod(methodName)).Contains(expectedCategory))
                .ToArray();

            Assert.IsEmpty(
                missingCategory,
                $"Expected these PAPI client integration test methods to be marked with {expectedCategory}: {string.Join(", ", missingCategory)}");
        }

        private static IEnumerable<string> MethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryBaseAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories);
        }

        private static Dictionary<string, string> PapiClientIntegrationTestSourceMethods([CallerFilePath] string categoryTestSourcePath = "")
        {
            var methods = GetPapiClientIntegrationTestMethods().Select(method => method.Name).ToHashSet();
            var methodsDirectory = Path.GetDirectoryName(categoryTestSourcePath)!;
            var sourceMethods = new Dictionary<string, string>();

            foreach (var testSourcePath in Directory.EnumerateFiles(methodsDirectory, "*Tests.cs"))
            {
                if (Path.GetFileName(testSourcePath) == Path.GetFileName(categoryTestSourcePath))
                {
                    continue;
                }

                var source = File.ReadAllText(testSourcePath);
                var methodMatches = Regex.Matches(
                    source,
                    @"public\s+(?:async\s+)?(?:Task|void)\s+(?<name>\w+)\s*\([^)]*\)\s*\{",
                    RegexOptions.CultureInvariant);

                foreach (Match methodMatch in methodMatches)
                {
                    var methodName = methodMatch.Groups["name"].Value;
                    if (!methods.Contains(methodName))
                    {
                        continue;
                    }

                    var openingBraceIndex = source.IndexOf('{', methodMatch.Index + methodMatch.Length - 1);
                    Assert.AreNotEqual(-1, openingBraceIndex, $"Expected to find opening brace for {methodName}.");

                    var closingBraceIndex = FindClosingBrace(source, openingBraceIndex);
                    sourceMethods[methodName] = source[methodMatch.Index..(closingBraceIndex + 1)];
                }
            }

            return sourceMethods;
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
    }
}