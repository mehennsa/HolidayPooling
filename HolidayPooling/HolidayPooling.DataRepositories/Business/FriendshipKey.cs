using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class FriendshipKey
    {

        #region Properties

        public int UserId { get; private set; }

        public string FriendName { get; private set; }

        #endregion

        #region .ctor

        public FriendshipKey(int userId, string friendName)
        {
            UserId = userId;
            FriendName = friendName;
        }

        #endregion

    }
}