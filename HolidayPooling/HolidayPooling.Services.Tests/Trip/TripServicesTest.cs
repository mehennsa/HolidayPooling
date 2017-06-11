using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Services.Trips;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.Services.Tests.Trip
{
    [TestFixture]
    public class TripServicesTest
    {

        #region Fields

        private ITripServices _tripServices;
        private Mock<ITripRepository> _mockTripRepo;
        private Mock<ITripParticipantRepository> _mockTripParticipant;
        private Mock<IUserTripRepository> _mockUserTripRepo;
        private Mock<IPotRepository> _mockPotRepo;
        private Mock<IPotUserRepository> _mockPotUserRepo;

        #endregion

        #region Methods

        private void CreateServices()
        {
            _tripServices = new TripServices(_mockTripRepo.Object, _mockPotRepo.Object, _mockTripParticipant.Object, _mockUserTripRepo.Object, _mockPotUserRepo.Object);
        }

        private void CreateMocks()
        {
            _mockTripRepo = new Mock<ITripRepository>();
            _mockPotRepo = new Mock<IPotRepository>();
            _mockTripParticipant = new Mock<ITripParticipantRepository>();
            _mockUserTripRepo = new Mock<IUserTripRepository>();
            _mockPotUserRepo = new Mock<IPotUserRepository>();
        }

        #endregion

        #region SetUp

        [SetUp]
        public void Setup()
        {
            CreateMocks();
        }

        #endregion

        #region Tests

        [Test]
        public void CreateTrip_WhenException_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripForException");
            _mockTripRepo.Setup(s => s.SaveTrip(trip)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void CreateTrip_WhenUnableToSaveTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripSaveError");
            const string error = "error when saving trip";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void CreateTrip_WhenUnableToSavePot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripPotSaveError");
            const string error = "error when saving pot";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void CreateTrip_WhenUnableToSaveTripInOrganizersTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripUserTripSaveError");
            const string error = "error when saving trip in user's trip";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserTripRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void CreateTrip_WhenUnableToAddOrganizerAsTripParticipant_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripTripParticipantSaveError");
            const string error = "error when saving organizer in trip's participant";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));

        }

        [Test]
        public void CreateTrip_WhenUnableToAddOrganizerAsPotMember_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aTripPotMemberError");
            const string error = "error when saving organizer in pot's users";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void CreateTrip_WhenValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "aValidTrip");
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _tripServices.CreateTrip(trip, 1);
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.IsNotNull(trip.TripPot);
            Assert.AreEqual(trip.Price, trip.TripPot.TargetAmount);
            Assert.AreEqual(1, trip.Participants.Count());
            Assert.AreEqual(1, trip.TripPot.Participants.Count());
            var potMember = trip.TripPot.Participants.FirstOrDefault();
            Assert.IsNotNull(potMember);
            Assert.AreEqual(trip.Price / trip.NumberMaxOfPeople, potMember.TargetAmount);
        }

        [Test]
        public void DeleteTrip_WhenException_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteException");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            _mockTripParticipant.Setup(m => m.DeleteTripParticipant(It.IsAny<TripParticipant>())).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void DeleteTrip_WhenUnableToDeleteParticipant_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteFailedForParticipant");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            const string error = "Unable to delete participant";
            var errors = new List<string> { error };
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void DeleteTrip_WhenUnableToDeleteUserTrips_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteFailedForUserTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            const string error = "Unable to delete user's trip";
            var errors = new List<string> { error };
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockUserTripRepo.SetupGet(m => m.Errors).Returns(errors);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void DeleteTrip_WhenUnableToDeletePotUser_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteFailedForPotUser");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var pot = ModelTestHelper.CreatePot(1, trip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to delete pot user";
            var errors = new List<string> { error };
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            pot.AddParticipant(potUser);
            Assert.AreEqual(1, pot.Participants.Count());
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void DeleteTrip_WhenUnableToDeletePot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteFailedForPot");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var pot = ModelTestHelper.CreatePot(1, trip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to delete pot";
            var errors = new List<string> { error };
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            pot.AddParticipant(potUser);
            Assert.AreEqual(1, pot.Participants.Count());
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void DeleteTrip_WhenUnableToDeleteTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "deleteFailedForTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var pot = ModelTestHelper.CreatePot(1, trip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to delete trip";
            var errors = new List<string> { error };
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            pot.AddParticipant(potUser);
            Assert.AreEqual(1, pot.Participants.Count());
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void DeleteTrip_WhenValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "validTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var pot = ModelTestHelper.CreatePot(1, trip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            pot.AddParticipant(potUser);
            Assert.AreEqual(1, pot.Participants.Count());
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            CreateServices();
            _tripServices.DeleteTrip(trip);
            Assert.IsFalse(_tripServices.HasErrors);
        }

        [Test]
        public void Participate_WhenExceptionIsThrown_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "ParticipateException");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.Participate(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void Participate_WhenUnableToSaveParticipant_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "ParticipantFailed");
            const string error = "Error when saving participant";
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.Participate(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Participate_WhenUnableToSaveUserTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UserTripFailed");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            const string error = "Error when saving user's trip";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockUserTripRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.Participate(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Participate_WhenUnableToSaveUserPot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "PotUserFailed");
            const string error = "Error when saving user's Pot";
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.Participate(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Participate_WhenValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "PotUserFailed");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            CreateServices();
            _tripServices.Participate(trip, 1, "aUser");
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(1, trip.Participants.Count());
            var ptp = trip.Participants.FirstOrDefault();
            Assert.IsNotNull(ptp);
            Assert.AreEqual("aUser", ptp.UserPseudo);
            Assert.AreEqual(1, trip.TripPot.Participants.Count());
            var potUser = trip.TripPot.Participants.FirstOrDefault();
            Assert.IsNotNull(potUser);
            Assert.AreEqual(1, potUser.UserId);
            Assert.AreEqual(trip.Price / trip.NumberMaxOfPeople, potUser.TargetAmount);
        }

        [Test]
        public void Quit_WhenException_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "QuitException");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            _mockTripParticipant.Setup(m => m.GetTripParticipant(1, "aUser")).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void Quit_WhenErrorWhileFindingParticipant_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "ErrorFindingParticipant");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            trip.TripPot = pot;
            const string error = "Error while finding participant";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenTripParticipantIsNull_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "NullParticipant");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            const string error = "Unable to find participant";
            var errors = new List<string>();
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(1, "aUser")).Returns(() => null);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUnableToDeleteTripParticipant_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "NullParticipant");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            trip.TripPot = pot;
            const string error = "Unable to delete participant";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(new List<string>());
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockTripParticipant.Setup(m => m.DeleteTripParticipant(ptp)).Callback
                (
                () =>
                {
                    _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(true);
                    _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
                }
                );
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUserTripIsNull_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UserTripNull");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            trip.TripPot = pot;
            const string error = "Unable to find user's trip";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(() => null);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(m => m.Errors).Returns(new List<string>());
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUnableToFindUserTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToFindUserTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            trip.TripPot = pot;
            const string error = "Error while looking for user's trip";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockUserTripRepo.SetupGet(m => m.Errors).Returns(errors);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUnableToDeleteUserTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            trip.TripPot = pot;
            const string error = "Error while deleting trip";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(m => m.DeleteUserTrip(userTrip)).Callback
                (
                () =>
                {
                    _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(true);
                    _mockUserTripRepo.SetupGet(m => m.Errors).Returns(errors);
                }
                );
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenPotUserIsNull_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            trip.TripPot = pot;
            const string error = "Unable to find user in the trip's pot";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(() => null);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUnableToFindPotUser_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            trip.TripPot = pot;
            const string error = "error while looking for pot user";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenUnableToDeletePotUser_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            trip.TripPot = pot;
            const string error = "error while trying to delete pot user";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.Setup(m => m.DeletePotUser(potUser)).Callback
                (
                () =>
                {
                    _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
                    _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
                }
                );
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void Quit_WhenPotUserHasNotPayedAndValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0);
            trip.TripPot = pot;
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            trip.TripPot.AddParticipant(potUser);
            Assert.AreEqual(1, trip.TripPot.Participants.Count());
            const string error = "error while trying to delete pot user";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(0, trip.Participants.Count());
            Assert.AreEqual(0, trip.TripPot.Participants.Count());
        }

        [Test]
        public void Quit_WhenPotUserHasPayedAndUnableToUpdatePot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip", 1000);
            var pot = ModelTestHelper.CreatePot(1, 1, targetAmount: 1000, amount: 200);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 200);
            trip.TripPot = pot;
            const string error = "error while trying to update pot";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotRepo.Setup(s => s.HasErrors).Returns(true);
            _mockPotRepo.Setup(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.AreEqual(200, pot.CurrentAmount);
        }

        [Test]
        public void Quit_WhenPotUserHasPayedAndValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "UnableToDeleteTrip", 1000);
            var pot = ModelTestHelper.CreatePot(1, 1, targetAmount: 1000, amount: 200);
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 200);
            trip.TripPot = pot;
            trip.AddParticipant(ptp);
            Assert.AreEqual(1, trip.Participants.Count());
            trip.TripPot.AddParticipant(potUser);
            Assert.AreEqual(1, trip.TripPot.Participants.Count());
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.Setup(m => m.GetTripParticipant(ptp.TripId, ptp.UserPseudo)).Returns(ptp);
            _mockUserTripRepo.Setup(m => m.GetUserTrip(userTrip.UserId, userTrip.TripName)).Returns(userTrip);
            _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotRepo.Setup(s => s.HasErrors).Returns(false);
            CreateServices();
            _tripServices.Quit(trip, 1, "aUser");
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(0, pot.CurrentAmount);
            Assert.AreEqual(0, trip.Participants.Count());
            Assert.AreEqual(0, trip.TripPot.Participants.Count());
        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenExceptionIsThrown_ShouldThrowException()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdate");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.UpdateAllowedNumberOfPeople(trip, 200);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenUnableToUpdatePotUser_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdate");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "error while updating pot user";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(m => m.UpdatePotUser(potUser)).Callback
                (
                    () =>
                    {
                        _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
                        _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
                    }
                );
            CreateServices();
            _tripServices.UpdateAllowedNumberOfPeople(trip, 200);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenUnableToUpdateTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdateTrip", maxNbPeople: 100);
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "error while updating trip";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.UpdateAllowedNumberOfPeople(trip, 200);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.AreEqual(100, trip.NumberMaxOfPeople);
        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "ValidUpdate", maxNbPeople: 100);
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _tripServices.UpdateAllowedNumberOfPeople(trip, 200);
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(200, trip.NumberMaxOfPeople);
        }

        [Test]
        public void UpdatePrice_WhenException_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "ExceptionToUpdate");
            var pot = ModelTestHelper.CreatePot(1, 1);
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _tripServices.UpdatePrice(trip, 250.2);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void UpdatePrice_WhenUnableToUpdateUserPot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdateUserPot");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "error while updating pot user";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.Setup(m => m.UpdatePotUser(potUser)).Callback
                (
                    () =>
                    {
                        _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
                        _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
                    }
                );
            CreateServices();
            _tripServices.UpdatePrice(trip, 312.3);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void UpdatePrice_WhenUnableToUpdateUserTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedTopUpdateUserTrip");
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            const string error = "error while updating user trip";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockUserTripRepo.Setup(m => m.UpdateUserTrip(userTrip)).Callback
                (
                    () =>
                    {
                        _mockUserTripRepo.SetupGet(m => m.HasErrors).Returns(true);
                        _mockUserTripRepo.SetupGet(m => m.Errors).Returns(errors);
                    }
                );
            CreateServices();
            _tripServices.UpdatePrice(trip, 312.3);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
        }

        [Test]
        public void UpdatePrice_WhenUnableToUpdatePot_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdatePot", 2000);
            var pot = ModelTestHelper.CreatePot(1, 1, targetAmount: 2000);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            const string error = "error while updating pot";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.UpdatePrice(trip, 312.3);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.AreEqual(2000, pot.TargetAmount);
        }

        [Test]
        public void UpdatePrice_WhenUnableToUpdateTrip_ShouldLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdatePot", price: 2000);
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            const string error = "error while updating pot";
            var errors = new List<string> { error };
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _tripServices.UpdatePrice(trip, 312.3);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.AreEqual(2000, trip.Price);
        }

        [Test]
        public void UpdatePrice_WhenValid_ShouldNotLogError()
        {
            var trip = ModelTestHelper.CreateTrip(1, "failedToUpdatePot", price: 2000);
            var pot = ModelTestHelper.CreatePot(1, 1, targetAmount: 2000);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            trip.TripPot = pot;
            _mockPotUserRepo.Setup(s => s.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _tripServices.UpdatePrice(trip, 312.3);
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(312.3, trip.Price);
            Assert.AreEqual(312.3, trip.TripPot.TargetAmount);
        }

        [Test]
        public void GetParticipants_WhenException_ShouldLogError()
        {
            _mockTripParticipant.Setup(s => s.GetTripParticipants(1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            var list = _tripServices.GetParticipants(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetParticipants_WhenUnableToRetrieveParticipant_ShouldLogError()
        {
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            const string error = "Error while retrieving participant";
            var errors = new List<string> { error };
            _mockTripParticipant.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(s => s.Errors).Returns(errors);
            _mockTripParticipant.Setup(s => s.GetTripParticipants(1)).Returns(new List<TripParticipant> { ptp });
            CreateServices();
            var list = _tripServices.GetParticipants(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetParticipants_WhenValid_ShouldReturnRightRecords()
        {
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            _mockTripParticipant.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripParticipant.Setup(s => s.GetTripParticipants(1)).Returns(new List<TripParticipant> { ptp });
            CreateServices();
            var list = _tripServices.GetParticipants(1);
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.AreEqual(1, list.Count());
            Assert.IsTrue(list.Any(t => t.UserPseudo == "aUser"));
        }

        [Test]
        public void GetTrip_WhenExceptionIsThrown_ShouldLogError()
        {
            _mockTripRepo.Setup(m => m.GetTrip(1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains("ExceptionForTest"));
            Assert.IsNull(trip);
        }

        [Test]
        public void GetTrip_WhenUnableToRetrieveTrip_ShouldLogError()
        {
            var mockTrip = ModelTestHelper.CreateTrip(1, "failedLoadTrip");
            const string error = "Unable to retrieveTrip";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(m => m.Errors).Returns(errors);
            _mockTripRepo.Setup(m => m.GetTrip(1)).Returns(mockTrip);
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.IsNull(trip);
        }

        [Test]
        public void GetTrip_WhenUnableToGetTripParticipants_ShouldLogError()
        {
            var mockTrip = ModelTestHelper.CreateTrip(1, "failedLoadTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(1, "aUser");
            const string error = "Unable to load trip participants";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(true);
            _mockTripParticipant.SetupGet(m => m.Errors).Returns(errors);
            _mockTripRepo.Setup(m => m.GetTrip(1)).Returns(mockTrip);
            _mockTripParticipant.Setup(m => m.GetTripParticipants(mockTrip.Id)).Returns(new List<TripParticipant> { ptp });
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.IsNull(trip);
        }

        [Test]
        public void GetTrip_WhenUnableToRetrievePot_ShouldLogError()
        {
            var mockTrip = ModelTestHelper.CreateTrip(1, "failedLoadTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(mockTrip.Id, "aUser");
            var pot = ModelTestHelper.CreatePot(1, mockTrip.Id);
            const string error = "Unable to load pot";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(m => m.Errors).Returns(errors);
            _mockTripRepo.Setup(m => m.GetTrip(1)).Returns(mockTrip);
            _mockTripParticipant.Setup(m => m.GetTripParticipants(1)).Returns(new List<TripParticipant> { ptp });
            _mockPotRepo.Setup(m => m.GetTripPots(mockTrip.Id)).Returns(pot);
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.IsNull(trip);
        }

        [Test]
        public void GetTrip_WhenUnableToRetrievePotUser_ShouldLogError()
        {
            var mockTrip = ModelTestHelper.CreateTrip(1, "failedLoadTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(mockTrip.Id, "aUser");
            var pot = ModelTestHelper.CreatePot(1, mockTrip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to load pot users";
            var errors = new List<string> { error };
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(m => m.Errors).Returns(errors);
            _mockTripRepo.Setup(m => m.GetTrip(1)).Returns(mockTrip);
            _mockTripParticipant.Setup(m => m.GetTripParticipants(1)).Returns(new List<TripParticipant> { ptp });
            _mockPotRepo.Setup(m => m.GetTripPots(mockTrip.Id)).Returns(pot);
            _mockPotUserRepo.Setup(m => m.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsTrue(_tripServices.HasErrors);
            Assert.IsTrue(_tripServices.Errors.Contains(error));
            Assert.IsNull(trip);
        }

        [Test]
        public void GetTrip_WhenValid_ShouldReturnValidTrip()
        {
            var mockTrip = ModelTestHelper.CreateTrip(1, "ValidLoadTrip");
            var ptp = ModelTestHelper.CreateTripParticipant(mockTrip.Id, "aUser");
            var pot = ModelTestHelper.CreatePot(1, mockTrip.Id);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            _mockTripRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripParticipant.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(m => m.HasErrors).Returns(false);
            _mockTripRepo.Setup(m => m.GetTrip(1)).Returns(mockTrip);
            _mockTripParticipant.Setup(m => m.GetTripParticipants(1)).Returns(new List<TripParticipant> { ptp });
            _mockPotRepo.Setup(m => m.GetTripPots(mockTrip.Id)).Returns(pot);
            _mockPotUserRepo.Setup(m => m.GetPotUsers(pot.Id)).Returns(new List<PotUser> { potUser });
            CreateServices();
            var trip = _tripServices.GetTrip(1);
            Assert.IsFalse(_tripServices.HasErrors);
            Assert.IsNotNull(trip);
            Assert.AreEqual(1, trip.Participants.Count());
            Assert.IsNotNull(trip.TripPot);
            Assert.AreEqual(1, trip.TripPot.Participants.Count());
        }

        #endregion

    }
}
