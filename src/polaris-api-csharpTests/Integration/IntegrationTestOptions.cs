namespace Clc.Polaris.Api;

public sealed class IntegrationTestOptions
{
    public bool EnableMutatingIntegrationTests { get; set; }
    public bool EnableStaffProtectedTests { get; set; }
    public bool EnableScenarioDependentTests { get; set; }
    public bool EnableAuthenticationFailureTests { get; set; }
}
