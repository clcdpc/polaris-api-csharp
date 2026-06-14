using System.Reflection;

namespace Clc.Polaris.Api.LiveIntegrationTests.Infrastructure
{
    [TestClass]
    public class LiveTestCategoryGuardTests
    {
        private static readonly string[] LiveCategories =
        [
            LiveTestCategories.ReadOnly,
            LiveTestCategories.StaffReadOnly,
            LiveTestCategories.Mutating,
            LiveTestCategories.StaffMutating,
            LiveTestCategories.Lifecycle,
            LiveTestCategories.StaffLifecycle,
        ];

        [TestMethod]
        [GovernanceTest]
        public void IntegrationTestBaseTests_HaveExactlyOneLiveCategory()
        {
            var uncategorizedOrAmbiguousTests = typeof(IntegrationTestBase).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IntegrationTestBase).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any())
                .Select(method => new
                {
                    MethodName = $"{method.DeclaringType!.FullName}.{method.Name}",
                    Categories = MethodCategories(method).Intersect(LiveCategories).ToArray(),
                })
                .Where(method => method.Categories.Length != 1)
                .Select(method => $"{method.MethodName} ({method.Categories.Length}: {string.Join(", ", method.Categories)})")
                .OrderBy(methodName => methodName)
                .ToArray();

            Assert.IsEmpty(
                uncategorizedOrAmbiguousTests,
                $"Expected each live integration test to have exactly one live category ({string.Join(", ", LiveCategories)}): {string.Join(", ", uncategorizedOrAmbiguousTests)}");
        }

        private static IEnumerable<string> MethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryBaseAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories);
        }
    }
}
