using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Clc.Polaris.Api.Tests.Integration;

internal sealed class IntegrationCleanup
{
    private readonly List<(string Description, Func<Task> Action)> cleanupActions = new();

    public void Add(string description, Func<Task> action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(action);

        cleanupActions.Add((description, action));
    }

    public async Task RunAsync()
    {
        var failures = new List<Exception>();

        for (var i = cleanupActions.Count - 1; i >= 0; i--)
        {
            var (description, action) = cleanupActions[i];

            try
            {
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                failures.Add(new InvalidOperationException($"Cleanup action failed: {description}", ex));
            }
        }

        if (failures.Count > 0)
        {
            var message = "One or more integration-test cleanup actions failed. Manual cleanup may be required. "
                + string.Join(" ", failures.Select(failure => failure.Message));
            throw new AssertFailedException(message, new AggregateException(failures));
        }
    }
}
