using System.Diagnostics;
using System.Text;
using GitTool;

namespace GitTool.Tests;

public sealed class GitClientTests
{
    [Fact]
    public void GetConfig_ReturnsValue()
    {
        var runner = new StubRunner(("config --get sample", "value"));
        var client = new GitClient(runner);

        Assert.Equal("value", client.GetConfig("sample"));
    }

    [Fact]
    public void GetRepositoryRoot_ReturnsValue()
    {
        var runner = new StubRunner(("rev-parse --show-toplevel", "/repo"));
        var client = new GitClient(runner);

        Assert.Equal("/repo", client.GetRepositoryRoot());
    }

    [Fact]
    public void GetContextFiles_FiltersNonContext()
    {
        var diffOutput = "a.task\nb.txt\nc.plan\n";
        var runner = new StubRunner(("diff --cached --name-only", diffOutput));
        var client = new GitClient(runner);

        var files = client.GetContextFiles();

        Assert.Equal(new[] { "a.task", "c.plan" }, files);
    }

    private sealed class StubRunner : IGitProcessRunner
    {
        private readonly Dictionary<string, string> _responses;

        internal StubRunner(params (string key, string value)[] responses)
        {
            _responses = responses.ToDictionary(x => x.key, x => x.value);
        }

        public string? Run(params string[] args)
        {
            var key = string.Join(' ', args);
            return _responses.TryGetValue(key, out var value) ? value : null;
        }
    }

    [Fact]
    public void GitProcessRunner_ReturnsNull_WhenProcessFailsToStart()
    {
        var runner = new GitProcessRunner(new NullProcessFactory());

        var result = runner.Run("config", "--get", "sample");

        Assert.Null(result);
    }

    [Fact]
    public void GitProcessRunner_ReturnsNull_OnTimeout()
    {
        var process = new HangingProcess();
        var runner = new GitProcessRunner(new StubProcessFactory(process), timeoutMilliseconds: 10);

        var result = runner.Run("diff", "--cached", "--name-only");

        Assert.Null(result);
        Assert.True(process.Killed);
    }

    [Fact]
    public void GitProcessRunner_UsesOutput_WhenSuccessful()
    {
        var process = new SuccessProcess("value");
        var runner = new GitProcessRunner(new StubProcessFactory(process), timeoutMilliseconds: 100);

        var result = runner.Run("config", "--get", "sample");

        Assert.Equal("value", result);
    }

    private sealed class NullProcessFactory : IProcessFactory
    {
        public IProcess? Start(ProcessStartInfo startInfo) => null;
    }

    private sealed class StubProcessFactory : IProcessFactory
    {
        private readonly IProcess _process;

        internal StubProcessFactory(IProcess process)
        {
            _process = process;
        }

        public IProcess Start(ProcessStartInfo startInfo) => _process;
    }

    private abstract class FakeProcess : IProcess
    {
        public bool Killed { get; protected set; }
        protected StringWriter OutputWriter { get; } = new();

        public StreamReader StandardOutput => new(new MemoryStream(Encoding.UTF8.GetBytes(OutputWriter.ToString())));
        public StreamReader StandardError => new(new MemoryStream());
        public virtual bool WaitForExit(int milliseconds) => true;
        public virtual void Kill(bool entireProcessTree)
        {
            Killed = true;
        }

        public abstract int ExitCode { get; }

        public void Dispose()
        {
        }
    }

    private sealed class HangingProcess : FakeProcess
    {
        public override bool WaitForExit(int milliseconds) => false;
        public override int ExitCode => 1;
    }

    private sealed class SuccessProcess : FakeProcess
    {
        private readonly int _exitCode;

        internal SuccessProcess(string output, int exitCode = 0)
        {
            _exitCode = exitCode;
            OutputWriter.Write(output);
        }

        public override int ExitCode => _exitCode;
    }
}
