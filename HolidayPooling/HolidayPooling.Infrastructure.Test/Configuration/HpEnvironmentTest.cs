﻿using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Environment;

namespace HolidayPooling.Infrastructure.Test.Configuration
{
    [TestFixture]
    public class HpEnvironmentTest
    {

        #region SetUp & TearDown

        [TearDown]
        public void TearDown()
        {
            ConnectionManager.Clear();
        }

        #endregion

        #region Tests

        [Test]
        public void SetupEnvironment_ShouldInitConnectionString()
        {
            var env = new HpEnvironment();
            env.SetupEnvironment(AppEnvironment.TEST);
            Assert.Greater(ConnectionManager.NumberOfConnections(), 0);
            Assert.IsNotEmpty(ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
            // Logger
            var logger = LoggerManager.GetLogger("ConsoleLogger");
            Assert.IsNotNull(logger);

            // Dispose
            env.Dispose();
            Assert.AreEqual(0, ConnectionManager.NumberOfConnections());
        }

        #endregion

    }
}
