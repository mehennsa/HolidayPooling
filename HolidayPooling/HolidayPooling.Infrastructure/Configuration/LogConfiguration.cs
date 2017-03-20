using Sams.Commons.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using System.IO;

namespace HolidayPooling.Infrastructure.Configuration
{
    public class LogConfiguration : BaseConfigurator
    {
        #region BaseConfigurator

        protected override void DoConfigure(string filePath)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(filePath));
        }

        #endregion
    }
}
