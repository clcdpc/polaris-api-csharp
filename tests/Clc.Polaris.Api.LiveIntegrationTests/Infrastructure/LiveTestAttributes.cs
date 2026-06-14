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
        public override IList<string> TestCategories => new[] { LiveTestCategories.ReadOnly, LiveTestCategories.RequiresStaffOverride };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Mutating, LiveTestCategories.RequiresDisposableData };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffMutatingLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Mutating, LiveTestCategories.RequiresStaffOverride, LiveTestCategories.RequiresDisposableData };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class LifecycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Mutating, LiveTestCategories.RequiresDisposableData, LiveTestCategories.Lifecycle };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffLifecycleLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.Mutating, LiveTestCategories.RequiresStaffOverride, LiveTestCategories.RequiresDisposableData, LiveTestCategories.Lifecycle };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SmokeLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.ReadOnly, LiveTestCategories.Smoke };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class StaffSmokeLiveTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { LiveTestCategories.ReadOnly, LiveTestCategories.RequiresStaffOverride, LiveTestCategories.Smoke };
    }
}
