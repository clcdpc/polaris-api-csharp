namespace Clc.Polaris.Api.UnitTests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class UnitTestAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { "Unit" };
    }
}
