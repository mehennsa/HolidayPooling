using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IPotUserRepository : IRepository
    {

        void SavePotUser(PotUser potUser);

        void UpdatePotUser(PotUser potUser);

        void DeletePotUser(PotUser potUser);

        PotUser GetPotUser(int potId, int userId);

        IEnumerable<PotUser> GetPotUsers(int potId);

        IEnumerable<PotUser> GetUserPots(int userId);

        IEnumerable<PotUser> GetAllPotUser();

    }
}
