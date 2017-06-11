using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Services.Trips;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.Services.Tests.Integration
{
    [TestFixture]
    public class TripServicesIntegrationTest : BaseIntegrationTest
    {

        #region BaseIntegrationTest

        protected override IEnumerable<string> TableUsed
        {
            get
            {
                yield return "TTRP";
                yield return "TPOT";
                yield return "TTRPPTP";
                yield return "TUSRTRP";
                yield return "TPOTUSR";
            }
        }

        #endregion

        #region Test

        [Test]
        public void CreateTrip_WhenExceptionIsThrown_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "createTripRollback");
            var tripRepo = new TripRepository();
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.Setup(s => s.SavePot(It.IsAny<Pot>())).Throws(new Exception("RollbackException"));
            var services = new TripServices(tripRepo, mockPotRepo.Object, new TripParticipantRepository(), 
                new UserTripRepository(), new PotUserRepository());
            services.CreateTrip(trip, 1);
            var secondServices = new TripServices();
            Assert.IsNull(secondServices.GetTrip(trip.Id));
        }

        [Test]
        public void CreateTrip_WhenErrorDuringProcess_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "createTripError");
            var tripRepo = new TripRepository();
            var potRepo = new PotRepository();
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.Setup(s => s.SavePot(It.IsAny<Pot>())).Callback(() => potRepo.SavePot(It.IsAny<Pot>()));
            mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockPotRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var services = new TripServices(tripRepo, mockPotRepo.Object, new TripParticipantRepository(),
                new UserTripRepository(), new PotUserRepository());
            services.CreateTrip(trip, 1);
            var secondServices = new TripServices();
            Assert.IsNull(secondServices.GetTrip(trip.Id));
        }

        [Test]
        public void CreateTrip_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "createTripCommit", organizer:"aUser");
            var userTripRepo = new UserTripRepository();
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var dbTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(dbTrip);
            Assert.IsNotNull(dbTrip.TripPot);
            var pot = dbTrip.TripPot;
            Assert.AreEqual(trip.Price, pot.TargetAmount);
            Assert.AreEqual(1, dbTrip.Participants.Count());
            var ptp = dbTrip.Participants.First();
            Assert.AreEqual(dbTrip.Id, ptp.TripId);
            Assert.AreEqual(1, pot.Participants.Count());
            var potUser = pot.Participants.First();
            Assert.AreEqual(pot.Id, potUser.PotId);
            var userTrips = userTripRepo.GetUserTripsByTrip(trip.TripName);
            Assert.IsFalse(userTripRepo.HasErrors);
            Assert.AreEqual(1, userTrips.Count());
            var userTrip = userTrips.First();
            Assert.AreEqual(trip.TripName, userTrip.TripName);
        }

        [Test]
        public void DeleteTrip_WhenException_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "deleteTripException");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Throws(new Exception("Exception"));
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.DeleteTrip(reloadedTrip);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
        }

        [Test]
        public void DeleteTrip_WhenError_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "deleteTripError");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            var userTrip = ModelTestHelper.CreateUserTrip(1, trip.TripName);
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.GetUserTripsByTrip(trip.TripName)).Returns(new List<UserTrip> { userTrip });
            mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockUserTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.DeleteTrip(reloadedTrip);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
        }

        [Test]
        public void DeleteTrip_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "deleteTripCommit");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            var pot = reloadedTrip.TripPot;
            Assert.IsNotNull(pot);
            var participants = reloadedTrip.Participants;
            Assert.AreEqual(1, participants.Count());
            var potUsers = pot.Participants;
            Assert.AreEqual(1, potUsers.Count());
            var tripRepo = new TripRepository();
            var potRepo = new PotRepository();
            var userTripRepo = new UserTripRepository();
            var potUserRepo = new PotUserRepository();
            var tripParticipantRepo = new TripParticipantRepository();
            services.DeleteTrip(reloadedTrip);
            Assert.IsFalse(services.HasErrors);
            Assert.IsNull(tripRepo.GetTrip(reloadedTrip.Id));
            Assert.IsNull(potRepo.GetPot(pot.Id));
            Assert.AreEqual(0, userTripRepo.GetUserTripsByTrip(reloadedTrip.TripName).Count());
            Assert.AreEqual(0, potUserRepo.GetPotUsers(pot.Id).Count());
            Assert.AreEqual(0, tripParticipantRepo.GetTripParticipants(reloadedTrip.Id).Count());
            
        }

        [Test]
        public void Participate_WhenException_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "participateException");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            Assert.IsNotNull(reloadedTrip);
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.SaveUserTrip(It.IsAny<UserTrip>())).Throws(new Exception("Exception"));
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsTrue(secondServices.HasErrors);
            Assert.AreEqual(1, services.GetParticipants(reloadedTrip.Id).Count());
        }

        [Test]
        public void Participate_WhenError_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "participateError");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            Assert.IsNotNull(reloadedTrip);
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.SaveUserTrip(It.IsAny<UserTrip>())).Callback(() => new UserTripRepository().SaveUserTrip(It.IsAny<UserTrip>()));
            mockUserTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockUserTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsTrue(secondServices.HasErrors);
            Assert.AreEqual(1, services.GetParticipants(reloadedTrip.Id).Count());
            Assert.AreEqual(1, new UserTripRepository().GetUserTripsByTrip(reloadedTrip.TripName).Count());
        }

        [Test]
        public void Participate_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "participateCommit");
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            Assert.IsNotNull(reloadedTrip);
            services.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            Assert.AreEqual(2, services.GetParticipants(reloadedTrip.Id).Count());
            Assert.AreEqual(2, new UserTripRepository().GetUserTripsByTrip(reloadedTrip.TripName).Count());
            Assert.AreEqual(2, new PotUserRepository().GetPotUsers(reloadedTrip.TripPot.Id).Count());
        }

        [Test]
        public void Quit_WhenException_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "QuitException");
            var services = new TripServices();
            var userTrip = ModelTestHelper.CreateUserTrip(2, trip.TripName);
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            services.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.DeleteUserTrip(It.IsAny<UserTrip>())).Throws(new Exception("Exception"));
            mockUserTripRepo.Setup(s => s.GetUserTrip(2, reloadedTrip.TripName)).Returns(userTrip);
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.Quit(reloadedTrip, 2, "aSecondUser");
            Assert.IsTrue(secondServices.HasErrors);
            Assert.AreEqual(2, services.GetParticipants(reloadedTrip.Id).Count());
        }

        [Test]
        public void Quit_WhenError_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "QuitError");
            var services = new TripServices();
            var userTrip = ModelTestHelper.CreateUserTrip(2, trip.TripName);
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            services.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            var mockUserTripRepo = new Mock<IUserTripRepository>();
            mockUserTripRepo.Setup(s => s.GetUserTrip(2, reloadedTrip.TripName)).Returns(userTrip);
            mockUserTripRepo.Setup(s => s.HasErrors).Returns(true);
            mockUserTripRepo.Setup(s => s.Errors).Returns(new List<string>());
            var secondServices = new TripServices(new TripRepository(), new PotRepository(), new TripParticipantRepository(),
                            mockUserTripRepo.Object, new PotUserRepository());
            secondServices.Quit(reloadedTrip, 2, "aSecondUser");
            Assert.IsTrue(secondServices.HasErrors);
            Assert.AreEqual(2, services.GetParticipants(reloadedTrip.Id).Count());
        }

        [Test]
        public void Quit_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "QuitCommit", 1000);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            services.Participate(reloadedTrip, 2, "aSecondUser");
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            Assert.IsNotNull(reloadedTrip.TripPot);
            var pot = reloadedTrip.TripPot;
            pot.CurrentAmount = 200;
            var potRepo = new PotRepository();
            potRepo.UpdatePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUserRepo = new PotUserRepository();
            var potUser = potUserRepo.GetPotUser(pot.Id, 2);
            Assert.IsFalse(potUserRepo.HasErrors);
            Assert.IsNotNull(potUser);
            potUser.Amount = 200;
            potUserRepo.UpdatePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            services.Quit(reloadedTrip, 2, "aSecondUser");
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            reloadedTrip = services.GetTrip(reloadedTrip.Id);
            Assert.IsFalse(services.HasErrors, string.Join(",", services.Errors));
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            pot = reloadedTrip.TripPot;
            Assert.IsNotNull(pot);
            Assert.AreEqual(0, pot.CurrentAmount);
            Assert.AreEqual(1, pot.Participants.Count());
            Assert.AreEqual(1, new UserTripRepository().GetUserTripsByTrip(reloadedTrip.TripName).Count());
        }


        [Test]
        public void UpdateAllowedNumberOfPeople_WhenException_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdateNbPeopleException", 1000, maxNbPeople:30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            var mockTripRepo = new Mock<ITripRepository>();
            mockTripRepo.Setup(s => s.UpdateTrip(reloadedTrip)).Throws(new Exception("ExceptionForTest"));
            var secondServices = new TripServices(mockTripRepo.Object, new PotRepository(), new TripParticipantRepository(),
                                    new UserTripRepository(), new PotUserRepository());
            secondServices.UpdateAllowedNumberOfPeople(trip, 50);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(30, reloadedTrip.NumberMaxOfPeople);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(reloadedTrip.Price / 30, potUser.TargetAmount);

        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenError_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdateNbPeopleError", 1000, maxNbPeople: 30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            var mockTripRepo = new Mock<ITripRepository>();
            mockTripRepo.Setup(s => s.UpdateTrip(reloadedTrip)).Callback(() => new TripRepository().UpdateTrip(reloadedTrip));
            mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var secondServices = new TripServices(mockTripRepo.Object, new PotRepository(), new TripParticipantRepository(),
                                    new UserTripRepository(), new PotUserRepository());
            secondServices.UpdateAllowedNumberOfPeople(trip, 50);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(30, reloadedTrip.NumberMaxOfPeople);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(reloadedTrip.Price / 30, potUser.TargetAmount);
        }

        [Test]
        public void UpdateAllowedNumberOfPeople_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdateNbPeopleValid", 1000, maxNbPeople: 30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            services.UpdateAllowedNumberOfPeople(reloadedTrip, 50);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(50, reloadedTrip.NumberMaxOfPeople);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(reloadedTrip.Price / 50, potUser.TargetAmount);
        }

        [Test]
        public void UpdatePrice_WhenException_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdatePriceException", 1000, maxNbPeople: 30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            var mockTripRepo = new Mock<ITripRepository>();
            mockTripRepo.Setup(s => s.UpdateTrip(reloadedTrip)).Throws(new Exception("ExceptionForTest"));
            var secondServices = new TripServices(mockTripRepo.Object, new PotRepository(), new TripParticipantRepository(),
                                    new UserTripRepository(), new PotUserRepository());
            secondServices.UpdatePrice(trip, 2000.26);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1000, reloadedTrip.Price);
            Assert.IsNotNull(reloadedTrip.TripPot);
            Assert.AreEqual(1000, reloadedTrip.TripPot.TargetAmount);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(1000 / (double) reloadedTrip.NumberMaxOfPeople, potUser.TargetAmount);
            var userTrip = new UserTripRepository().GetUserTrip(1, trip.TripName);
            Assert.IsNotNull(userTrip);
            Assert.AreEqual(1000, userTrip.TripAmount);
        }

        [Test]
        public void UpdatePrice_WhenError_ShouldRollback()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdatePriceError", 1000, maxNbPeople: 30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            var mockTripRepo = new Mock<ITripRepository>();
            mockTripRepo.Setup(s => s.UpdateTrip(reloadedTrip)).Callback(() => new TripRepository().UpdateTrip(reloadedTrip));
            mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var secondServices = new TripServices(mockTripRepo.Object, new PotRepository(), new TripParticipantRepository(),
                                    new UserTripRepository(), new PotUserRepository());
            secondServices.UpdatePrice(trip, 2000.26);
            Assert.IsTrue(secondServices.HasErrors);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1000, reloadedTrip.Price);
            Assert.IsNotNull(reloadedTrip.TripPot);
            Assert.AreEqual(1000, reloadedTrip.TripPot.TargetAmount);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(1000 / (double) reloadedTrip.NumberMaxOfPeople, potUser.TargetAmount);
            var userTrip = new UserTripRepository().GetUserTrip(1, trip.TripName);
            Assert.IsNotNull(userTrip);
            Assert.AreEqual(1000, userTrip.TripAmount);
        }

        [Test]
        public void UpdatePrice_WhenValid_ShouldCommit()
        {
            var trip = ModelTestHelper.CreateTrip(-1, "UpdatePriceValid", 1000, maxNbPeople: 30);
            var services = new TripServices();
            services.CreateTrip(trip, 1);
            Assert.IsFalse(services.HasErrors);
            var reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(1, reloadedTrip.Participants.Count());
            services.UpdatePrice(reloadedTrip, 2000.26);
            reloadedTrip = services.GetTrip(trip.Id);
            Assert.IsNotNull(reloadedTrip);
            Assert.AreEqual(2000.26, reloadedTrip.Price);
            Assert.IsNotNull(reloadedTrip.TripPot);
            Assert.AreEqual(2000.26, reloadedTrip.TripPot.TargetAmount);
            var potUser = new PotUserRepository().GetPotUser(reloadedTrip.TripPot.Id, 1);
            Assert.IsNotNull(potUser);
            Assert.AreEqual(2000.26 / reloadedTrip.NumberMaxOfPeople, potUser.TargetAmount);
            var userTrip = new UserTripRepository().GetUserTrip(1, trip.TripName);
            Assert.IsNotNull(userTrip);
            Assert.AreEqual(2000.26, userTrip.TripAmount);
        }

        #endregion
    }
}
