using HolidayPooling.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Core
{
    [Serializable]
    [DataContract]
    public class PotUser : ICloneable
    {

        #region Fields

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int PotId { get; set; }

        [DataMember]
        public bool HasPayed { get; set; }

        [DataMember]
        public double Amount { get; set; }

        [DataMember]
        public double TargetAmount { get; set; }

        [DataMember]
        public bool HasCancelled { get; set; }

        [DataMember]
        public string CancellationReason { get; set; }

        [DataMember]
        public bool HasValidated { get; set; }

        #endregion

        #region .ctor

        public PotUser()
        {

        }

        public PotUser(int userId, int potId, bool hasPayed, double amount, double targetAmount, bool hasCancelled,
            string cancellationReason, bool hasValidated) : this()
        {
            UserId = userId;
            PotId = potId;
            HasPayed = hasPayed;
            Amount = amount;
            TargetAmount = targetAmount;
            HasCancelled = hasCancelled;
            CancellationReason = cancellationReason;
            HasValidated = hasValidated;
        }

        internal PotUser(PotUser potUser)
            : this(potUser.UserId, potUser.PotId, potUser.HasPayed, potUser.Amount, potUser.TargetAmount,
            potUser.HasCancelled, potUser.CancellationReason, potUser.HasValidated)
        {

        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new PotUser(this);
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

            return Equals((PotUser)obj);
        }

        public bool Equals(PotUser otherPotUser)
        {
            if (otherPotUser == null)
            {
                return false;
            }

            if (ReferenceEquals(this, otherPotUser))
            {
                return true;
            }

            return PotId == otherPotUser.PotId && UserId == otherPotUser.UserId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)HashCodeHelper.HashConstant;
                hash = HashCodeHelper.GetUnitaryHashcode(hash, UserId);
                hash = HashCodeHelper.GetUnitaryHashcode(hash, PotId);
                return hash;
            }
        }

        #endregion
    }
}