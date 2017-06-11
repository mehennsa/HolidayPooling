using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
using NUnit.Framework;

namespace HolidayPooling.Models.Tests.Helpers
{
    [TestFixture]
    public class ModelEnumConverterTest
    {

        #region RoleEnum

        #region Fields

        private const string Admin = "Admin";
        private const string Common = "Common";

        #endregion

        #region Tests

        [TestCase(null, Role.None)]
        [TestCase("", Role.None)]
        [TestCase("wrong", Role.None)]
        [TestCase(Admin, Role.Admin)]
        [TestCase(Common, Role.Common)]
        public void RoleEnumFromString_ShouldReturnCorrectEnumValue(string role, Role expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.RoleFromString(role));
        }

        [TestCase(Role.Admin, Admin)]
        [TestCase(Role.Common, Common)]
        [TestCase(Role.None, "")]
        public void RoleEnumToString_ShouldReturnCorrectStringValue(Role role, string expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.RoleToString(role));
        }

        #endregion

        #endregion

        #region PotMode

        #region Fields

        private const string Lead = "Lead";
        private const string Shared = "Shared";


        #endregion

        #region Tests

        [TestCase(null, PotMode.None)]
        [TestCase("", PotMode.None)]
        [TestCase("Wrong", PotMode.None)]
        [TestCase(Lead, PotMode.Lead)]
        [TestCase(Shared, PotMode.Shared)]
        public void PotModeFromString_ShouldReturnCorrectEnumValue(string mode, PotMode expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.PotModeFromString(mode));
        }

        [TestCase(PotMode.Lead, Lead)]
        [TestCase(PotMode.Shared, Shared)]
        [TestCase(PotMode.None, "")]
        public void PotModeToString_ShouldReturnCorrectStringValue(PotMode mode, string expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.PotModeToString(mode));
        }

        #endregion

        #endregion

        #region UserType


        #region Fields

        private const string Business = "Business";
        private const string Customer = "Customer";

        #endregion

        #region Tests
        
        [TestCase(null, UserType.None)]
        [TestCase("", UserType.None)]
        [TestCase("Wrong", UserType.None)]
        [TestCase(Business, UserType.Business)]
        [TestCase(Customer, UserType.Customer)]
        public void UserTypeFromString_ShouldReturnCorrectEnumValue(string type, UserType expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.UserTypeFromString(type));
        }

        [TestCase(UserType.Business, Business)]
        [TestCase(UserType.Customer, Customer)]
        [TestCase(UserType.None, "")]
        public void UserTypeToString_ShouldReturnCorrectStringValue(UserType type, string expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.UserTypeToString(type));
        }
        
        #endregion

        #endregion

    }
}
