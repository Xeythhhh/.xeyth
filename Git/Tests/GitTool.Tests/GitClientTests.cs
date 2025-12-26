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
}
