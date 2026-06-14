namespace Clc.Polaris.Api.LiveIntegrationTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadOnlyLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.ReadOnly];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedReadOnlyLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.ProtectedReadOnly];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.Mutating];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedMutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.ProtectedMutating];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class LifecycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.Lifecycle];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedLifeCycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.ProtectedLifeCycle];
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GovernanceTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => [LiveTestCategories.Governance];
    }
}
