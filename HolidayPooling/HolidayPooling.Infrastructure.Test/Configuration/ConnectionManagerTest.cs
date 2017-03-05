using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Infrastructure.Test.Configuration
{
    [TestFixture]
    public class ConnectionManagerTest
    {

        #region SetUp & TearDown

        [SetUp]
        public void Setup()
        {
            ConnectionManager.Clear();
        }

        #endregion

        #region Tests

        [TestCase(null)]
        [TestCase("")]
        public void AddConnectionString_WhenConnectionStringIsNullOrEmpty_ShouldThrowArgumentNullException(string connection)
        {
            Assert.Throws<ArgumentNullException>(() => ConnectionManager.AddConnection(HolidayPoolingDatabase.HP, connection));
        }

        [Test]
        public void AddConnectionString_WhenDbIsNone_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ConnectionManager.AddConnection(HolidayPoolingDatabase.None, "connection"));
        }

        [Test]
        public void AddConnectionString_WhenNotExist_ShouldAddIt()
        {
            var connectionString = "createConnectionString";
            ConnectionManager.AddConnection(HolidayPoolingDatabase.HP, connectionString);
            Assert.AreEqual(1, ConnectionManager.NumberOfConnections());
            Assert.AreEqual(connectionString, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
        }

        [Test]
        public void AddConnectionString_WhenExist_ShouldUpdate()
        {
            var connectionString = "createConnectionString";
            ConnectionManager.AddConnection(HolidayPoolingDatabase.HP, connectionString);
            Assert.AreEqual(1, ConnectionManager.NumberOfConnections());
            Assert.AreEqual(connectionString, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
            var update = "updateConnectionString";
            ConnectionManager.AddConnection(HolidayPoolingDatabase.HP, update);
            Assert.AreEqual(1, ConnectionManager.NumberOfConnections());
            Assert.AreEqual(update, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
        }

        [Test]
        public void GetConnectionString_WhenNotExist_ShouldThrowConfigurationException()
        {
            Assert.Throws<ConfigurationException>(() => ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
        }

        [Test]
        public void GetConnectionString_WhenDbIsNone_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ConnectionManager.GetConnectionString(HolidayPoolingDatabase.None));
        }

        [Test]
        public void Clear_ShouldRemoveAllConnections()
        {
            var connectionString = "createConnectionString";
            ConnectionManager.AddConnection(HolidayPoolingDatabase.HP, connectionString);
            Assert.AreEqual(1, ConnectionManager.NumberOfConnections());
            Assert.AreEqual(connectionString, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP));
            ConnectionManager.Clear();
            Assert.AreEqual(0, ConnectionManager.NumberOfConnections());
        }

        #endregion

    }
}
