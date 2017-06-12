
using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.ImportExport
{
    public interface IFriendshipDbImportExport : IDbImportExport<FriendshipKey, Friendship>
    {

        IEnumerable<Friendship> GetUserFriendships(int userId);

        IEnumerable<Friendship> GetRequestedFriendships(int userId);

        IEnumerable<Friendship> GetWaitingFriendships(int userId);

    }
}