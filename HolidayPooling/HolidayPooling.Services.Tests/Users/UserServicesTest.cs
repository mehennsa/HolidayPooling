using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Services.Users;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Tests.Users
{
    [TestFixture]
    public class UserServicesTest
    {

        #region Properties

        private IUserServices _userServices;
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IUserTripRepository> _mockTripRepo;
        private Mock<IFriendshipRepository> _mockFriendshipRepo;

        #endregion

        #region Methods

        private void CreateUserServices()
        {
            _userServices = new UserServices(_mockUserRepo.Object, _mockTripRepo.Object, _mockFriendshipRepo.Object);
        }

        private void CreateMocks()
        {
            _mockUserRepo = new Mock<IUserRepository> { CallBase = true };
            _mockTripRepo = new Mock<IUserTripRepository> { CallBase = true };
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
        public void CreateUser_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            CreateUserServices();
            _userServices.CreateUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void CreateUser_WhenRepoHasError_ShouldLogError()
        {
            const string error = "Error during save";
            var errorList = new List<string> { error };
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(errorList);
            CreateUserServices();
            _userServices.CreateUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
        }

        [Test]
        public void CreateUser_WhenValid_ShouldNotLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateUserServices();
            _userServices.CreateUser(user);
            Assert.IsFalse(_userServices.HasErrors);
        }

        [Test]
        public void UpdateUser_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            CreateUserServices();
            _userServices.UpdateUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void UpdateUser_WhenRepoHasError_ShouldLogError()
        {
            const string error = "Error during update";
            var errorList = new List<string> { error };
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(errorList);
            CreateUserServices();
            _userServices.UpdateUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
        }

        [Test]
        public void UpdateUser_WhenValid_ShouldNotLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "first");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateUserServices();
            _userServices.UpdateUser(user);
            Assert.IsFalse(_userServices.HasErrors);
        }

        [Test]
        public void DeleteUser_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void DeleteUser_WhenErrorFindingFriend_ShouldLogError()
        {
            var errors = new List<string> { "Friend not found" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => null);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(errors);
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.Setup(s => s.Errors).Returns(new List<string>());
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Friend not found"));
        }

        [Test]
        public void DeleteUser_WhenUserRepoHasErrorFindingFriends_ShouldLogError()
        {
            var errors = new List<string> { "repo in error" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.Errors).Returns(errors);
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.Setup(s => s.Errors).Returns(new List<string>());
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("repo in error"));
        }

        [Test]
        public void DeleteUser_WhenOtherFriendshipIsNull_ShouldLogError()
        {
            var errors = new List<string> { "error when searching for other friendship" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>());
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => null);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("error when searching for other friendship"));
        }

        [Test]
        public void DeleteUser_WhenRepoHasErrorsSearchingFriends_ShouldLogError()
        {
            var errors = new List<string> { "repo has error searching friendship" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>());
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => ModelTestHelper.CreateFriendship(2, user.Pseudo));
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("repo has error searching friendship"));
        }

        [Test]
        public void DeleteUser_WhenUnableToDeleteFriendship_ShouldLogError()
        {
            var errors = new List<string> { "unable to delete friendship" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>());
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => ModelTestHelper.CreateFriendship(2, user.Pseudo));
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(user.Friends.First())).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(true)
                );
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("unable to delete friendship"));
        }

        [Test]
        public void DeleteUser_WhenUnableToDeleteOtherFriendship_ShouldLogError()
        {
            var errors = new List<string> { "unable to delete other friendship" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));

            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>());

            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            var otherFriendship = ModelTestHelper.CreateFriendship(2, user.Pseudo);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => otherFriendship);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(errors);
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(user.Friends.First())).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );

            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(otherFriendship)).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(true)
                );
            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("unable to delete other friendship"));
        }

        [Test]
        public void DeleteUser_WhenUnableToDeleteUserTrip_ShouldLogError()
        {
            var errors = new List<string> { "unable to delete user's trip" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            user.AddTrip(ModelTestHelper.CreateUserTrip(user.Id, "First Trip"));

            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(new List<string>());

            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            var otherFriendship = ModelTestHelper.CreateFriendship(2, user.Pseudo);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => otherFriendship);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(user.Friends.First())).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );

            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(otherFriendship)).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );


            _mockTripRepo.Setup(s => s.DeleteUserTrip(user.Trips.First())).Callback
                (
                    () => _mockTripRepo.SetupGet(s => s.HasErrors).Returns(true)
                );
            _mockTripRepo.SetupGet(s => s.Errors).Returns(errors);

            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("unable to delete user's trip"));
        }


        [Test]
        public void DeleteUser_WhenUnableToDeleteUserItself_ShouldLogError()
        {
            var errors = new List<string> { "unable to delete user" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            user.AddTrip(ModelTestHelper.CreateUserTrip(user.Id, "First Trip"));

            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(errors);
            _mockUserRepo.Setup(s => s.DeleteUser(user)).Callback(() => _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true));

            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            var otherFriendship = ModelTestHelper.CreateFriendship(2, user.Pseudo);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => otherFriendship);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(user.Friends.First())).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );

            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(otherFriendship)).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );


            _mockTripRepo.Setup(s => s.DeleteUserTrip(user.Trips.First())).Callback
                (
                    () => _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false)
                );
            _mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());

            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("unable to delete user"));
        }

        [Test]
        public void DeleteUser_WhenValid_ShouldNotLogError()
        {
            var errors = new List<string> { "unable to delete user" };
            var user = ModelTestHelper.CreateUser(1, "first");
            user.AddFriend(ModelTestHelper.CreateFriendship(user.Id, "aFriend"));
            user.AddTrip(ModelTestHelper.CreateUserTrip(user.Id, "First Trip"));

            _mockUserRepo.Setup(s => s.GetUserInfo("aFriend")).Returns(() => ModelTestHelper.CreateUser(2, "aFriend"));
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.SetupGet(g => g.Errors).Returns(errors);
            _mockUserRepo.Setup(s => s.DeleteUser(user)).Callback(() => _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false));

            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            var otherFriendship = ModelTestHelper.CreateFriendship(2, user.Pseudo);
            _mockFriendshipRepo.Setup(s => s.GetFriendship(2, user.Pseudo)).Returns(() => otherFriendship);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(user.Friends.First())).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );

            _mockFriendshipRepo.Setup(s => s.DeleteFriendship(otherFriendship)).Callback
                (
                    () => _mockFriendshipRepo.Setup(s => s.HasErrors).Returns(false)
                );


            _mockTripRepo.Setup(s => s.DeleteUserTrip(user.Trips.First())).Callback
                (
                    () => _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false)
                );
            _mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());

            CreateUserServices();
            _userServices.DeleteUser(user);
            Assert.IsFalse(_userServices.HasErrors);
        }

        [Test]
        public void LoginByMail_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.Setup(s => s.GetUserByMail(user.Mail, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            CreateUserServices();
            _userServices.LoginByMail(user.Mail, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void LoginByMail_WhenFoundUserIsNull_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByMail(user.Mail, user.Password)).Returns(() => null);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Error user null" });
            CreateUserServices();
            _userServices.LoginByMail(user.Mail, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Error user null"));
        }

        [Test]
        public void LoginByMail_WhenError_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserRepo.Setup(s => s.GetUserByMail(user.Mail, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Error login by mail" });
            CreateUserServices();
            _userServices.LoginByMail(user.Mail, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Error login by mail"));
        }

        [Test]
        public void LoginByMail_WhenUnableToFillUser_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByMail(user.Mail, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception while finding friendship"));
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.Setup(s => s.GetUserTrips(user.Id)).Returns(new List<UserTrip>());
            CreateUserServices();
            var dbUser = _userServices.LoginByMail(user.Mail, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsNull(dbUser);
            Assert.IsTrue(_userServices.Errors.Contains("Unable to find user information"));
        }

        [Test]
        public void LoginByMail_WhenValid_ShouldNotLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByMail(user.Mail, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(user.Id))
                .Returns(new List<Friendship> { ModelTestHelper.CreateFriendship(user.Id, "aFriend") });
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.Setup(s => s.GetUserTrips(user.Id))
                .Returns(new List<UserTrip> { ModelTestHelper.CreateUserTrip(user.Id, "aTrip")});
            CreateUserServices();
            var dbUser = _userServices.LoginByMail(user.Mail, user.Password);
            Assert.IsFalse(_userServices.HasErrors);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual(1, dbUser.Friends.Count());
            Assert.AreEqual(1, dbUser.Trips.Count());
        }

        [Test]
        public void LoginByPseudo_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "aPseudo");
            _mockUserRepo.Setup(s => s.GetUserByPseudo(user.Pseudo, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            CreateUserServices();
            _userServices.LoginByPseudo(user.Pseudo, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void LoginByPseudo_WhenFoundUserIsNull_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "aPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByPseudo(user.Pseudo, user.Password)).Returns(() => null);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Error user null" });
            CreateUserServices();
            _userServices.LoginByPseudo(user.Pseudo, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Error user null"));
        }

        [Test]
        public void LoginByPseudo_WhenError_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserRepo.Setup(s => s.GetUserByPseudo(user.Pseudo, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { "Error login by pseudo" });
            CreateUserServices();
            _userServices.LoginByPseudo(user.Pseudo, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Error login by pseudo"));
        }

        [Test]
        public void LoginByPseudo_WhenUnableToFillUser_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "aPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByPseudo(user.Pseudo, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockTripRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception while finding trips"));
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(user.Id)).Returns(new List<Friendship>());
            CreateUserServices();
            var dbUser = _userServices.LoginByPseudo(user.Pseudo, user.Password);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsNull(dbUser);
            Assert.IsTrue(_userServices.Errors.Contains("Unable to find user information"));
        }

        [Test]
        public void LoginByPseudo_WhenValid_ShouldNotLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "aPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserByPseudo(user.Pseudo, user.Password)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            CreateUserServices();
            var dbUser = _userServices.LoginByPseudo(user.Pseudo, user.Password);
            Assert.IsFalse(_userServices.HasErrors);
            Assert.IsNotNull(dbUser);
        }

        [Test]
        public void GetUserTrips_WhenException_ShouldLogError()
        {
            _mockTripRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            _mockTripRepo.Setup(s => s.GetUserTrips(1)).Returns(new List<UserTrip>());
            CreateUserServices();
            var list = _userServices.GetUserTrips(1);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void GetUserTrips_WhenError_ShouldLogError()
        {
            const string error = "Error while finding trips";
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string> { error });
            CreateUserServices();
            var list = _userServices.GetUserTrips(1);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
        }

        [Test]
        public void GetUserTrips_WhenValid_ShouldNotLogError()
        {
            _mockTripRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockTripRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockTripRepo.Setup(s => s.GetUserTrips(1))
                .Returns(new List<UserTrip> { ModelTestHelper.CreateUserTrip(1, "one"), ModelTestHelper.CreateUserTrip(1, "two") });
            CreateUserServices();
            var list = _userServices.GetUserTrips(1);
            Assert.IsFalse(_userServices.HasErrors);
            Assert.AreEqual(2, list.Count());
        }

        [Test]
        public void GetUserFriendships_WhenException_ShouldLogError()
        {
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception For Test"));
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(1)).Returns(new List<Friendship>());
            CreateUserServices();
            var list = _userServices.GetUserFriendships(1);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception For Test"));
        }

        [Test]
        public void GetUserFriendship_WhenError_ShouldLogError()
        {
            const string error = "Error while finding friendships";
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string> { error });
            CreateUserServices();
            var list = _userServices.GetUserFriendships(1);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
        }

        [Test]
        public void GetUserFriendship_WhenValid_ShouldNotLogError()
        {
            _mockFriendshipRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockFriendshipRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            _mockFriendshipRepo.Setup(s => s.GetUserFriendships(1))
                .Returns(new List<Friendship> { ModelTestHelper.CreateFriendship(1, "one"), ModelTestHelper.CreateFriendship(1, "two") });
            CreateUserServices();
            var list = _userServices.GetUserFriendships(1);
            Assert.IsFalse(_userServices.HasErrors);
            Assert.AreEqual(2, list.Count());
        }

        [Test]
        public void GetUserInfo_WhenException_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(1, "aUserPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Throws(new Exception("Exception for test"));
            _mockUserRepo.Setup(s => s.GetUserInfo(user.Pseudo)).Returns(user);
            CreateUserServices();
            var other = _userServices.GetUserInfo(user.Pseudo);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains("Exception for test"));
            Assert.IsNull(other);
        }

        [Test]
        public void GetUserInfo_WhenUserFoundIsNull_ShouldLogError()
        {
            const string error = "User not found";
            var user = ModelTestHelper.CreateUser(1, "aUserPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserInfo(user.Pseudo)).Returns(() => null);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { error });
            CreateUserServices();
            var other = _userServices.GetUserInfo(user.Pseudo);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
            Assert.IsNull(other);
        }

        [Test]
        public void GetUserInfo_WhenError_ShouldLogError()
        {
            const string error = "error during load";
            var user = ModelTestHelper.CreateUser(1, "aUserPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockUserRepo.Setup(s => s.GetUserInfo(user.Pseudo)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string> { error });
            CreateUserServices();
            var other = _userServices.GetUserInfo(user.Pseudo);
            Assert.IsTrue(_userServices.HasErrors);
            Assert.IsTrue(_userServices.Errors.Contains(error));
            Assert.IsNull(other);
        }

        [Test]
        public void GetUserInfo_WhenValid_ShouldReturnValidUser()
        {
            var user = ModelTestHelper.CreateUser(1, "aUserPseudo");
            _mockUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockUserRepo.Setup(s => s.GetUserInfo(user.Pseudo)).Returns(user);
            _mockUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            CreateUserServices();
            var other = _userServices.GetUserInfo(user.Pseudo);
            Assert.IsFalse(_userServices.HasErrors);
            Assert.IsNotNull(other);
        }

        #endregion

    }
}
