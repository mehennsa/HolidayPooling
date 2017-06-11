﻿using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IUserTripDbImportExport : IDbImportExport<UserTripKey, UserTrip>
    {
        IEnumerable<UserTrip> GetTripForUser(int userId);

        IEnumerable<UserTrip> GetUserTripsByTrip(string tripName);
    }
}