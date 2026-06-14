namespace Clc.Polaris.Api.LiveIntegrationTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadOnlyLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.ReadOnly };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffReadOnlyLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.StaffReadOnly };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Mutating };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffMutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.StaffMutating };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class LifecycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Lifecycle };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffLifecycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.StaffLifecycle };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GovernanceTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Governance };
    }
}
