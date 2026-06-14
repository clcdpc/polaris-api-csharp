using System.Reflection;

namespace Clc.Polaris.Api.LiveIntegrationTests.Infrastructure
{
    [TestClass]
    public class LiveTestCategoryGuardTests
    {
        private static readonly string[] PrimaryLiveCategories =
        [
            LiveTestCategories.ReadOnly,
            LiveTestCategories.Mutating,
        ];

        [TestMethod]
        [ReadOnlyLiveTest]
        public void IntegrationTestBaseTests_HaveExactlyOnePrimaryLiveCategory()
        {
            var uncategorizedOrAmbiguousTests = typeof(IntegrationTestBase).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IntegrationTestBase).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                .Where(method => method.GetCustomAttributes<TestMethodAttribute>(inherit: false).Any())
                .Select(method => new
                {
                    MethodName = $"{method.DeclaringType!.FullName}.{method.Name}",
                    PrimaryCategories = MethodCategories(method).Intersect(PrimaryLiveCategories).ToArray(),
                })
                .Where(method => method.PrimaryCategories.Length != 1)
                .Select(method => $"{method.MethodName} ({method.PrimaryCategories.Length}: {string.Join(", ", method.PrimaryCategories)})")
                .OrderBy(methodName => methodName)
                .ToArray();

            Assert.IsEmpty(
                uncategorizedOrAmbiguousTests,
                $"Expected each live integration test to have exactly one primary live category ({string.Join(", ", PrimaryLiveCategories)}): {string.Join(", ", uncategorizedOrAmbiguousTests)}");
        }

        private static IEnumerable<string> MethodCategories(MethodInfo method)
        {
            return method
                .GetCustomAttributes<TestCategoryBaseAttribute>(inherit: false)
                .SelectMany(attribute => attribute.TestCategories);
        }
    }
}
