using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
