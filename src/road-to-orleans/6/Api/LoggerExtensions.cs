namespace Api;

public static partial class LoggerExtensions
{

    #region Constants & Statics

    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Connection Retry.")]
    public static partial void ConnectionFailed(this ILogger logger);

    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Run error.")]
    public static partial void RunError(this ILogger logger);

    #endregion

}
