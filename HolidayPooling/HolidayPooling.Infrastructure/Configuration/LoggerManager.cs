using log4net;
using Sams.Commons.Infrastructure.Checks;

namespace HolidayPooling.Infrastructure.Configuration
{
    public static class LoggerManager
    {

        public static ILog GetLogger(string loggerName)
        {
            Check.IsNotNullOrEmpty(loggerName, "logger name should be provided");
            return LogManager.GetLogger(loggerName);
        }

    }
}
