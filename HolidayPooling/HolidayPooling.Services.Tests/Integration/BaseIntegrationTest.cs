using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Database;
using Sams.Commons.Infrastructure.Environment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Tests.Integration
{
    public abstract class BaseIntegrationTest
    {

        #region Properties

        protected abstract IEnumerable<string> TableUsed
        {
            get;
        }

        protected HpEnvironment _env;

        #endregion

        #region Methods

        protected void DeleteTables()
        {
            const string query = "Delete From ";
            foreach(var table in TableUsed)
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP)))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query + table;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _env = new HpEnvironment();
            _env.SetupEnvironment(AppEnvironment.TEST);
            DeleteTables();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            DeleteTables();
            _env.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTables();
        }

        #endregion

    }
}
