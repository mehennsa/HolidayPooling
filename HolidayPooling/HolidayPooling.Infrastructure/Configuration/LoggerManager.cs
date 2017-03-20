using log4net;
using Sams.Commons.Infrastructure.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
