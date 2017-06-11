using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Configuration;
using System.Collections.Generic;

namespace HolidayPooling.Infrastructure.Configuration
{
    public static class ConnectionManager
    {

        #region Fields

        #region Database

        private static IDictionary<HolidayPoolingDatabase, string> _connectionStrings = new Dictionary<HolidayPoolingDatabase, string>();

        #endregion

        #endregion

        #region Methods

        #region Database

        private static void CheckHolidayPoolingDatabaseIsNotNone(HolidayPoolingDatabase value, string parameterName)
        {
            Check.That<HolidayPoolingDatabase>(value, b => b != HolidayPoolingDatabase.None, parameterName);
        }

        public static void AddConnection(HolidayPoolingDatabase db, string connectionString)
        {
            Check.IsNotNullOrEmpty(connectionString, "connectionString");
            CheckHolidayPoolingDatabaseIsNotNone(db, "db");
            if (!_connectionStrings.ContainsKey(db))
            {
                _connectionStrings.Add(db, connectionString);
            }
            else
            {
                _connectionStrings[db] = connectionString;
            }
        }

        public static string GetConnectionString(HolidayPoolingDatabase db)
        {
            CheckHolidayPoolingDatabaseIsNotNone(db, "db");
            if (!_connectionStrings.ContainsKey(db))
            {
                throw new ConfigurationException("Invalid Database Name: " + db.ToString());
            }

            return _connectionStrings[db];
        }

        public static void Clear()
        {
            _connectionStrings.Clear();
        }

        public static int NumberOfConnections()
        {
            return _connectionStrings.Keys.Count;
        }

        #endregion

        #endregion

    }
}