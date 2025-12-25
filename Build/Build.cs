using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target ValidateChatInstructions => _ => _
        .Executes(() =>
        {
            const string arguments = "run --project src/Automation.Framework -- validate-chat-locations";
            ProcessTasks.StartProcess("dotnet", arguments, workingDirectory: RootDirectory)
                .AssertZeroExitCode();
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(ValidateChatInstructions)
        .Executes(() =>
        {
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("dotnet", "test --no-build", workingDirectory: RootDirectory)
                .AssertZeroExitCode();
        });

}
