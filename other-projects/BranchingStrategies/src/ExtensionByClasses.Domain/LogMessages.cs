using Microsoft.Extensions.Logging;

namespace ExtensionByClasses.Domain;

public static partial class LogMessages
{
    [LoggerMessage(LogLevel.Information, Message = "Adding {Formatter} for type {FormattingType}")]
    public static partial void LogEmployeeFormatterAdded(this ILogger logger, Type formatter, string formattingType);   
}