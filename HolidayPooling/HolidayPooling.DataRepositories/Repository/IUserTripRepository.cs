using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IUserTripRepository : IRepository
    {

        void SaveUserTrip(UserTrip userTrip);

        void UpdateUserTrip(UserTrip userTrip);

        void DeleteUserTrip(UserTrip userTrip);

        UserTrip GetUserTrip(int userId, string tripName);

        IEnumerable<UserTrip> GetUserTrips(int userId);

        IEnumerable<UserTrip> GetAllUserTrip();

    }
}
