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
    public class Trip : ICloneable
    {

        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string TripName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public int NumberMaxOfPeople { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Organizer { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Pot TripPot { get; set; }

        private List<TripParticipant> _participants;

        [DataMember]
        public IEnumerable<TripParticipant> Participants
        {
            get { return _participants; }
            set
            {

                _participants = value != null ? new List<TripParticipant>(value) : null;
            }
        }

        [DataMember]
        public double Note { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        #endregion

        #region .ctor

        public Trip()
        {
            Id = -1;
            // Illimited
            NumberMaxOfPeople = -1;
            _participants = new List<TripParticipant>();
            TripPot = new Pot();
        }

        public Trip(int id, string tripName, double price, string description, int numberMaxOfPeople, string location,
            string organizer, DateTime startDate, DateTime endDate, DateTime validityDate, double note)
            : this()
        {
            Id = id;
            TripName = tripName;
            Price = price;
            Description = description;
            NumberMaxOfPeople = numberMaxOfPeople;
            Location = location;
            Organizer = organizer;
            StartDate = startDate;
            EndDate = endDate;
            ValidityDate = validityDate;
            Note = note;
        }

        internal Trip(int id, string tripName, double price, string description, int numberMaxOfPeople, string location,
            string organizer, DateTime startDate, DateTime endDate, DateTime validityDate,
            double note, Pot tripPot, List<TripParticipant> participants)
            : this(id, tripName, price, description, numberMaxOfPeople, location, organizer, startDate, endDate, validityDate, note)
        {
            _participants = participants ?? new List<TripParticipant>();
        }

        internal Trip(int id, string tripName, double price, string description, int numberMaxOfPeople, string location,
            string organizer, DateTime startDate, DateTime endDate, DateTime validityDate,
            double note, Pot tripPot, List<TripParticipant> participants, DateTime modificationDate)
            : this(id, tripName, price, description, numberMaxOfPeople, location, organizer, startDate, endDate, validityDate, note, tripPot, participants)
        {
            ModificationDate = modificationDate;
        }

        internal Trip(Trip aTrip)
            : this(aTrip.Id, aTrip.TripName, aTrip.Price, aTrip.Description, aTrip.NumberMaxOfPeople, aTrip.Location,
            aTrip.Organizer, aTrip.StartDate, aTrip.EndDate, aTrip.ValidityDate, aTrip.Note, aTrip.TripPot, aTrip._participants, aTrip.ModificationDate)
        {

        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            var clonedParticipants = _participants != null ? _participants.CloneAll().ToList() : new List<TripParticipant>();
            var clonedPot = TripPot != null ? TripPot.Clone() as Pot : new Pot();
            var cloneTrip = new Trip(this);
            cloneTrip.Participants = clonedParticipants;
            cloneTrip.TripPot = clonedPot;
            return cloneTrip;
        }

        #endregion

        #region Methods

        public void AddParticipant(TripParticipant participant)
        {
            if (participant.TripId != Id)
            {
                return;
            }

            if (!_participants.Contains(participant))
            {
                _participants.Add(participant);
            }

        }

        public void DeleteParticipant(TripParticipant participant)
        {
            if (participant.TripId != Id)
            {
                return;
            }

            if (_participants.Contains(participant))
            {
                _participants.Remove(participant);
            }
        }

        public void UpdateParticipant(TripParticipant participant)
        {
            if (participant.TripId != Id)
            {
                return;
            }

            if (_participants.Contains(participant))
            {
                _participants.Remove(participant);
                _participants.Add(participant);
            }
        }

        public TripParticipant GetParticipant(string userPseudo)
        {
            return _participants.FirstOrDefault(p => p.UserPseudo == userPseudo);
        }

        internal TripParticipant GetParticipantByIndex(int index)
        {
            if (index < 0 || _participants == null || _participants.Count == 0 || _participants.Count <= index)
            {
                return null;
            }

            return _participants[index];
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

            return Equals((Trip)obj);
        }

        public bool Equals(Trip trip)
        {
            if (trip == null)
            {
                return false;
            }

            if (ReferenceEquals(this, trip))
            {
                return true;
            }
            return Id == trip.Id;
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

        public override string ToString()
        {
            return Description;
        }

        #endregion

    }
}