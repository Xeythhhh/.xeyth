using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Cli;
using Contracts.Core.Models;
using Contracts.Core.Services;
using Spectre.Console.Testing;
using VerifyTests;
using VerifyXunit;

namespace Contracts.Core.Tests.Contracts.Cli;

public sealed class ValidateCommandRunnerVisualTests
{
    [Fact]
    public async Task Validate_WithMixedResults_RendersPanelsAndSummaryChart()
    {
        var console = new TestConsole();
        console.Profile.Width = 120;
        console.Profile.Capabilities.Interactive = false; // deterministic output

        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        Environment.SetEnvironmentVariable("NO_COLOR", "1");

        try
        {
            using var workspace = new TempWorkspace();
            var options = new ValidateOptions(workspace.Path, null, true);

            var runner = new ValidateCommandRunner(
                console,
                new FakeDiscoveryService(workspace),
                new FakeValidationService());

            await runner.RunAsync(options);

            await Verify(console.Output)
                .UseDirectory("Snapshots");
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }

    private sealed class FakeDiscoveryService : IContractDiscoveryService
    {
        private readonly TempWorkspace _workspace;

        internal FakeDiscoveryService(TempWorkspace workspace)
        {
            _workspace = workspace;
        }

        public Task<IReadOnlyList<ContractMetadata>> DiscoverContractsAsync(string rootPath, CancellationToken cancellationToken = default)
        {
            var contracts = new List<ContractMetadata>
            {
                new()
                {
                    SourcePath = Path.Combine(rootPath, "contracts", "standard.metadata"),
                    Target = new TargetConfiguration
                    {
                        Patterns = new List<string> { "**/*.task" }
                    }
                }
            };

            return Task.FromResult<IReadOnlyList<ContractMetadata>>(contracts);
        }

        public async Task<IReadOnlyList<ContractMetadata>> DiscoverAndMergeContractsAsync(IEnumerable<string> rootPaths, CancellationToken cancellationToken = default)
        {
            var merged = new List<ContractMetadata>();

            foreach (var path in rootPaths)
            {
                var contracts = await DiscoverContractsAsync(path, cancellationToken);
                merged.AddRange(contracts);
            }

            return merged;
        }
    }

    private sealed class FakeValidationService : IContractValidationService
    {
        public Task<ValidationResult> ValidateAsync(string filePath, ContractMetadata contract, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(BuildResult(filePath));
        }

        public Task<ValidationResult> ValidateAsync(
            string filePath,
            IEnumerable<ContractMetadata> contracts,
            string? rootPath = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(BuildResult(filePath));
        }

        public ValidationResult Validate(string filePath, string content, ContractMetadata contract)
        {
            return BuildResult(filePath);
        }

        public ValidationResult Validate(string filePath, string content, IEnumerable<ContractMetadata> contracts, string? rootPath = null)
        {
            return BuildResult(filePath);
        }

        private static ValidationResult BuildResult(string filePath)
        {
            var violations = new List<Violation>();

            if (filePath.EndsWith("warning.task", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(new Violation(
                    Code: "warn-001",
                    Message: "Sample warning",
                    Severity: ViolationSeverity.Warning,
                    FilePath: filePath,
                    LineNumber: 12,
                    Section: "section"));
            }

            if (filePath.EndsWith("error.task", StringComparison.OrdinalIgnoreCase))
            {
                violations.Add(new Violation(
                    Code: "err-001",
                    Message: "Sample error",
                    Severity: ViolationSeverity.Error,
                    FilePath: filePath,
                    LineNumber: 3,
                    Section: "header"));
            }

            return new ValidationResult(filePath, violations);
        }
    }

    private sealed class TempWorkspace : IDisposable
    {
        internal string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "xeyth-contracts", Guid.NewGuid().ToString("N"));

        internal TempWorkspace()
        {
            Directory.CreateDirectory(Path);
            File.WriteAllText(System.IO.Path.Combine(Path, "ok.task"), "# ok");
            File.WriteAllText(System.IO.Path.Combine(Path, "warning.task"), "# warning");
            File.WriteAllText(System.IO.Path.Combine(Path, "error.task"), "# error");
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup
            }
        }
    }
}
