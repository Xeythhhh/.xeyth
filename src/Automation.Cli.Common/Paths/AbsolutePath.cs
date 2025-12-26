using System.Runtime.InteropServices;

namespace Xeyth.Common.IO.Paths;

/// <summary>
/// Normalizes and validates absolute filesystem paths and provides helpers for workspace-bound checks.
/// </summary>
public readonly record struct AbsolutePath
{
    private readonly string _value;

    private AbsolutePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Path is required", nameof(value));
        }

        var full = Path.GetFullPath(value);
        if (!Path.IsPathRooted(full))
        {
            throw new ArgumentException("Path must be absolute", nameof(value));
        }

        _value = NormalizeSeparators(full);
    }

    public string Value => _value;

    public static AbsolutePath From(string path) => new(path);

    public AbsolutePath Combine(string relative)
    {
        if (string.IsNullOrWhiteSpace(relative))
        {
            throw new ArgumentException("Relative path is required", nameof(relative));
        }

        var combined = Path.Combine(_value, relative);
        return new AbsolutePath(combined);
    }

    public bool IsUnder(AbsolutePath root)
    {
        var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var rootTrimmed = TrimTrailingSeparators(root._value);
        var currentTrimmed = TrimTrailingSeparators(_value);

        if (string.Equals(rootTrimmed, currentTrimmed, comparison))
        {
            return true;
        }

        return currentTrimmed.StartsWith(rootTrimmed + Path.DirectorySeparatorChar, comparison);
    }

    public override string ToString() => _value;

    private static string NormalizeSeparators(string path) =>
        path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

    private static string TrimTrailingSeparators(string path) =>
        path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}
