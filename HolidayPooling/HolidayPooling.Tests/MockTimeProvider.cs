using HolidayPooling.Infrastructure.TimeProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
