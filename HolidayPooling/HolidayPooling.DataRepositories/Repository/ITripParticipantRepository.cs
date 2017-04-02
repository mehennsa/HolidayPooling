using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface ITripParticipantRepository : IRepository
    {

        void SaveTripParticipant(TripParticipant tripParticipant);

        void UpdateTripParticipant(TripParticipant tripParticipant);

        void DeleteTripParticipant(TripParticipant tripParticipant);

        TripParticipant GetTripParticipant(int tripId, string userPseudo);

        IEnumerable<TripParticipant> GetTripParticipants(int tripId);

        IEnumerable<TripParticipant> GetAllTripParticipants();

    }
}
