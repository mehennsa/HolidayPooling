using HolidayPooling.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Core
{
    [DataContract]
    [Serializable]
    public class UserTrip : ICloneable
    {

        #region Properties

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string TripName { get; set; }

        [DataMember]
        public bool HasParticipated { get; set; }

        [DataMember]
        public bool HasOrganized { get; set; }

        [DataMember]
        public double UserNote { get; set; }

        [DataMember]
        public double TripAmount { get; set; }

        #endregion

        #region .ctor

        public UserTrip()
        {
        }

        public UserTrip(int userId, string tripName, bool hasParticipated, bool hasOrganized, double userNote, double tripAmount)
            : this()
        {
            UserId = userId;
            TripName = tripName;
            HasParticipated = hasParticipated;
            HasOrganized = hasOrganized;
            UserNote = userNote;
            TripAmount = tripAmount;
        }

        internal UserTrip(UserTrip aUserTrip)
            : this(aUserTrip.UserId, aUserTrip.TripName, aUserTrip.HasParticipated, aUserTrip.HasOrganized, aUserTrip.UserNote, aUserTrip.TripAmount)
        {

        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((UserTrip)obj);
        }

        public bool Equals(UserTrip trip)
        {
            if (trip == null)
            {
                return false;
            }

            if (ReferenceEquals(this, trip))
            {
                return true;
            }

            return TripName == trip.TripName && UserId == trip.UserId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)HashCodeHelper.HashConstant;
                hash = HashCodeHelper.GetUnitaryHashcode(hash, TripName);
                hash = HashCodeHelper.GetUnitaryHashcode(hash, UserId);
                return hash;
            }
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new UserTrip(this);
        }

        #endregion
    }
}