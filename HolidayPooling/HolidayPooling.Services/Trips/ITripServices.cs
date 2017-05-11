using HolidayPooling.Models.Core;
using HolidayPooling.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Trips
{
    public interface ITripServices : IServices
    {

        void CreateTrip(Trip trip, int userId);

        void Participate(Trip trip, int userId, string userPseudo);

        void Quit(Trip trip, int userId, string userPseudo);

        void DeleteTrip(Trip trip);

        void UpdatePrice(Trip trip, double newAmount);

        void UpdateAllowedNumberOfPeople(Trip trip, int newNumberOfPeople);

        Trip GetTrip(int tripId);

        IEnumerable<TripParticipant> GetParticipants(int tripId);

    }
}
