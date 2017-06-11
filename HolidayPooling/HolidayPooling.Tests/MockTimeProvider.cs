using HolidayPooling.Infrastructure.TimeProviders;
using System;

namespace HolidayPooling.Tests
{
    public class MockTimeProvider : ITimeProvider
    {

        #region Properties

        private readonly DateTime _now;

        #endregion

        #region .ctor

        public MockTimeProvider(DateTime now)
        {
            _now = now;
        }

        #endregion

        #region ITimeProvider

        public DateTime Now()
        {
            return _now;
        }

        #endregion
    }
}
