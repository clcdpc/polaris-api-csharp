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
        };

        private static readonly string[] KnownProtectedReadOnlyIntegrationTests =
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
            nameof(PapiClientTests.HoldRequestCancelTest),
            nameof(PapiClientTests.HoldRequestCreateTest),
            nameof(PapiClientTests.HoldRequestCreateTest2),
            nameof(PapiClientTests.HoldRequestReactivateTest),
            nameof(PapiClientTests.HoldRequestReplyTest),
            nameof(PapiClientTests.HoldRequestSuspendTest),
            nameof(PapiClientTests.ItemRenewTest),
            nameof(PapiClientTests.TestTitleListCreate_Get_Delete),
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
        };

        private static readonly string[] KnownProtectedMutatingIntegrationTests =
        {
            nameof(PapiClientTests.CreatePatronBlocksTest_FreeTextBlock),
            nameof(PapiClientTests.CreatePatronBlocksTest_SystemBlock),
            nameof(PapiClientTests.CreatePatronBlocksTest_LibraryAssignedBlock),
            nameof(PapiClientTests.NotificationUpdateTest),
            nameof(PapiClientTests.PatronAccountCreateCreditTest),
            nameof(PapiClientTests.PatronAccountDepositCreditTest),
            nameof(PapiClientTests.PatronAccountPayTest),
            nameof(PapiClientTests.PatronAccountPayAllTest),
            nameof(PapiClientTests.PatronAccountRefundCreditTest),
            nameof(PapiClientTests.PatronAccountVoidTest),
            nameof(PapiClientTests.RecordSetContentAddTest),
            nameof(PapiClientTests.RecordSetContentAddTest_List),
            nameof(PapiClientTests.RecordSetContentRemoveTest),
            nameof(PapiClientTests.RecordSetContentRemoveTest_List),
        };

        [TestMethod]
        public void LivePapiClientTestsHaveExactlyOneIntegrationCategory()
        {
            var incorrectlyCategorized = GetPapiClientTestMethods()
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
                $"Expected every live {nameof(PapiClientTests)} method to have exactly one integration category ({string.Join(", ", IntegrationCategoryNames)}): {string.Join(", ", incorrectlyCategorized)}");
        }

        [TestMethod]
        public void PapiClientTestsDoNotUseLegacyIntegrationCategories()
        {
            var legacyCategories = new[]
            {
                LegacyIntegrationCategory,
                LegacyProtectedIntegrationCategory,
            };

            var methodsWithLegacyCategories = GetPapiClientTestMethods()
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
                $"Expected no {nameof(PapiClientTests)} method to use legacy raw integration category traits: {string.Join(", ", methodsWithLegacyCategories)}");
        }

        [TestMethod]
        public void StaffOverrideIntegrationTestsHaveProtectedIntegrationCategory()
        {
            var unprotectedMethods = PapiClientTestSourceMethods()
                .Where(method => method.Value.Contains("IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings)"))
                .Select(method => GetPapiClientTestMethod(method.Key))
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
            var missingDoNotParallelize = GetPapiClientTestMethods()
                .Where(method => MethodCategories(method).Intersect(MutatingIntegrationCategoryNames).Any())
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingDoNotParallelize.Any(),
                $"Expected mutating integration tests to be marked with {nameof(DoNotParallelizeAttribute)} unless a documented exception is added to this test: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void ReadOnlyIntegrationTestsAreParallelizedUnlessDocumentedException()
        {
            var documentedExceptions = Array.Empty<string>();

            var unexpectedlyNonParallelized = GetPapiClientTestMethods()
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

        private static IEnumerable<MethodInfo> GetPapiClientTestMethods()
        {
            return typeof(PapiClientTests)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any());
        }

        private static MethodInfo GetPapiClientTestMethod(string methodName)
        {
            var method = typeof(PapiClientTests).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Expected to find public test method {nameof(PapiClientTests)}.{methodName}.");

            return method;
        }

        private static void AssertMethodsHaveCategory(IEnumerable<string> methodNames, string expectedCategory)
        {
            var missingCategory = methodNames
                .Where(methodName => !MethodCategories(GetPapiClientTestMethod(methodName)).Contains(expectedCategory))
                .ToArray();

            Assert.IsFalse(
                missingCategory.Any(),
                $"Expected these {nameof(PapiClientTests)} methods to be marked with {expectedCategory}: {string.Join(", ", missingCategory)}");
        }

        private static IEnumerable<string> MethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryBaseAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories);
        }

        private static Dictionary<string, string> PapiClientTestSourceMethods([CallerFilePath] string categoryTestSourcePath = "")
        {
            var testSourcePath = Path.Combine(Path.GetDirectoryName(categoryTestSourcePath)!, "PapiClientTests.cs");
            var source = File.ReadAllText(testSourcePath);
            var sourceMethods = new Dictionary<string, string>();
            var methodMatches = Regex.Matches(
                source,
                @"public\s+(?:async\s+)?(?:Task|void)\s+(?<name>\w+)\s*\([^)]*\)\s*\{",
                RegexOptions.CultureInvariant);

            foreach (Match methodMatch in methodMatches)
            {
                var openingBraceIndex = source.IndexOf('{', methodMatch.Index + methodMatch.Length - 1);
                Assert.AreNotEqual(-1, openingBraceIndex, $"Expected to find opening brace for {methodMatch.Groups["name"].Value}.");

                var closingBraceIndex = FindClosingBrace(source, openingBraceIndex);
                sourceMethods[methodMatch.Groups["name"].Value] = source[methodMatch.Index..(closingBraceIndex + 1)];
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
