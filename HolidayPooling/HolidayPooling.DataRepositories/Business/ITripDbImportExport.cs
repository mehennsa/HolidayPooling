using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Business
{
    public interface ITripDbImportExport : IDbImportExport<int, Trip>
    {

        bool IsTripNameUsed(string name);

        Trip GetTripByName(string name);

        IEnumerable<Trip> GetValidTrips(DateTime validityDate);

        IEnumerable<Trip> GetTripBetweenStartDateAndEndDate(DateTime? startDate, DateTime? endDate);

    }
}
