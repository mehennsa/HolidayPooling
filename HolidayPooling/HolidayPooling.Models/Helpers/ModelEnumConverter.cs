using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Helpers
{
    public static class ModelEnumConverter
    {

        #region Role

        #region Fields

        private const string Admin = "Admin";
        private const string Common = "Common";

        #endregion

        #region Methods

        public static string RoleToString(Role role)
        {
            var result = string.Empty;
            switch (role)
            {
                case Role.Admin:
                    result = Admin;
                    break;
                case Role.Common:
                    result = Common;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static Role RoleFromString(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return Role.None;
            }

            var result = Role.None;
            switch (role)
            {
                case Admin:
                    result = Role.Admin;
                    break;
                case Common:
                    result = Role.Common;
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion

        #endregion

        #region UserType

        #region Fields

        private const string Business = "Business";
        private const string Customer = "Customer";

        #endregion

        #region Methods

        public static string UserTypeToString(UserType userType)
        {
            var result = string.Empty;

            switch (userType)
            {
                case UserType.Customer:
                    result = Customer;
                    break;
                case UserType.Business:
                    result = Business;
                    break;
            }

            return result;
        }

        public static UserType UserTypeFromString(string userType)
        {

            if (string.IsNullOrEmpty(userType))
            {
                return UserType.None;
            }

            var result = UserType.None;

            switch (userType)
            {
                case Business:
                    result = UserType.Business;
                    break;
                case Customer:
                    result = UserType.Customer;
                    break;
            }

            return result;
        }

        #endregion

        #endregion

        #region PotMode

        #region Fields

        private const string Lead = "Lead";
        private const string Shared = "Shared";

        #endregion

        #region Methods

        public static string PotModeToString(PotMode mode)
        {
            var result = string.Empty;
            switch (mode)
            {
                case PotMode.Lead:
                    result = Lead;
                    break;
                case PotMode.Shared:
                    result = Shared;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static PotMode PotModeFromString(string mode)
        {
            if (string.IsNullOrEmpty(mode))
            {
                return PotMode.None;
            }

            var result = PotMode.None;
            switch (mode)
            {
                case Lead:
                    result = PotMode.Lead;
                    break;
                case Shared:
                    result = PotMode.Shared;
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