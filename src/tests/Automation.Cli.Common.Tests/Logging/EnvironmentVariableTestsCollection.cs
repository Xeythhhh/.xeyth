namespace Automation.Cli.Common.Tests.Logging;

/// <summary>
/// Collection definition to prevent parallel execution of tests that modify environment variables.
/// </summary>
[CollectionDefinition("EnvironmentVariableTests", DisableParallelization = true)]
public class EnvironmentVariableTestsCollection
{
    // This class is never instantiated. It exists only to define the collection.
}
