using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class TripParticipantKey
    {

        #region Properties

        public int TripId { get; private set; }

        public string UserPseudo { get; private set; }

        #endregion

        #region .ctor

        public TripParticipantKey(int tripId, string userPseudo)
        {
            TripId = tripId;
            UserPseudo = userPseudo;
        }

        #endregion

    }
}