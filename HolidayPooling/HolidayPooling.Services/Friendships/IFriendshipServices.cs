using HolidayPooling.Models.Core;
using HolidayPooling.Services.Core;
using System.Collections.Generic;

namespace HolidayPooling.Services.Friendships
{
    public interface IFriendshipServices : IServices
    {

        void RequestFriendship(Friendship friendship, string userPseudo);

        void DenyFriendship(Friendship friendship, string userPseudo);

        void AcceptFriendship(Friendship friendship, string userPseudo);

        IEnumerable<Friendship> GetUserFriendships(int userId);

        IEnumerable<Friendship> GetRequestedFriendships(int userId);

        IEnumerable<Friendship> GetWaitingFriendships(int userId);

        IEnumerable<Friendship> GetUserFriendships(string pseudo);

        Friendship GetFriendship(int userId, string friendName);

    }
}
