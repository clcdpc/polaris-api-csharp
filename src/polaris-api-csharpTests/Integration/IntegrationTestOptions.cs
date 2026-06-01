namespace Clc.Polaris.Api.Tests.Integration;

public sealed class IntegrationTestOptions
{
    public const string SectionName = "IntegrationTestOptions";

    public bool EnableMutatingIntegrationTests { get; set; }
    public bool EnableStaffProtectedTests { get; set; }
    public bool EnableScenarioDependentTests { get; set; }
    public bool EnableAuthenticationFailureTests { get; set; }
}
