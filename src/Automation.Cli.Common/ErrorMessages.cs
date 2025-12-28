namespace Automation.Cli.Common;

/// <summary>
/// Provides improved error messages with context, suggestions, and helpful guidance.
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Creates an error message for a missing required argument.
    /// </summary>
    public static string MissingRequiredArgument(string argumentName, string? example = null)
    {
        var message = $"Missing required argument: {argumentName}";
        if (!string.IsNullOrWhiteSpace(example))
        {
            message += $"\n\nExample: {example}";
        }
        return message;
    }

    /// <summary>
    /// Creates an error message for an unknown option.
    /// </summary>
    public static string UnknownOption(string option, IEnumerable<string>? validOptions = null)
    {
        var message = $"Unknown option: {option}";
        
        if (validOptions is not null)
        {
            var options = validOptions.ToList();
            if (options.Count > 0)
            {
                message += $"\n\nValid options:\n  " + string.Join("\n  ", options);
            }
        }
        
        message += "\n\nUse --help to see all available options.";
        return message;
    }

    /// <summary>
    /// Creates an error message for an unknown command.
    /// </summary>
    public static string UnknownCommand(string command, IEnumerable<string>? validCommands = null)
    {
        var message = $"Unknown command: {command}";
        
        if (validCommands is not null)
        {
            var commands = validCommands.ToList();
            if (commands.Count > 0)
            {
                message += $"\n\nAvailable commands:\n  " + string.Join("\n  ", commands);
            }
        }
        
        message += "\n\nUse --help to see usage information.";
        return message;
    }

    /// <summary>
    /// Creates an error message for a missing value for an option.
    /// </summary>
    public static string MissingValue(string option, string? expectedValueType = null)
    {
        var message = $"Missing value for {option}";
        
        if (!string.IsNullOrWhiteSpace(expectedValueType))
        {
            message += $"\n\nExpected: {option} <{expectedValueType}>";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for a file not found.
    /// </summary>
    public static string FileNotFound(string filePath, string? suggestion = null)
    {
        var message = $"File not found: {filePath}";
        
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            message += $"\n\nSuggestion: {suggestion}";
        }
        else
        {
            message += "\n\nPlease check that the file path is correct and the file exists.";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for a directory not found.
    /// </summary>
    public static string DirectoryNotFound(string directoryPath, string? suggestion = null)
    {
        var message = $"Directory not found: {directoryPath}";
        
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            message += $"\n\nSuggestion: {suggestion}";
        }
        else
        {
            message += "\n\nPlease check that the directory path is correct and the directory exists.";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for a path that must be within a workspace.
    /// </summary>
    public static string PathMustBeWithinWorkspace(string path, string workspaceRoot, string? suggestion = null)
    {
        var message = $"Path must be within workspace root.\n\nProvided path: {path}\nWorkspace root: {workspaceRoot}";
        
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            message += $"\n\nSuggestion: {suggestion}";
        }
        else
        {
            message += "\n\nPlease provide a path that is within the workspace directory.";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for an invalid value.
    /// </summary>
    public static string InvalidValue(string option, string providedValue, IEnumerable<string>? validValues = null)
    {
        var message = $"Invalid value for {option}: {providedValue}";
        
        if (validValues is not null)
        {
            var values = validValues.ToList();
            if (values.Count > 0)
            {
                message += $"\n\nValid values:\n  " + string.Join("\n  ", values);
            }
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for an invalid JSON file.
    /// </summary>
    public static string InvalidJson(string filePath, string? detail = null)
    {
        var message = $"Invalid JSON file: {filePath}";
        
        if (!string.IsNullOrWhiteSpace(detail))
        {
            message += $"\n\nDetails: {detail}";
        }
        
        message += "\n\nPlease ensure the file contains valid JSON.";
        return message;
    }

    /// <summary>
    /// Creates an error message for a resource that already exists.
    /// </summary>
    public static string AlreadyExists(string resourceType, string path, string? suggestion = null)
    {
        var message = $"{resourceType} already exists: {path}";
        
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            message += $"\n\nSuggestion: {suggestion}";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for a resource that was not found.
    /// </summary>
    public static string NotFound(string resourceType, string identifier, string? suggestion = null)
    {
        var message = $"{resourceType} not found: {identifier}";
        
        if (!string.IsNullOrWhiteSpace(suggestion))
        {
            message += $"\n\nSuggestion: {suggestion}";
        }
        
        return message;
    }

    /// <summary>
    /// Creates an error message for a path that must be absolute.
    /// </summary>
    public static string PathMustBeAbsolute(string path)
    {
        return $"Path must be absolute: {path}\n\nRelative paths are not allowed in this context.";
    }

    /// <summary>
    /// Creates an error message for a required value.
    /// </summary>
    public static string RequiredValue(string parameterName, string? context = null)
    {
        var message = $"{parameterName} is required";
        
        if (!string.IsNullOrWhiteSpace(context))
        {
            message += $"\n\nContext: {context}";
        }
        
        return message;
    }
}
