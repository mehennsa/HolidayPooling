using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Services.Users;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Tests.Integration
{
    [TestFixture]
    public class UserServicesIntegrationTest : BaseIntegrationTest
    {

        #region BaseIntegrationTest

        protected override IEnumerable<string> TableUsed
        {
            get
            {
                yield return "TUSR";
                yield return "TFRDSHP";
                yield return "TUSRTRP";
            }
        }

        #endregion

        #region Tests

        [Test]
        public void CreateUser_WhenException_ShouldRollback()
        {
            var user = ModelTestHelper.CreateUser(-1, "Rollbackuser");
            var mock = new Mock<IUserRepository>();
            var userRepo = new UserRepository();
            mock.SetupGet(s => s.HasErrors).Throws(new Exception("Rollback exception"));
            mock.Setup(s => s.SaveUser(user)).Callback(() => userRepo.SaveUser(user));
            var service = new UserServices(mock.Object, new UserTripRepository(), new FriendshipRepository());
            service.CreateUser(user);
            Assert.IsNull(service.GetUserInfo(user.Pseudo));
        }

        [Test]
        public void CreateUser_WhenValid_ShouldCommit()
        {
            var user = ModelTestHelper.CreateUser(-1, "CommitUser");
            var service = new UserServices();
            service.CreateUser(user);
            Assert.IsNotNull(service.GetUserInfo(user.Pseudo));
        }

        [Test]
        public void UpdateUser_WhenException_ShouldRollback()
        {
            var user = ModelTestHelper.CreateUser(-1, "Rollbackuser");
            var mock = new Mock<IUserRepository>();
            var userRepo = new UserRepository();
            userRepo.SaveUser(user);
            Assert.IsFalse(userRepo.HasErrors);
            mock.SetupGet(s => s.HasErrors).Throws(new Exception("Rollback exception"));
            mock.Setup(s => s.UpdateUser(user)).Callback(() => userRepo.UpdateUser(user));
            var service = new UserServices(mock.Object, new UserTripRepository(), new FriendshipRepository());
            var oldNumber = user.PhoneNumber;
            user.PhoneNumber = "UpdatedNumber";
            service.UpdateUser(user);
            service = new UserServices();
            var dbUser = service.GetUserInfo(user.Pseudo);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual(oldNumber, dbUser.PhoneNumber);
        }

        [Test]
        public void UpdateUser_WhenValid_ShouldCommit()
        {
            var user = ModelTestHelper.CreateUser(-1, "CommitUser");
            var service = new UserServices();
            var userRepo = new UserRepository();
            userRepo.SaveUser(user);
            Assert.IsFalse(userRepo.HasErrors);
            var oldNumber = user.PhoneNumber;
            user.PhoneNumber = "UpdatedNumber";
            service.UpdateUser(user);
            var dbUser = service.GetUserInfo(user.Pseudo);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual("UpdatedNumber", dbUser.PhoneNumber);
        }

        [Test]
        public void Delete_WhenException_ShouldRollBack()
        {
            var userRepo = new UserRepository();
            var friendshipRepo = new FriendshipRepository();
            var tripRepo = new UserTripRepository();

            var user = ModelTestHelper.CreateUser(1, "DeleteUser");
            var pwd = user.Password;
            userRepo.SaveUser(user);
            Assert.IsFalse(userRepo.HasErrors);
            var secondUser = ModelTestHelper.CreateUser(2, "AFriend", "AFriendMail");
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);

            var friendship = ModelTestHelper.CreateFriendship(user.Id, secondUser.Pseudo);
            var otherFriendship = ModelTestHelper.CreateFriendship(secondUser.Id, user.Pseudo);
            friendshipRepo.SaveFriendship(friendship);
            friendshipRepo.SaveFriendship(otherFriendship);
            Assert.IsFalse(friendshipRepo.HasErrors);
            user.AddFriend(friendship);
            secondUser.AddFriend(otherFriendship);

            var trip = ModelTestHelper.CreateUserTrip(user.Id, "ATrip");
            tripRepo.SaveUserTrip(trip);
            Assert.IsFalse(tripRepo.HasErrors);
            user.AddTrip(trip);

            var mock = new Mock<IUserTripRepository>();
            mock.Setup(s => s.DeleteUserTrip(trip)).Callback(() => tripRepo.DeleteUserTrip(trip));
            mock.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for rollback"));
            var service = new UserServices(new UserRepository(), mock.Object, new FriendshipRepository());
            service.DeleteUser(user);
            service = new UserServices();
            var dbUser = service.LoginByPseudo(user.Pseudo, pwd);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual(1, dbUser.Friends.Count());
            Assert.AreEqual(1, dbUser.Trips.Count());
            Assert.AreEqual(1, service.GetUserTrips(user.Id).Count());
            Assert.AreEqual(1, service.GetUserFriendships(user.Id).Count());
            Assert.AreEqual(1, service.GetUserFriendships(secondUser.Id).Count(f => f.FriendName == user.Pseudo));
        }

        [Test]
        public void Delete_WhenErrorDuringDelete_ShouldRollBack()
        {
            var userRepo = new UserRepository();
            var friendshipRepo = new FriendshipRepository();
            var tripRepo = new UserTripRepository();

            var user = ModelTestHelper.CreateUser(1, "DeleteUser");
            var pwd = user.Password;
            userRepo.SaveUser(user);
            Assert.IsFalse(userRepo.HasErrors);
            var secondUser = ModelTestHelper.CreateUser(2, "AFriend", "AFriendMail");
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);

            var friendship = ModelTestHelper.CreateFriendship(user.Id, secondUser.Pseudo);
            var otherFriendship = ModelTestHelper.CreateFriendship(secondUser.Id, user.Pseudo);
            friendshipRepo.SaveFriendship(friendship);
            friendshipRepo.SaveFriendship(otherFriendship);
            Assert.IsFalse(friendshipRepo.HasErrors);
            user.AddFriend(friendship);
            secondUser.AddFriend(otherFriendship);

            var trip = ModelTestHelper.CreateUserTrip(user.Id, "ATrip");
            tripRepo.SaveUserTrip(trip);
            Assert.IsFalse(tripRepo.HasErrors);
            user.AddTrip(trip);

            var mock = new Mock<IUserTripRepository>();
            mock.Setup(s => s.DeleteUserTrip(trip)).Callback(() => tripRepo.DeleteUserTrip(trip));
            mock.SetupGet(s => s.HasErrors).Returns(true);
            var service = new UserServices(new UserRepository(), mock.Object, new FriendshipRepository());
            service.DeleteUser(user);
            service = new UserServices();
            var dbUser = service.LoginByPseudo(user.Pseudo, pwd);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual(1, dbUser.Friends.Count());
            Assert.AreEqual(1, dbUser.Trips.Count());
            Assert.AreEqual(1, service.GetUserTrips(user.Id).Count());
            Assert.AreEqual(1, service.GetUserFriendships(user.Id).Count());
            Assert.AreEqual(1, service.GetUserFriendships(secondUser.Id).Count(f => f.FriendName == user.Pseudo));
        }

        [Test]
        public void Delete_WhenValid_ShouldCommit()
        {
            var userRepo = new UserRepository();
            var friendshipRepo = new FriendshipRepository();
            var tripRepo = new UserTripRepository();

            var user = ModelTestHelper.CreateUser(1, "DeleteUser");
            userRepo.SaveUser(user);
            Assert.IsFalse(userRepo.HasErrors);
            var secondUser = ModelTestHelper.CreateUser(2, "AFriend", "AFriendMail");
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);

            var friendship = ModelTestHelper.CreateFriendship(user.Id, secondUser.Pseudo);
            var otherFriendship = ModelTestHelper.CreateFriendship(secondUser.Id, user.Pseudo);
            friendshipRepo.SaveFriendship(friendship);
            friendshipRepo.SaveFriendship(otherFriendship);
            Assert.IsFalse(friendshipRepo.HasErrors);
            user.AddFriend(friendship);
            secondUser.AddFriend(otherFriendship);

            var trip = ModelTestHelper.CreateUserTrip(user.Id, "ATrip");
            tripRepo.SaveUserTrip(trip);
            Assert.IsFalse(tripRepo.HasErrors);
            user.AddTrip(trip);

            var service = new UserServices();
            service.DeleteUser(user);
            Assert.IsNull(service.GetUserInfo(user.Pseudo));
            Assert.AreEqual(0, service.GetUserTrips(user.Id).Count());
            Assert.AreEqual(0, service.GetUserFriendships(user.Id).Count());
            Assert.AreEqual(0, service.GetUserFriendships(secondUser.Id).Count(f => f.FriendName == user.Pseudo));
        }

        #endregion
    }
}
