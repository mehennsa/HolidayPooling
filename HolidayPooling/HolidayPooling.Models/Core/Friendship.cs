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
    public class Friendship : ICloneable
    {

        #region Properties

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string FriendName { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public bool IsRequested { get; set; }

        [DataMember]
        public bool IsWaiting { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        #endregion

        #region .ctor

        public Friendship()
        {
        }

        public Friendship(int userId, string friendName, DateTime startDate, bool isRequested, bool isWaiting)
            : this()
        {
            UserId = userId;
            FriendName = friendName;
            StartDate = startDate;
            IsRequested = isRequested;
            IsWaiting = isWaiting;
        }

        internal Friendship(int userId, string friendName, DateTime startDate, bool isRequested, bool isWaiting, DateTime modificationDate)
            : this(userId, friendName, startDate, isRequested, isWaiting)
        {
            ModificationDate = modificationDate;
        }

        internal Friendship(Friendship aFriendship)
            : this(aFriendship.UserId, aFriendship.FriendName, aFriendship.StartDate,
            aFriendship.IsRequested, aFriendship.IsWaiting, aFriendship.ModificationDate)
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

            return Equals((Friendship)obj);

        }

        public bool Equals(Friendship friendship)
        {
            if (friendship == null)
            {
                return false;
            }

            if (ReferenceEquals(this, friendship))
            {
                return true;
            }

            return FriendName == friendship.FriendName && UserId == friendship.UserId;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)HashCodeHelper.HashConstant;
                hash = HashCodeHelper.GetUnitaryHashcode(hash, FriendName);
                hash = HashCodeHelper.GetUnitaryHashcode(hash, UserId);
                return hash;
            }
        }

        #endregion


        #region ICloneable

        public virtual object Clone()
        {
            return new Friendship(this);
        }

        #endregion
    }
}