using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Infrastructure.Converters;
using NUnit.Framework;

namespace HolidayPooling.Infrastructure.Test.Converters
{
    [TestFixture]
    public class TechnicalEnumConverterTest
    {

        #region Fields

        private const string HolidayPoolingDB = "HolidayPooling";

        #endregion

        #region Tests

        [TestCase(HolidayPoolingDatabase.None, "")]
        [TestCase(HolidayPoolingDatabase.HP, HolidayPoolingDB)]
        public void HolidayPoolingDatabaseToString_ShouldReturnCorrectStringValue(HolidayPoolingDatabase hp, string expected)
        {
            Assert.AreEqual(expected, TechnicalEnumConverter.HolidayPoolingDatabaseToString(hp));
        }

        [TestCase("", HolidayPoolingDatabase.None)]
        [TestCase(null, HolidayPoolingDatabase.None)]
        [TestCase(HolidayPoolingDB, HolidayPoolingDatabase.HP)]
        [TestCase("wrong", HolidayPoolingDatabase.None)]
        public void HolidayPoolingFromString_ShouldReturnCorrectEnumValue(string hp, HolidayPoolingDatabase expected)
        {
            Assert.AreEqual(expected, TechnicalEnumConverter.HolidayPoolingDatabaseFromString(hp));
        }

        #endregion

    }
}
