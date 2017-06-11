using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Services.Friendships;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.Services.Tests.Friendships
{

    [TestFixture]
    public class FriendshipServicesTest
    {

        #region Fields

        private IFriendshipServices _friendshipServices;
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IFriendshipRepository> _mockFriendshipRepo;

        #endregion

        #region Methods

        private void CreateFriendshipServices()
        {
            _friendshipServices = new FriendshipServices(_mockUserRepo.Object, _mockFriendshipRepo.Object);
        }

        private void CreateMocks()
        {
            _mockUserRepo = new Mock<IUserRepository> { CallBase = true };
            _mockFriendshipRepo = new Mock<IFriendshipRepository> { CallBase = true };
        }

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            CreateMocks();
        }

        #endregion

        #region Tests

        [Test]
        public void AcceptFriendship_WhenException_ShouldLogError()
        {
            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for Test"));
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendship, "toto");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Exception for Test"));
            Assert.IsTrue(friendship.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenUnableToUpdateFriendship_ShouldLogError()
        {
            const string error = "Unable to update current friendship";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "UpdateFailed");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "anUpdateFailed");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenUnableToFindOtherFriendAccount_ShouldLogError()
        {
            const string error = "Unable to find friend account";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "WrongOtherUser");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "wrongUser");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenFriendAccountIsNull_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "AccountNull");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "AccountNull");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find friend's account"));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenUnableToFindOtherFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            const string error = "Unable to update current friendship";
            var errors = new List<string> { error };
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Callback
                (
                    () =>
                    {
                        _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
                        _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);

                    }
                );
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenOtherFriendshipIsNull_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => null);
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to retrieve friendship"));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_WhenUnableToUpdateOtherFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            _mockFriendshipRepo.Setup(s => s.UpdateFrendship(otherFriendship)).Callback
                (
                    () =>
                    {
                        _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
                    }
                );
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to update other friendship"));
            Assert.IsTrue(friendshp.IsWaiting);
        }

        [Test]
        public void AcceptFriendship_ShouldNotFail()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            CreateFriendshipServices();
            _friendshipServices.AcceptFriendship(friendshp, "me");
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.IsFalse(friendshp.IsWaiting);
        }

        [Test]
        public void DenyFriendship_WhenException_ShouldLogError()
        {
            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for Test"));
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendship, "toto");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Exception for Test"));
        }

        [Test]
        public void DenyFriendship_WhenUnableToDeleteFriendship_ShouldLogError()
        {
            const string error = "Unable to delete current friendship";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "deleteFailed");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "aDeleteFailed");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void DenyFriendship_WhenUnableToFindOtherFriendAccount_ShouldLogError()
        {
            const string error = "Unable to find friend account";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "WrongOtherUser");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "wrongUser");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void DenyFriendship_WhenFriendAccountIsNull_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "AccountNull");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "AccountNull");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find friend's account"));
        }

        [Test]
        public void DenyFriend_WhenUnableToFindOtherFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            const string error = "Unable to delete current friendship";
            var errors = new List<string> { error };
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Callback
                (
                    () =>
                    {
                        _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
                        _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);

                    }
                );
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void DenyFriendship_WhenOtherFriendshipIsNull_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => null);
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find friend's friendship"));
        }

        [Test]
        public void DenyFrienship_WhenUnableToDeleteOtherFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(otherFriendship)).Callback
                (
                    () =>
                    {
                        _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
                        _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Unable to delete other friendship" });
                    }
                );
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to delete other friendship"));
        }

        [Test]
        public void DenyFriendship_ShouldNotFail()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            CreateFriendshipServices();
            _friendshipServices.DenyFriendship(friendshp, "me");
            Assert.IsFalse(_friendshipServices.HasErrors);
        }

        [Test]
        public void RequestFriendship_WhenException_ShouldLogError()
        {
            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for Test"));
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendship, "toto");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Exception for Test"));
        }

        [Test]
        public void RequestFriendship_WhenUnableToDeleteFriendship_ShouldLogError()
        {
            const string error = "Unable to save current friendship";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "saveFailed");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendshp, "aSaveFailed");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void RequestFriendship_WhenUnableToFindOtherFriendAccount_ShouldLogError()
        {
            const string error = "Unable to find friend account";
            var errors = new List<string> { error };
            var friendshp = ModelTestHelper.CreateFriendship(1, "WrongOtherUser");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(errors);
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendshp, "wrongUser");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void RequestFriendship_WhenFriendAccountIsNull_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "AccountNull");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => null);
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendshp, "AccountNull");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find friend's account"));
        }

        [Test]
        public void RequestFriendship_WhenUnableToCloneFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var mockFriendship = new Mock<Friendship> { CallBase = true };
            mockFriendship.Setup(f => f.Clone()).Returns(() => null);
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            const string error = "Unexpected error occured, please call support";
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);

            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(mockFriendship.Object, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
        }

        [Test]
        public void RequestFriendship_WhenUnableToSaveOtherFriendship_ShouldLogError()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            _mockFriendshipRepo.Setup(s => s.SaveFriendship(otherFriendship)).Callback
                (
                    () =>
                    {
                        _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
                        _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Unable to save other friendship" });
                    }
                );
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendshp, "me");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to save other friendship"));
        }

        [Test]
        public void RequestFriendship_ShouldNotFail()
        {
            var friendshp = ModelTestHelper.CreateFriendship(1, "aFriend");
            var friend = ModelTestHelper.CreateUser(2, "aFriend");
            var otherFriendship = ModelTestHelper.CreateFriendship(2, "me");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>(0));
            _mockUserRepo.Setup(s => s.GetUserInfo(It.IsAny<string>())).Returns(() => friend);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, "me")).Returns(() => otherFriendship);
            CreateFriendshipServices();
            _friendshipServices.RequestFriendship(friendshp, "me");
            Assert.IsFalse(_friendshipServices.HasErrors);
        }

        [Test]
        public void GetFriendships_WhenException_ShouldReturnEmptyList()
        {
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for Test"));
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships(1);
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Exception for Test"));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetFriendships_WhenUnableToRetrieveFriendships_ShouldReturnEmptyList()
        {
            const string error = "Unable to retrieve friendships";
            var errors = new List<string> { error };
            _mockFriendshipRepo.SetupGet(f => f.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(f => f.Errors).Returns(errors);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships(1);
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetFriendships_ShouldReturnValidRecord()
        {
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(1)).Returns
                (
                    new List<Friendship>
                    {
                        ModelTestHelper.CreateFriendship(1, "first"),
                        ModelTestHelper.CreateFriendship(1, "second")
                    }
                );
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships(1);
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "first"));
            Assert.IsTrue(list.Any(f => f.FriendName == "second"));
        }

        [Test]
        public void GetRequestedFriendships_ShouldReturnValidRecord()
        {
            _mockFriendshipRepo.Setup(s => s.GetRequestedFriendships(1)).Returns
                (
                    new List<Friendship>
                    {
                        ModelTestHelper.CreateFriendship(1, "first"),
                        ModelTestHelper.CreateFriendship(1, "second")
                    }
                );
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateFriendshipServices();
            var list = _friendshipServices.GetRequestedFriendships(1);
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "first"));
            Assert.IsTrue(list.Any(f => f.FriendName == "second"));
        }

        [Test]
        public void GetWaitingFriendships_ShouldReturnValidRecord()
        {
            _mockFriendshipRepo.Setup(s => s.GetWaitingFriendships(1)).Returns
                (
                    new List<Friendship>
                    {
                        ModelTestHelper.CreateFriendship(1, "first"),
                        ModelTestHelper.CreateFriendship(1, "second")
                    }
                );
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateFriendshipServices();
            var list = _friendshipServices.GetWaitingFriendships(1);
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "first"));
            Assert.IsTrue(list.Any(f => f.FriendName == "second"));
        }

        [Test]
        public void GetFriendshipsByPseudo_WhenException_ShouldReturnEmptyList()
        {
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for Test"));
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships("exception");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Exception for Test"));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetFriendshipsByPseudo_WhenUnableToFindUserAccount_ShouldReturnEmptyList()
        {
            const string error = "Unable to find acount";
            var errors = new List<string> { error };
            _mockUserRepo.SetupGet(u => u.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(u => u.Errors).Returns(errors);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships("UnableToFindAccount");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.AreEqual(0, list.Count());

        }

        [Test]
        public void GetFriendshipsByPseudo_WhenUserAccountIsNull_ShouldReturnEmptyList()
        {
            _mockUserRepo.SetupGet(u => u.HasErrors).Returns(false);
            _mockUserRepo.Setup(u => u.GetUserInfo("AccountNull")).Returns(() => null);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships("AccountNull");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find user's account"));
            Assert.AreEqual(0, list.Count());

        }

        [Test]
        public void GetFriendshipsByPseudo_WhenUnableToRetrieveFriendships_ShouldReturnEmptyList()
        {
            var user = ModelTestHelper.CreateUser(1, "aUser");
            const string error = "Unable to retrieve friendships";
            var errors = new List<string> { error };
            _mockUserRepo.SetupGet(u => u.HasErrors).Returns(false);
            _mockUserRepo.Setup(u => u.GetUserInfo(user.Pseudo)).Returns(() => user);
            _mockFriendshipRepo.SetupGet(f => f.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(f => f.Errors).Returns(errors);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships(user.Pseudo);
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetFriendshipsByPseudo_ShouldReturnValidRecord()
        {
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(1)).Returns
                (
                    new List<Friendship>
                    {
                        ModelTestHelper.CreateFriendship(1, "first"),
                        ModelTestHelper.CreateFriendship(1, "second")
                    }
                );
            var user = ModelTestHelper.CreateUser(1, "aUser");
            _mockUserRepo.SetupGet(u => u.HasErrors).Returns(false);
            _mockUserRepo.Setup(u => u.GetUserInfo(user.Pseudo)).Returns(() => user);
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateFriendshipServices();
            var list = _friendshipServices.GetUserFriendships(user.Pseudo);
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "first"));
            Assert.IsTrue(list.Any(f => f.FriendName == "second"));
        }

        [Test]
        public void GetFriendship_WhenException_ShouldReturnNullAndLogError()
        {
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("ExceptionForTest"));
            CreateFriendshipServices();
            var friendship = _friendshipServices.GetFriendship(1, "exception");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("ExceptionForTest"));
            Assert.IsNull(friendship);
        }

        [Test]
        public void GetFriendship_WhenErrorDuringRetrieval_ShouldReturnNullAndLogError()
        {
            const string error = "Error during retrieval";
            var errors = new List<string> { error };
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateFriendshipServices();
            var friendship = _friendshipServices.GetFriendship(1, "error");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains(error));
            Assert.IsNull(friendship);
        }

        [Test]
        public void GetFriendship_WhenFriendshipIsNull_ShouldReturnNullAndLogError()
        {
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockFriendshipRepo.Setup(s => s.GetFriendship(1, "null")).Returns(() => null);
            CreateFriendshipServices();
            var friendship = _friendshipServices.GetFriendship(1, "null");
            Assert.IsTrue(_friendshipServices.HasErrors);
            Assert.IsTrue(_friendshipServices.Errors.Contains("Unable to find friendship"));
            Assert.IsNull(friendship);
        }

        [Test]
        public void GetFriendship_WhenValid_ShouldReturnFriendship()
        {
            var friendship = ModelTestHelper.CreateFriendship(1, "aFriend");
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>(0));
            _mockFriendshipRepo.Setup(s => s.GetFriendship(1, "aFriend")).Returns(friendship);
            CreateFriendshipServices();
            var dbFriendship = _friendshipServices.GetFriendship(1, "aFriend");
            Assert.IsFalse(_friendshipServices.HasErrors);
            Assert.IsNotNull(dbFriendship);
        }

        #endregion

    }
}
