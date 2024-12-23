using Microsoft.Extensions.Logging;

namespace MyNamespace;

public static partial class LoggerExtensions
{

    #region Constants & Statics

    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Connection Retry.")]
    public static partial void ConnectionFailed(this ILogger logger);

    #endregion

}