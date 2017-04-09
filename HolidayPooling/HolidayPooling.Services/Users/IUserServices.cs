using HolidayPooling.Models.Core;
using HolidayPooling.Services.Core;
using System.Collections.Generic;

namespace HolidayPooling.Services.Users
{
    public interface IUserServices : IServices
    {

        void CreateUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);

        User LoginByMail(string mail, string password);

        User LoginByPseudo(string pseudo, string password);

        User GetUserInfo(string pseudo);

        IEnumerable<UserTrip> GetUserTrips(int userId);

        IEnumerable<Friendship> GetUserFriendships(int userId);

    }
}
