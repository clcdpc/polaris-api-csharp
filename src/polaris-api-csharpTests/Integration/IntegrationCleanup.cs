using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

internal sealed class IntegrationCleanup
{
    private readonly Stack<(string Description, Func<Task> Cleanup)> _cleanupActions = new();

    public void Add(string description, Func<Task> cleanup)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(cleanup);

        _cleanupActions.Push((description, cleanup));
    }

    public async Task RunAsync()
    {
        var failures = new List<Exception>();

        while (_cleanupActions.Count > 0)
        {
            var (description, cleanup) = _cleanupActions.Pop();

            try
            {
                await cleanup().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                failures.Add(new InvalidOperationException($"Cleanup action '{description}' failed.", ex));
            }
        }

        if (failures.Count > 0)
        {
            throw new AssertFailedException(
                $"One or more integration-test cleanup actions failed. Manual cleanup may be required. Failed cleanup action count: {failures.Count}.",
                new AggregateException(failures));
        }
    }
}
