using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Tests
{
    internal static class ModelTestHelper
    {

        #region Methods

        internal static Friendship CreateFriendship(int userId, string friendName, DateTime? startDate = null, bool isRequested = false,
            bool isWaiting = true)
        {
            var valDate = startDate.HasValue ? startDate.Value : DateTime.Today;
            return new Friendship(userId, friendName, valDate, isRequested, isWaiting);
        }

        internal static PotUser CreatePotUser(int userId, int potId, bool hasPayed = false, double amount = 100,
            double targetAmount = 1000, bool hasCancelled = false, string cancellationReason = null, bool hasValidated = true)
        {
            return new PotUser
                (
                    userId,
                    potId,
                    hasPayed,
                    amount,
                    targetAmount,
                    hasCancelled,
                    string.IsNullOrEmpty(cancellationReason) ? "TestCancel" : cancellationReason,
                    hasValidated
                );
        }

        internal static UserTrip CreateUserTrip(int userId, string tripName, bool hasParticipated = true,
            bool hasOrganized = false, double userNote = 2.25, double tripAmount = 512.6)
        {
            return new UserTrip
                (
                    userId,
                    tripName,
                    hasParticipated,
                    hasOrganized,
                    userNote,
                    tripAmount
                );
        }

        internal static TripParticipant CreateTripParticipant(int tripId, string userPseudo,
            bool hasParticipated = false, double tripNote = 3.1, DateTime? validationDate = null)
        {
            return new TripParticipant
                (
                    tripId,
                    userPseudo,
                    hasParticipated,
                    tripNote,
                    validationDate
                );
        }

        internal static Pot CreatePot(int id, int tripId, string organizer = "orga", PotMode mode = PotMode.Lead,
            double amount = 1000, double targetAmount = 2000, string name = "PotName",
            DateTime? startDate = null, DateTime? endDate = null, DateTime? validityDate = null,
            string description = "TestDesc", bool isCancelled = false, string cancellationReason = "Reason",
            DateTime? cancellationDate = null)
        {
            var valStart = startDate.HasValue ? startDate.Value : DateTime.Today;
            var valEnd = endDate.HasValue ? endDate.Value : DateTime.Today;
            var valValidity = validityDate.HasValue ? validityDate.Value : DateTime.Today;
            return new Pot
                (
                    id,
                    tripId,
                    organizer,
                    mode,
                    amount,
                    targetAmount,
                    name,
                    valStart,
                    valEnd,
                    valValidity,
                    description,
                    isCancelled,
                    cancellationReason,
                    cancellationDate
                );
        }

        internal static Pot CreatePot(int id, int tripId, List<PotUser> participants,
            string organizer = "orga", PotMode mode = PotMode.Lead,
            double amount = 1000, double targetAmount = 2000, string name = "PotName",
            DateTime? startDate = null, DateTime? endDate = null, DateTime? validityDate = null,
            string description = "TestDesc", bool isCancelled = false, string cancellationReason = "Reason",
            DateTime? cancellationDate = null)
        {
            var valStart = startDate.HasValue ? startDate.Value : DateTime.Today;
            var valEnd = endDate.HasValue ? endDate.Value : DateTime.Today;
            var valValidity = validityDate.HasValue ? validityDate.Value : DateTime.Today;
            return new Pot
                (
                    id,
                    tripId,
                    organizer,
                    mode,
                    amount,
                    targetAmount,
                    name,
                    valStart,
                    valEnd,
                    valValidity,
                    description,
                    isCancelled,
                    cancellationReason,
                    cancellationDate,
                    participants
                );
        }

        internal static Trip CreateTrip(int id, string tripName, List<TripParticipant> participants, Pot pot,
            double price = 100.30, string description = "Desc",
            int maxNbPeople = 10, string location = "loc", string organizer = "orga",
            DateTime? startDate = null, DateTime? endDate = null, DateTime? validityDate = null,
            double note = 3.2)
        {
            var valStart = startDate.HasValue ? startDate.Value : DateTime.Today;
            var valEnd = endDate.HasValue ? endDate.Value : DateTime.Today;
            var valValidity = validityDate.HasValue ? validityDate.Value : DateTime.Today;
            return new Trip
                (
                    id,
                    tripName,
                    price,
                    description,
                    maxNbPeople,
                    location,
                    organizer,
                    valStart,
                    valEnd,
                    valValidity,
                    note,
                    pot,
                    participants
                );
        }

        internal static Trip CreateTrip(int id, string tripName, double price = 100.30, string description = "Desc",
            int maxNbPeople = 10, string location = "loc", string organizer = "orga",
            DateTime? startDate = null, DateTime? endDate = null, DateTime? validityDate = null,
            double note = 3.2)
        {
            var valStart = startDate.HasValue ? startDate.Value : DateTime.Today;
            var valEnd = endDate.HasValue ? endDate.Value : DateTime.Today;
            var valValidity = validityDate.HasValue ? validityDate.Value : DateTime.Today;
            return new Trip
                (
                    id,
                    tripName,
                    price,
                    description,
                    maxNbPeople,
                    location,
                    organizer,
                    valStart,
                    valEnd,
                    valValidity,
                    note
                );
        }
        
        internal static User CreateUser(int id, string pseudo, string mail = "myEmail",
            string password = "Pwd", int age = 22, string description = "desc", RoleEnum role = RoleEnum.Admin,
            DateTime? creationDate = null, string phoneNumber = "phoneNumber", UserType type = UserType.Customer,
            double note = 3.2)
        {
            var valCreationDate = creationDate.HasValue ? creationDate.Value : DateTime.Today;
            return new User
                (
                    id,
                    pseudo,
                    mail,
                    password,
                    age,
                    description,
                    role,
                    valCreationDate,
                    phoneNumber,
                    type,
                    note
                );
        }

        internal static User CreateUser(int id, string pseudo, List<string> centerOfInterest, List<Friendship> friends,
           List<UserTrip> trips, string mail = "myEmail",
           string password = "Pwd", int age = 22, string description = "desc", RoleEnum role = RoleEnum.Admin,
           DateTime? creationDate = null, string phoneNumber = "phoneNumber", UserType type = UserType.Customer,
           double note = 3.2)
        {
            var valCreationDate = creationDate.HasValue ? creationDate.Value : DateTime.Today;
            return new User
                (
                    id,
                    pseudo,
                    mail,
                    password,
                    age,
                    description,
                    role,
                    valCreationDate,
                    phoneNumber,
                    type,
                    note,
                    centerOfInterest,
                    trips,
                    friends
                );
        }

        #endregion

    }
}
