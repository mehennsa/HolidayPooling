using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [TestCase(null, RoleEnum.None)]
        [TestCase("", RoleEnum.None)]
        [TestCase("wrong", RoleEnum.None)]
        [TestCase(Admin, RoleEnum.Admin)]
        [TestCase(Common, RoleEnum.Common)]
        public void RoleEnumFromString_ShouldReturnCorrectEnumValue(string role, RoleEnum expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.RoleEnumFromString(role));
        }

        [TestCase(RoleEnum.Admin, Admin)]
        [TestCase(RoleEnum.Common, Common)]
        [TestCase(RoleEnum.None, "")]
        public void RoleEnumToString_ShouldReturnCorrectStringValue(RoleEnum role, string expected)
        {
            Assert.AreEqual(expected, ModelEnumConverter.RoleEnumToString(role));
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
