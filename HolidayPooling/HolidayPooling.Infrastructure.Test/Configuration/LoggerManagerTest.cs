using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;
using System;

namespace HolidayPooling.Infrastructure.Test.Configuration
{
    [TestFixture]
    public class LoggerManagerTest
    {

        #region Tests

        [TestCase("")]
        [TestCase(null)]
        public void GetLogger_WhenNameIsNullOrEmpty_ShouldThrowArgumentNullException(string name)
        {
            Assert.Throws<ArgumentNullException>(() => LoggerManager.GetLogger(name));
        }


        [Test]
        public void GetLogger_ShouldReturnLogger()
        {
            Assert.IsNotNull(LoggerManager.GetLogger("ConsoleLogger"));
        }

        #endregion

    }
}
