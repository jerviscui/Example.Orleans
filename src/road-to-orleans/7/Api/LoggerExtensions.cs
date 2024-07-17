namespace Api;

public static partial class LoggerExtensions
{

    #region Constants & Statics

    [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Connection Retry.")]
    public static partial void ConnectionFailed(this ILogger logger);

    [LoggerMessage(EventId = 2000, Level = LogLevel.Error, Message = "SayHelloAsync Canceled: {Message}")]
    public static partial void GrainCanceled(this ILogger logger, string message);

    [LoggerMessage(EventId = 2001, Level = LogLevel.Error, Message = "SayHelloAsync error: {Message}")]
    public static partial void GrainError(this ILogger logger, string message);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Error, Message = "Run error.")]
    public static partial void RunError(this ILogger logger);

    #endregion

}
