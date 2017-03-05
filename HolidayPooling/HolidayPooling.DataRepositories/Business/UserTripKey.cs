using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class UserTripKey
    {

        #region Properties

        public int UserId { get; private set; }

        public string TripName { get; private set; }

        #endregion

        #region .ctor

        public UserTripKey(int userId, string tripName)
        {
            UserId = userId;
            TripName = tripName;
        }

        #endregion

    }
}