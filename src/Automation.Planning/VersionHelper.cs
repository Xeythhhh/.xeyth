using System.Reflection;

namespace Automation.Planning;

internal static class VersionHelper
{
    internal static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            var buildMetadataIndex = informational.IndexOf('+');
            return buildMetadataIndex > 0 ? informational[..buildMetadataIndex] : informational;
        }

        return assembly.GetName().Version?.ToString(3) ?? "1.0.0";
    }
}
