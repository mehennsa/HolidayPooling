using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface ITripRepository : IRepository
    {

        void SaveTrip(Trip trip);

        void UpdateTrip(Trip trip);

        void DeleteTrip(Trip trip);

        Trip GetTrip(int tripId);

        Trip GetTrip(string tripName);

        IEnumerable<Trip> GetValidTrips(DateTime validityDate);

        IEnumerable<Trip> GetTripsByDate(DateTime? startDate, DateTime? endDate);

        IEnumerable<Trip> GetAllTrips();

    }
}
