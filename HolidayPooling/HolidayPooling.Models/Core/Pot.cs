using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sams.Commons.Infrastructure.Helper;
using HolidayPooling.Infrastructure.Configuration;

namespace HolidayPooling.Models.Core
{
    [Serializable]
    [DataContract]
    public class Pot : ICloneable
    {

        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Organizer { get; set; }

        [DataMember]
        public PotMode Mode { get; set; }

        [DataMember]
        public double CurrentAmount { get; set; }

        [DataMember]
        public double TargetAmount { get; set; }

        [DataMember]
        public int TripId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsCancelled { get; set; }

        [DataMember]
        public string CancellationReason { get; set; }

        [DataMember]
        public DateTime? CancellationDate { get; set; }

        private List<PotUser> _participants;

        [DataMember]
        public IEnumerable<PotUser> Participants
        {
            get { return _participants; }
        }

        #endregion

        #region .ctor

        public Pot()
        {
            _participants = new List<PotUser>();
        }

        public Pot(int id, int tripId, string organizer, PotMode mode, double amount, double targetAmount, string name,
            DateTime startDate, DateTime endDate, DateTime validityDate, string description, bool isCancelled,
            string cancellationReason, DateTime? cancellationDate)
            : this()
        {
            Id = id;
            TripId = tripId;
            Organizer = organizer;
            Mode = mode;
            CurrentAmount = amount;
            TargetAmount = targetAmount;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            ValidityDate = validityDate;
            Description = description;
            IsCancelled = isCancelled;
            CancellationReason = cancellationReason;
            CancellationDate = cancellationDate;
        }

        public Pot(int id, int tripId, string organizer, PotMode mode, double amount, double targetAmount, string name,
            DateTime startDate, DateTime endDate, DateTime validityDate, string description, bool isCancelled,
            string cancellationReason, DateTime? cancellationDate, List<PotUser> participants)

            : this(id, tripId, organizer, mode, amount, targetAmount, name, startDate, endDate, validityDate,
            description, isCancelled, cancellationReason, cancellationDate)
        {
            _participants = participants;
        }

        internal Pot(Pot pot)
            : this(pot.Id, pot.TripId, pot.Organizer, pot.Mode, pot.CurrentAmount, pot.TargetAmount, pot.Name,
            pot.StartDate, pot.EndDate, pot.ValidityDate, pot.Description, pot.IsCancelled, pot.CancellationReason,
            pot.CancellationDate, pot._participants)
        {

        }

        #endregion

        #region Methods

        public void AddParticipant(PotUser potUser)
        {
            if (Id != potUser.PotId)
            {
                return;
            }

            if (!_participants.Contains(potUser))
            {
                _participants.Add(potUser);
            }
        }

        public void DeleteParticipant(PotUser potUser)
        {
            if (Id != potUser.PotId)
            {
                return;
            }

            if (_participants.Contains(potUser))
            {
                _participants.Remove(potUser);
            }
        }

        public void UpdateParticipant(PotUser potUser)
        {
            if (Id != potUser.PotId)
            {
                return;
            }

            if (_participants.Contains(potUser))
            {
                _participants.Remove(potUser);
                _participants.Add(potUser);
            }
        }

        public PotUser GetParticipant(int userId)
        {
            return _participants.FirstOrDefault(u => u.UserId == userId);
        }

        internal PotUser GetParticipantByIndex(int index)
        {
            if (index < 0 || _participants.Count == 0 || index >= _participants.Count)
            {
                return null;
            }

            return _participants[index];
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            var clonedParticipant = _participants != null ? _participants.CloneAll().ToList() : new List<PotUser>();
            var clone = new Pot(this);
            clone._participants = clonedParticipant;
            return clone;
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

            return Equals((Pot)obj);
        }

        public bool Equals(Pot pot)
        {
            if (pot == null)
            {
                return false;
            }

            if (ReferenceEquals(this, pot))
            {
                return true;
            }

            return Id == pot.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)HashCodeHelper.HashConstant;
                hash = HashCodeHelper.GetUnitaryHashcode(hash, Id);
                return hash;
            }
        }

        #endregion
    }
}