using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IUserTripRepository : IRepository
    {

        void SaveUserTrip(UserTrip userTrip);

        void UpdateUserTrip(UserTrip userTrip);

        void DeleteUserTrip(UserTrip userTrip);

        UserTrip GetUserTrip(int userId, string tripName);

        IEnumerable<UserTrip> GetUserTrips(int userId);

        IEnumerable<UserTrip> GetUserTripsByTrip(string tripName);

        IEnumerable<UserTrip> GetAllUserTrip();

    }
}
