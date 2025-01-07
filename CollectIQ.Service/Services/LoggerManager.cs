using CollectIQ.Service.Interfaces;
using NLog;


namespace CollectIQ.Service.Services
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger _logger;

        public LoggerManager(ILogger logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public void LogWarn(string message)
        {
            _logger.Warn(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }
    }
}
