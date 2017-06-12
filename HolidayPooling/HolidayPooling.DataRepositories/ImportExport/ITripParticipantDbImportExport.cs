using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.ImportExport
{
    public interface ITripParticipantDbImportExport : IDbImportExport<TripParticipantKey, TripParticipant>
    {

        IEnumerable<TripParticipant> GetParticipantsForTrip(int tripId);

    }
}