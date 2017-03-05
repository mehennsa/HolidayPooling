using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IUserTripDbImportExport : IDbImportExport<UserTripKey, UserTrip>
    {
        IEnumerable<UserTrip> GetTripForUser(int userId);
    }
}