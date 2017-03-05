using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public interface ITripParticipantDbImportExport : IDbImportExport<TripParticipantKey, TripParticipant>
    {

        IEnumerable<TripParticipant> GetParticipantsForTrip(int tripId);

    }
}