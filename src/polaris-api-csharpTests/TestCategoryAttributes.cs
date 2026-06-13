namespace Clc.Polaris.Api.Tests
{
    public static class TestCategories
    {
        public const string Unit = "Unit";
        public const string ReadOnlyIntegration = "ReadOnlyIntegration";
        public const string ProtectedReadOnlyIntegration = "ProtectedReadOnlyIntegration";
        public const string MutatingIntegration = "MutatingIntegration";
        public const string ProtectedMutatingIntegration = "ProtectedMutatingIntegration";
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnitTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.Unit };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadOnlyIntegrationTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ReadOnlyIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedReadOnlyIntegrationTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ProtectedReadOnlyIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MutatingIntegrationTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.MutatingIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedMutatingIntegrationTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ProtectedMutatingIntegration };
    }
}
