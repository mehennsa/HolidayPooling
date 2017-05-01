using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Sams.Commons.Infrastructure.Helper;
using HolidayPooling.Infrastructure.Configuration;

namespace HolidayPooling.Models.Core
{
    [Serializable]
    [DataContract]
    public class User : ICloneable
    {

        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Pseudo { get; set; }

        [DataMember]
        public int Age { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public Role Role { get; set; }

        [DataMember]
        public UserType Type { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public double Note { get; set; }

        [DataMember]
        public List<string> CenterOfInterests { get; set; }

        private readonly List<UserTrip> _trips;

        [DataMember]
        public IEnumerable<UserTrip> Trips
        {
            get { return _trips; }
        }

        private readonly List<Friendship> _friends;

        [DataMember]
        public IEnumerable<Friendship> Friends
        {
            get { return _friends; }
        }

        #endregion

        #region .ctor

        public User()
        {
            Id = -1;
            _friends = new List<Friendship>();
            _trips = new List<UserTrip>();
            CenterOfInterests = new List<string>();
        }

        public User(int id, string mail, string password, string pseudo, int age, string description,
            Role role, DateTime creationDate, string phoneNumber, UserType type, double note)
            : this()
        {
            Id = id;
            Mail = mail;
            Password = password;
            Pseudo = pseudo;
            Age = age;
            Description = description;
            Role = role;
            CreationDate = creationDate;
            PhoneNumber = phoneNumber;
            Note = note;
            Type = type;
        }

        internal User(int id, string mail, string password, string pseudo, int age, string description, Role role,
            DateTime creationDate, string phoneNumber, UserType type, double note, List<string> centerOfInterests,
            List<UserTrip> trips, List<Friendship> friends)
            : this(id, mail, password, pseudo, age, description, role, creationDate, phoneNumber, type, note)
        {
            _trips = trips;
            _friends = friends;
            CenterOfInterests = centerOfInterests;
        }

        internal User(User user)
            : this(user.Id, user.Mail, user.Password, user.Pseudo, user.Age, user.Description, user.Role, user.CreationDate,
            user.PhoneNumber, user.Type, user.Note, user.CenterOfInterests,
            user._trips, user._friends)
        {

        }

        #endregion

        #region Methods

        #region Friendship

        public void AddFriend(Friendship friendship)
        {
            if (friendship.UserId != Id)
            {
                return;
            }

            if (!_friends.Contains(friendship))
            {
                _friends.Add(friendship);
            }
        }

        public void DeleteFriend(Friendship friendship)
        {
            if (friendship.UserId != Id)
            {
                return;
            }

            if (_friends.Contains(friendship))
            {
                _friends.Remove(friendship);
            }
        }

        public void UpdateFriend(Friendship friendship)
        {
            if (friendship.UserId != Id)
            {
                return;
            }

            if (_friends.Contains(friendship))
            {
                _friends.Remove(friendship);
                _friends.Add(friendship);
            }
        }

        public Friendship GetFriend(string friendName)
        {
            return _friends.FirstOrDefault(f => f.FriendName == friendName);
        }

        internal Friendship GetFriend(int index)
        {
            if (index < 0 || _friends.Count == 0 || index >= _friends.Count)
            {
                return null;
            }

            return _friends[index];
        }

        #endregion

        #region Trip

        public void AddTrip(UserTrip trip)
        {
            if (trip.UserId != Id)
            {
                return;
            }

            if (!_trips.Contains(trip))
            {
                _trips.Add(trip);
            }
        }

        public void DeleteTrip(UserTrip trip)
        {
            if (trip.UserId != Id)
            {
                return;
            }

            if (_trips.Contains(trip))
            {
                _trips.Remove(trip);
            }
        }

        public void UpdateTrip(UserTrip trip)
        {
            if (trip.UserId != Id)
            {
                return;
            }

            if (_trips.Contains(trip))
            {
                _trips.Remove(trip);
                _trips.Add(trip);
            }
        }

        public UserTrip GetTrip(string tripName)
        {
            return _trips.FirstOrDefault(t => t.TripName == tripName);
        }

        internal UserTrip GetTrip(int index)
        {
            if (index < 0 || _trips.Count == 0 || _trips.Count <= index)
            {
                return null;
            }

            return _trips[index];
        }

        #endregion

        #endregion

        #region ICloneable

        public object Clone()
        {
            var clonedTrips = _trips != null ? _trips.CloneAll().ToList() : new List<UserTrip>();
            var clonedFriends = _friends != null ? _friends.CloneAll().ToList() : new List<Friendship>();
            var clonedCenterOfInterests = CenterOfInterests != null ? CenterOfInterests.CloneAll().ToList() : new List<string>();
            return new User(this.Id, this.Mail, this.Password, this.Pseudo, this.Age, this.Description, this.Role, this.CreationDate,
                PhoneNumber, Type, Note, clonedCenterOfInterests,
                clonedTrips, clonedFriends);
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

            return Equals((User)obj);
        }

        public bool Equals(User user)
        {
            if (user == null)
            {
                return false;
            }

            if (ReferenceEquals(this, user))
            {
                return true;
            }

            return Id == user.Id;
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
            return string.Format("{0}, {1}", Pseudo, Age);
        }

        #endregion

    }
}