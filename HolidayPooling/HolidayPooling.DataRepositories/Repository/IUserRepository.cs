using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IUserRepository : IRepository
    {

        void SaveUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);

        IEnumerable<User> GetUsers();

        User GetUser(int userId);

        User GetUserByMail(string mail, string password);

        User GetUserByPseudo(string pseudo, string password);

        User GetUserInfo(string pseudo);

    }
}
