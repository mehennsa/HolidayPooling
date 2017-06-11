using HolidayPooling.DataRepositories.Business;
using HolidayPooling.DataRepositories.Core;
using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.DataRepositories.Tests.Repository
{
    [TestFixture]
    public class UserTripRepositoryTest : RepositoryTestBase<IUserTripDbImportExport, IUserTripRepository>
    {

        #region Constants

        private const string NullUserTripErrorMessage = "Please provide valid user trip information";
        
        private const string SaveFailed = "Internal Error : Unable to save user's trip";

        private const string DeleteFailed = "Internal Error : Unable to delete user's trip";

        private const string UpdateFailed = "Internal Error : Unable to update user's trip";

        #endregion

        #region RepositoryTestBase<UserTripDbImportExport, IUserTripRepository>

        protected override IUserTripRepository CreateRepository(IUserTripDbImportExport persister)
        {
            return new UserTripRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void SaveUserTrip_WhenUserTripNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SaveUserTrip(null);
            CheckErrors(repo, NullUserTripErrorMessage);
        }

        [Test]
        public void SaveUserTrip_WhenExceptionThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<UserTrip>())).Throws(new ImportExportException("Exception For Test"));
            var repo = CreateRepository(mock.Object);
            repo.SaveUserTrip(new UserTrip());
            CheckErrors(repo, "Exception For Test");
        }

        [Test]
        public void SaveUserTrip_WhenDbInsertFail_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<UserTrip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.SaveUserTrip(new UserTrip());
            CheckErrors(repo, SaveFailed);
        }

        [Test]
        public void SaveUserTrip_WhenValid_ShouldNotSetErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<UserTrip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var userTrip = ModelTestHelper.CreateUserTrip(1, "MyTrip");
            repo.SaveUserTrip(userTrip);
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void UpdateUserTrip_WhenUserTripIsNull_ShouldLogErrors()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdateUserTrip(null);
            CheckErrors(repo, NullUserTripErrorMessage);
        }

        [Test]
        public void UpdateUserTrip_WhenExceptionIsThrown_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<UserTrip>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdateUserTrip(new UserTrip());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void UpdateUserTrip_WhenUpdateInDbFails_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<UserTrip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdateUserTrip(new UserTrip());
            CheckErrors(repo, UpdateFailed);
        }

        [Test]
        public void UpdateUserTrip_WhenValid_ShouldNotSetErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<UserTrip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var userTrip = ModelTestHelper.CreateUserTrip(1, "UpdateTest");
            repo.UpdateUserTrip(userTrip);
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void DeleteUserTrip_WhenUserTripIsNull_ShouldLogErrors()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeleteUserTrip(null);
            CheckErrors(repo, NullUserTripErrorMessage);
        }

        [Test]
        public void DeleteUserTrip_WhenExceptionIsThrown_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<UserTrip>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeleteUserTrip(new UserTrip());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void DeleteUserTrip_WhenDeleteInDbFails_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<UserTrip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeleteUserTrip(new UserTrip());
            CheckErrors(repo, DeleteFailed);
        }

        [Test]
        public void DeleteUserTrip_WhenValid_ShouldNotSetErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<UserTrip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var userTrip = ModelTestHelper.CreateUserTrip(2, "DeleteTrip");
            repo.DeleteUserTrip(userTrip);
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void GetUserTrip_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<UserTripKey>())).Throws(new ImportExportException("ExceptionForGetUserTripTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserTrip(1, "mockTrip");
            CheckErrors(repo, "ExceptionForGetUserTripTest");
        }

        [Test]
        public void GetUserTrip_WhenValid_ShouldNotSetError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<UserTripKey>())).Returns(ModelTestHelper.CreateUserTrip(1, "GetUserTripTest"));
            var repo = CreateRepository(mock.Object);
            var userTrip = repo.GetUserTrip(1, "GetUserTripTest");
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(userTrip);
            Assert.AreEqual(1, userTrip.UserId);
            Assert.AreEqual("GetUserTripTest", userTrip.TripName);
        }

        [Test]
        public void GetUserTrips_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetTripForUser(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetUserTripsTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserTrips(-1);
            CheckErrors(repo, "ExceptionForGetUserTripsTest");
        }

        [Test]
        public void GetUserTrips_WhenValid_ShouldNotSetErrors()
        {
            var list = new List<UserTrip> { ModelTestHelper.CreateUserTrip(1, "Trip1"), ModelTestHelper.CreateUserTrip(1, "Trip2") };
            var mock = CreateMock();
            mock.Setup(s => s.GetTripForUser(It.IsAny<int>())).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetUserTrips(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(u => u.TripName == "Trip1"));
            Assert.IsTrue(dbList.Any(u => u.TripName == "Trip2"));
        }

        [Test]
        public void GetUserTripsByTrip_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserTripsByTrip(It.IsAny<string>())).Throws(new ImportExportException("ExceptionForGetUserTripsByTripTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserTripsByTrip("exceptionTripn");
            CheckErrors(repo, "ExceptionForGetUserTripsByTripTest");
        }

        [Test]
        public void GetUserTripsByTrip_WhenValid_SHouldNotSetErrors()
        {
            var list = new List<UserTrip> { ModelTestHelper.CreateUserTrip(1, "Trip1"), ModelTestHelper.CreateUserTrip(2, "Trip1") };
            var mock = CreateMock();
            mock.Setup(s => s.GetUserTripsByTrip("Trip1")).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetUserTripsByTrip("Trip1");
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(u => u.UserId == 1));
            Assert.IsTrue(dbList.Any(u => u.UserId == 2));
        }

        [Test]
        public void GetAllUserTrip_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForGetAllUserTrip"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllUserTrip();
            CheckErrors(repo, "ExceptionForGetAllUserTrip");
        }

        [Test]
        public void GetAllUserTrips_WhenValid_ShouldNotSetErrors()
        {
            var list = new List<UserTrip> { ModelTestHelper.CreateUserTrip(1, "Trip1"), ModelTestHelper.CreateUserTrip(2, "Trip2") };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllUserTrip();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(u => u.UserId == 1));
            Assert.IsTrue(dbList.Any(u => u.UserId == 2));
        }

        #endregion
    }
}
