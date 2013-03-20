using NLog;

namespace NuBot
{
    public class RobotLog : IRobotLog
    {
        private readonly Logger _logger;

        public RobotLog(Logger logger)
        {
            _logger = logger;
        }

        public void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }
    }
}
