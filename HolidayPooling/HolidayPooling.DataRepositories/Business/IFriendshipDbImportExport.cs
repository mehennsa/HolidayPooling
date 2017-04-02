
using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IFriendshipDbImportExport : IDbImportExport<FriendshipKey, Friendship>
    {

        IEnumerable<Friendship> GetUserFriendships(int userId);

        IEnumerable<Friendship> GetRequestedFriendships(int userId);

        IEnumerable<Friendship> GetWaitingFriendships(int userId);

    }
}