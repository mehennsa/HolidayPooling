using HolidayPooling.Infrastructure.Configuration;
using System;
using System.Runtime.Serialization;

namespace HolidayPooling.Models.Core
{
    [DataContract]
    [Serializable]
    public class TripParticipant : ICloneable
    {

        #region Properties

        [DataMember]
        public int TripId { get; set; }

        [DataMember]
        public string UserPseudo { get; set; }

        [DataMember]
        public bool HasParticipated { get; set; }

        [DataMember]
        public double TripNote { get; set; }

        [DataMember]
        public DateTime? ValidationDate { get; set; }

        #endregion

        #region .ctor

        public TripParticipant()
        {

        }

        public TripParticipant(int tripId, string userPseudo, bool hasParticipated, double tripNote, DateTime? validationDate)
            : this()
        {
            TripId = tripId;
            UserPseudo = userPseudo;
            HasParticipated = hasParticipated;
            TripNote = tripNote;
            ValidationDate = validationDate;
        }

        internal TripParticipant(TripParticipant aTripParticipant)
            : this(aTripParticipant.TripId, aTripParticipant.UserPseudo, aTripParticipant.HasParticipated,
            aTripParticipant.TripNote, aTripParticipant.ValidationDate)
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

            return Equals((TripParticipant)obj);
        }

        public bool Equals(TripParticipant participant)
        {

            if (participant == null)
            {
                return false;
            }

            if (ReferenceEquals(this, participant))
            {
                return true;
            }

            return UserPseudo == participant.UserPseudo && TripId == participant.TripId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)HashCodeHelper.HashConstant;
                hash = HashCodeHelper.GetUnitaryHashcode(hash, UserPseudo);
                hash = HashCodeHelper.GetUnitaryHashcode(hash, TripId);
                return hash;
            }
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new TripParticipant(this);
        }

        #endregion
    }
}