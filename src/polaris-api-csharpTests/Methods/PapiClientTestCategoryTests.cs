using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Clc.Polaris.Api.Tests
{
    [TestClass]
    [UnitCategory]
    public class PapiClientTestCategoryTests
    {
        private const string LegacyIntegrationCategory = "Integration";
        private const string LegacyProtectedIntegrationCategory = "ProtectedIntegration";

        private static readonly string[] IntegrationCategoryNames =
        {
            TestCategories.ReadOnlyIntegration,
            TestCategories.ProtectedReadOnlyIntegration,
            TestCategories.MutatingIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] ProtectedIntegrationCategoryNames =
        {
            TestCategories.ProtectedReadOnlyIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] MutatingIntegrationCategoryNames =
        {
            TestCategories.MutatingIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] ReadOnlyIntegrationCategoryNames =
        {
            TestCategories.ReadOnlyIntegration,
            TestCategories.ProtectedReadOnlyIntegration,
        };

        private static readonly string[] KnownReadOnlyIntegrationTests =
        {
            "ApiKeyValidateTest",
            "ApiVersionGetTest",
            "BibGetTest",
            "BibGetTest_PassBranchId",
            "BibSearchTest",
            "CollectionsGetTest",
            "DatesClosedGetTest",
            "HoldingsGetTest",
            "ItemStatusesGetAsyncTest",
            "LimitFiltersGetTest",
            "MARCTypeOfMaterialsGetAsyncTest",
            "OrganizationsGetTest",
            "PatronAccountGetTest",
            "PatronBasicDataGetTest",
            "PatronCirculateBlocksGetTest",
            "PatronCodesGetTest",
            "PatronHoldRequestsGetTest",
            "PatronILLRequestsGetTest",
            "PatronItemsOutGetTest",
            "PatronMessagesGetTest",
            "PatronPreferencesGetTest",
            "PatronReadingHistoryGetTest",
            "PatronSavedSearchesGetTest",
            "PatronTitleListGetTitlesTest",
            "PatronValidateTest",
            "PickupBranchesGetTest",
            "ShelfLocationsGetTest",
        };

        private static readonly string[] KnownProtectedReadOnlyIntegrationTests =
        {
            "AuthenticateStaffUserTest",
            "HoldRequestGetListTest",
            "Patron_GetBarcodeFromIdTest",
            "PatronRenewBlocksGetTest",
            "PatronSearchTest",
            "RecordSetRecordsGetTest",
            "SA_GetValueByOrgTest",
            "Synch_BibsByIdGetTest",
        };

        private static readonly string[] KnownMutatingIntegrationTests =
        {
            "HoldRequestCancelTest",
            "HoldRequestCreateTest",
            "HoldRequestCreateTest2",
            "HoldRequestReactivateTest",
            "HoldRequestReplyTest",
            "HoldRequestSuspendTest",
            "ItemRenewTest",
            "PatronAccountCreateTitleList_CreatedListCanBeFoundAndDeleted",
            "PatronMessageDeleteTest",
            "PatronMessageUpdateStatusTest",
            "PatronReadingHistoryClearTest",
            "PatronTitleListAddTitleTest",
            "PatronTitleListCopyAllTitlesTest",
            "PatronTitleListCopyTitleTest",
            "PatronTitleListDeleteAllTitlesTest",
            "PatronTitleListDeleteTitleTest",
            "PatronTitleListMoveTitleTest",
            "PatronUpdateTest",
            "PatronUpdateUserNameTest",
        };

        private static readonly string[] KnownProtectedMutatingIntegrationTests =
        {
            "CreatePatronBlocksTest_FreeTextBlock",
            "CreatePatronBlocksTest_SystemBlock",
            "CreatePatronBlocksTest_LibraryAssignedBlock",
            "NotificationUpdateTest",
            "PatronAccountCreateCreditTest",
            "PatronAccountDepositCreditTest",
            "PatronAccountPayTest",
            "PatronAccountPayAllTest",
            "PatronAccountRefundCreditTest",
            "PatronAccountVoidTest",
            "RecordSetContentAddTest",
            "RecordSetContentAddTest_List",
            "RecordSetContentRemoveTest",
            "RecordSetContentRemoveTest_List",
        };

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

            Assert.IsFalse(
                incorrectlyCategorized.Any(),
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
                .Where(method => method.Categories.Any())
                .Select(method => $"{method.Name} ({string.Join(", ", method.Categories)})")
                .ToArray();

            Assert.IsFalse(
                methodsWithLegacyCategories.Any(),
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

            Assert.IsFalse(
                unprotectedMethods.Any(),
                $"Expected tests requiring a staff override account to use {TestCategories.ProtectedReadOnlyIntegration} or {TestCategories.ProtectedMutatingIntegration}: {string.Join(", ", unprotectedMethods)}");
        }

        [TestMethod]
        public void MutatingIntegrationTestsAreNotParallelized()
        {
            var missingDoNotParallelize = GetPapiClientIntegrationTestMethods()
                .Where(method => MethodCategories(method).Intersect(MutatingIntegrationCategoryNames).Any())
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingDoNotParallelize.Any(),
                $"Expected mutating integration tests to be marked with {nameof(DoNotParallelizeAttribute)}: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void ReadOnlyIntegrationTestsAreParallelizedUnlessDocumentedException()
        {
            var documentedExceptions = Array.Empty<string>();

            var unexpectedlyNonParallelized = GetPapiClientIntegrationTestMethods()
                .Where(method => !documentedExceptions.Contains(method.Name))
                .Where(method => MethodCategories(method).Intersect(ReadOnlyIntegrationCategoryNames).Any())
                .Where(method => method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                unexpectedlyNonParallelized.Any(),
                $"Expected read-only integration tests not to be marked with {nameof(DoNotParallelizeAttribute)} unless documented in this test: {string.Join(", ", unexpectedlyNonParallelized)}");
        }

        [TestMethod]
        public void KnownIntegrationTestsHaveExpectedCategories()
        {
            AssertMethodsHaveCategory(KnownReadOnlyIntegrationTests, TestCategories.ReadOnlyIntegration);
            AssertMethodsHaveCategory(KnownProtectedReadOnlyIntegrationTests, TestCategories.ProtectedReadOnlyIntegration);
            AssertMethodsHaveCategory(KnownMutatingIntegrationTests, TestCategories.MutatingIntegration);
            AssertMethodsHaveCategory(KnownProtectedMutatingIntegrationTests, TestCategories.ProtectedMutatingIntegration);
        }

        private static IEnumerable<MethodInfo> GetPapiClientIntegrationTestMethods()
        {
            return typeof(IntegrationTestBase).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IntegrationTestBase).IsAssignableFrom(type))
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

            Assert.IsFalse(
                missingCategory.Any(),
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
