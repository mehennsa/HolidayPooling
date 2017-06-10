﻿using HolidayPooling.Infrastructure.TimeProviders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HolidayPooling.Infrastructure.Test.TimeProviders
{
    [TestFixture]
    public class TimeProviderTest
    {

        #region Tests

        [Test]
        public void Now_ShouldReturnCurrentDate()
        {
            var now = new TimeProvider().Now();
            Assert.Greater(now, DateTime.MinValue);
            Assert.Less(now, DateTime.MaxValue);
            // sleep of 10 millisecond
            Thread.Sleep(10);
            Assert.Less(now, DateTime.Now);
        }

        #endregion

    }
}
