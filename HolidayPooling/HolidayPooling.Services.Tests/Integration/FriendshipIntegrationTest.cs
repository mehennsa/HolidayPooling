using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Services.Friendships;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HolidayPooling.Services.Tests.Integration
{
    [TestFixture]
    public class FriendshipIntegrationTest : BaseIntegrationTest
    {
        #region BaseIntegrationTest

        protected override IEnumerable<string> TableUsed
        {
            get
            {
                yield return "TUSR";
                yield return "TFRDSHP";
            }
        }

        #endregion

        #region Tests

        [Test]
        public void RequestFriendship_WhenException_ShouldRollback()
        {

            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friendshipRepo = new FriendshipRepository();
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(s => s.HasErrors).Throws(new Exception("For Rollback"));
            var service = new FriendshipServices(mock.Object, friendshipRepo);
            service.RequestFriendship(friendship, "aFriend");
            Assert.IsNull(friendshipRepo.GetFriendship(friendship.UserId, friendship.FriendName));

        }

        [Test]
        public void RequestFriendship_WhenErrorDuringSave_ShouldRollback()
        {

            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friendshipRepo = new FriendshipRepository();
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(s => s.HasErrors).Returns(true);
            mock.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            var service = new FriendshipServices(mock.Object, friendshipRepo);
            service.RequestFriendship(friendship, "aFriend");
            Assert.IsNull(friendshipRepo.GetFriendship(friendship.UserId, friendship.FriendName));

        }

        [Test]
        public void RequestFriendship_WhenValid_ShouldCommit()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var dbFriendship = service.GetFriendship(firstUser.Id, secondUser.Pseudo);
            Assert.IsNotNull(dbFriendship);
            var otherFriendship = service.GetFriendship(secondUser.Id, firstUser.Pseudo);
            Assert.IsNotNull(otherFriendship);
            Assert.IsFalse(otherFriendship.IsRequested);
            Assert.IsTrue(otherFriendship.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenException_ShouldRollback()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(u => u.HasErrors).Throws(new Exception("Rollback"));
            var mockService = new FriendshipServices(mock.Object, new FriendshipRepository());
            mockService.AcceptFriendship(friendship, firstUser.Pseudo);
            Assert.IsTrue(mockService.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsTrue(dbFriendship.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenError_ShouldRollback()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(u => u.HasErrors).Returns(false);
            mock.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            var mockService = new FriendshipServices(mock.Object, new FriendshipRepository());
            mockService.AcceptFriendship(friendship, firstUser.Pseudo);
            Assert.IsTrue(mockService.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsTrue(dbFriendship.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenValid_ShouldCommit()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            service.AcceptFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsNotNull(dbFriendship);
            Assert.IsFalse(dbFriendship.IsWaiting);
            var otherFriendship = service.GetFriendship(secondUser.Id, firstUser.Pseudo);
            Assert.IsNotNull(otherFriendship);
            Assert.IsFalse(dbFriendship.IsWaiting);
        }


        [Test]
        public void DenyFriendship_WhenException_ShouldRollback()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(u => u.HasErrors).Throws(new Exception("Rollback"));
            var mockService = new FriendshipServices(mock.Object, new FriendshipRepository());
            mockService.DenyFriendship(friendship, firstUser.Pseudo);
            Assert.IsTrue(mockService.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsNotNull(dbFriendship.IsWaiting);
        }

        [Test]
        public void DenyFriendship_WhenError_ShouldRollback()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var mock = new Mock<IUserRepository>();
            mock.SetupGet(u => u.HasErrors).Returns(false);
            mock.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            var mockService = new FriendshipServices(mock.Object, new FriendshipRepository());
            mockService.DenyFriendship(friendship, firstUser.Pseudo);
            Assert.IsTrue(mockService.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsNotNull(dbFriendship);
        }

        [Test]
        public void DenyFriendship_WhenValid_ShouldCommit()
        {
            var firstUser = ModelTestHelper.CreateUser(-1, "aUser");
            var secondUser = ModelTestHelper.CreateUser(-1, "aFriend", "secondEmail");
            var userRepo = new UserRepository();
            userRepo.SaveUser(firstUser);
            Assert.IsFalse(userRepo.HasErrors);
            userRepo.SaveUser(secondUser);
            Assert.IsFalse(userRepo.HasErrors);
            var friendship = ModelTestHelper.CreateFriendship(firstUser.Id, secondUser.Pseudo, DateTime.Now, true, true);
            var service = new FriendshipServices();
            service.RequestFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            service.DenyFriendship(friendship, firstUser.Pseudo);
            Assert.IsFalse(service.HasErrors);
            var dbFriendship = service.GetFriendship(friendship.UserId, friendship.FriendName);
            Assert.IsNull(dbFriendship);
            var otherFriendship = service.GetFriendship(secondUser.Id, firstUser.Pseudo);
            Assert.IsNull(otherFriendship);
        }

        #endregion

    }
}
