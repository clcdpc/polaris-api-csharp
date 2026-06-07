using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
    public sealed class UnitCategoryAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.Unit };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadOnlyIntegrationCategoryAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ReadOnlyIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedReadOnlyIntegrationCategoryAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ProtectedReadOnlyIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MutatingIntegrationCategoryAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.MutatingIntegration };
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProtectedMutatingIntegrationCategoryAttribute : TestCategoryBaseAttribute
    {
        public override IList<string> TestCategories => new[] { global::Clc.Polaris.Api.Tests.TestCategories.ProtectedMutatingIntegration };
    }
}
