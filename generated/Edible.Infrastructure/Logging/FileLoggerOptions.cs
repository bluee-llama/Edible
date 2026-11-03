using Microsoft.Extensions.Logging;

namespace Edible.Infrastructure.Logging;

public sealed class FileLoggerOptions
{
    public string FilePath { get; set; } = Path.Combine("logs", "application.log");

    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
}