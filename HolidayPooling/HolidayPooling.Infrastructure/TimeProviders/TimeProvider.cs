using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Infrastructure.TimeProviders
{
    public sealed class TimeProvider : ITimeProvider
    {

        #region TimeProvider

        public DateTime Now()
        {
            return DateTime.Now;
        }

        #endregion
    }
}
