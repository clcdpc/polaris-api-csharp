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

        private static readonly string[] IntegrationCategories =
        {
            TestCategories.ReadOnlyIntegration,
            TestCategories.ProtectedReadOnlyIntegration,
            TestCategories.MutatingIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] ProtectedIntegrationCategories =
        {
            TestCategories.ProtectedReadOnlyIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] MutatingIntegrationCategories =
        {
            TestCategories.MutatingIntegration,
            TestCategories.ProtectedMutatingIntegration,
        };

        private static readonly string[] ReadOnlyIntegrationCategories =
        {
            TestCategories.ReadOnlyIntegration,
            TestCategories.ProtectedReadOnlyIntegration,
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
        public void LivePapiClientTestsHaveExactlyOneIntegrationCategoryOrDocumentedException()
        {
            var documentedExceptions = new[]
            {
                // This test currently only documents the not-yet-implemented method and does not issue a live request.
                nameof(PapiClientTests.HeadingsSearchTest),
            };

            var incorrectlyClassified = GetPapiClientTestMethods()
                .Where(method => !documentedExceptions.Contains(method.Name))
                .Select(method => new
                {
                    Method = method,
                    Categories = MethodCategories(method).Where(IntegrationCategories.Contains).ToArray(),
                })
                .Where(method => method.Categories.Length != 1)
                .Select(method => $"{method.Method.Name} ({string.Join(", ", method.Categories.DefaultIfEmpty("no integration category"))})")
                .ToArray();

            Assert.IsFalse(
                incorrectlyClassified.Any(),
                $"Expected every live {nameof(PapiClientTests)} method to have exactly one integration category: {string.Join(", ", incorrectlyClassified)}");
        }

        [TestMethod]
        public void PapiClientTestsDoNotUseLegacyIntegrationCategories()
        {
            var legacyCategoryMethods = GetPapiClientTestMethods()
                .Select(method => new
                {
                    Method = method,
                    Categories = MethodCategories(method)
                        .Where(category => category is LegacyIntegrationCategory or LegacyProtectedIntegrationCategory)
                        .ToArray(),
                })
                .Where(method => method.Categories.Any())
                .Select(method => $"{method.Method.Name} ({string.Join(", ", method.Categories)})")
                .ToArray();

            Assert.IsFalse(
                legacyCategoryMethods.Any(),
                $"Expected no {nameof(PapiClientTests)} methods to use legacy raw integration categories: {string.Join(", ", legacyCategoryMethods)}");
        }

        [TestMethod]
        public void StaffOverrideIntegrationTestsHaveProtectedIntegrationCategory()
        {
            var methodsRequiringStaffOverride = PapiClientTestSourceMethods()
                .Where(method => method.Value.Contains("IntegrationTestRequirements.RequireStaffOverrideAccount(PapiSettings)"))
                .Select(method => method.Key)
                .ToArray();

            var missingProtectedCategory = methodsRequiringStaffOverride
                .Where(methodName => !MethodHasAnyCategory(methodName, ProtectedIntegrationCategories))
                .ToArray();

            Assert.IsFalse(
                missingProtectedCategory.Any(),
                $"Expected tests that call {nameof(IntegrationTestRequirements.RequireStaffOverrideAccount)} to use either {TestCategories.ProtectedReadOnlyIntegration} or {TestCategories.ProtectedMutatingIntegration}: {string.Join(", ", missingProtectedCategory)}");
        }

        [TestMethod]
        public void MutatingIntegrationTestsAreNotParallelized()
        {
            var missingDoNotParallelize = GetPapiClientTestMethods()
                .Where(method => MethodHasAnyCategory(method, MutatingIntegrationCategories))
                .Where(method => !method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                missingDoNotParallelize.Any(),
                $"Expected mutating integration tests to be marked with {nameof(DoNotParallelizeAttribute)}: {string.Join(", ", missingDoNotParallelize)}");
        }

        [TestMethod]
        public void ReadOnlyIntegrationTestsAreParallelizableUnlessDocumented()
        {
            var documentedDoNotParallelizeReadOnlyTests = Array.Empty<string>();

            var unexpectedlyNonParallelized = GetPapiClientTestMethods()
                .Where(method => !documentedDoNotParallelizeReadOnlyTests.Contains(method.Name))
                .Where(method => MethodHasAnyCategory(method, ReadOnlyIntegrationCategories))
                .Where(method => method.GetCustomAttributes<DoNotParallelizeAttribute>(inherit: false).Any())
                .Select(method => method.Name)
                .ToArray();

            Assert.IsFalse(
                unexpectedlyNonParallelized.Any(),
                $"Expected read-only integration tests not to be marked with {nameof(DoNotParallelizeAttribute)} unless documented in {nameof(documentedDoNotParallelizeReadOnlyTests)}: {string.Join(", ", unexpectedlyNonParallelized)}");
        }

        [TestMethod]
        public void KnownMutatingTestsHaveMutatingIntegrationCategory()
        {
            var missingCategory = KnownMutatingIntegrationTests
                .Where(methodName => !MethodHasAnyCategory(methodName, MutatingIntegrationCategories))
                .ToArray();

            Assert.IsFalse(
                missingCategory.Any(),
                $"Expected known mutating tests to use either {TestCategories.MutatingIntegration} or {TestCategories.ProtectedMutatingIntegration}: {string.Join(", ", missingCategory)}");
        }

        private static IEnumerable<MethodInfo> GetPapiClientTestMethods()
        {
            return typeof(PapiClientTests)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any());
        }

        private static bool MethodHasAnyCategory(string methodName, IEnumerable<string> expectedCategories)
        {
            var method = typeof(PapiClientTests).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(method, $"Expected to find public test method {nameof(PapiClientTests)}.{methodName}.");

            return MethodHasAnyCategory(method, expectedCategories);
        }

        private static bool MethodHasAnyCategory(MethodInfo method, IEnumerable<string> expectedCategories)
        {
            return MethodCategories(method).Intersect(expectedCategories).Any();
        }

        private static string[] MethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryBaseAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories)
                .ToArray();
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
