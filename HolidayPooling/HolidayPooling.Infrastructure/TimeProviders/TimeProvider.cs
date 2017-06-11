using System;

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
