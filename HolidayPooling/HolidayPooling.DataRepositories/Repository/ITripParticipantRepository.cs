using HolidayPooling.Models.Core;
using System.Collections.Generic;

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
