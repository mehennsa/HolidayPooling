using HolidayPooling.Infrastructure.Configuration;

namespace HolidayPooling.Infrastructure.Converters
{
    public static class TechnicalEnumConverter
    {

        #region HolidayPoolingDatabase

        #region Fields

        private const string HolidayPoolingDB = "HolidayPooling";

        #endregion

        #region Methods

        public static string HolidayPoolingDatabaseToString(HolidayPoolingDatabase db)
        {
            var result = string.Empty;
            switch (db)
            {
                case HolidayPoolingDatabase.HP:
                    result = HolidayPoolingDB;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static HolidayPoolingDatabase HolidayPoolingDatabaseFromString(string db)
        {
            if (string.IsNullOrEmpty(db))
            {
                return HolidayPoolingDatabase.None;
            }

            var result = HolidayPoolingDatabase.None;
            switch (db)
            {
                case HolidayPoolingDB:
                    result = HolidayPoolingDatabase.HP;
                    break;
                default:
                    break;
            }

            return result;

        }

        #endregion

        #endregion

    }
}