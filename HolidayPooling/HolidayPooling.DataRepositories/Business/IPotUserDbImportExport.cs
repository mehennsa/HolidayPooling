using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IPotUserDbImportExport : IDbImportExport<PotUserKey, PotUser>
    {

        IEnumerable<PotUser> GetPotUsers(int potId);

        IEnumerable<PotUser> GetUserPots(int userId);

    }
}
