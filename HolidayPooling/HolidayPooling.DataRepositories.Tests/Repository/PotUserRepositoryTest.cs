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
    public class PotUserRepositoryTest : RepositoryTestBase<IPotUserDbImportExport, IPotUserRepository>
    {


        #region Constants

        private const string NullPotUserErrorMessage = "Please provide a valid pot's user";

        private const string SaveFail = "Internal Error : Unable to save pot's user";

        private const string UpdateFail = "Internal Error : Unable to update pot's user";

        private const string DeleteFail = "Internal Error : Unable to delete pot's user";

        #endregion

        #region RepositoryTestBase<IPotUserDbImportExport, IPotUserRepository>

        protected override IPotUserRepository CreateRepository(IPotUserDbImportExport persister)
        {
            return new PotUserRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void SavePotUser_WhenUserPotNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SavePotUser(null);
            CheckErrors(repo, NullPotUserErrorMessage);
        }

        [Test]
        public void SavePotUser_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<PotUser>())).Throws(new ImportExportException("ExceptionForSaveTest"));
            var repo = CreateRepository(mock.Object);
            repo.SavePotUser(new PotUser());
            CheckErrors(repo, "ExceptionForSaveTest");
        }

        [Test]
        public void SavePotUser_WhenDbInsertFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<PotUser>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.SavePotUser(new PotUser());
            CheckErrors(repo, SaveFail);
        }

        [Test]
        public void SavePotUser_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<PotUser>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.SavePotUser(new PotUser());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void UpdatePotUser_WhenNullPotUser_ShoudLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdatePotUser(null);
            CheckErrors(repo, NullPotUserErrorMessage);
        }

        [Test]
        public void UpdatePotUser_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<PotUser>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdatePotUser(new PotUser());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void UpdatePotUser_WhenDbUpdateFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<PotUser>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdatePotUser(new PotUser());
            CheckErrors(repo, UpdateFail);
        }

        [Test]
        public void UpdatePotUser_WhenValid_ShouldNotLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<PotUser>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.UpdatePotUser(new PotUser());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void DeletePotUser_WhenNullPotUser_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeletePotUser(null);
            CheckErrors(repo, NullPotUserErrorMessage);
        }

        [Test]
        public void DeletePotUser_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<PotUser>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeletePotUser(new PotUser());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void DeletePotUser_WhenDbDeleteFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<PotUser>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeletePotUser(new PotUser());
            CheckErrors(repo, DeleteFail);
        }

        [Test]
        public void DeletePotUser_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<PotUser>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.DeletePotUser(new PotUser());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void GetPotUser_WhenException_ShoudLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<PotUserKey>())).Throws(new ImportExportException("ExceptionForGetPotUserTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetPotUser(1, 1);
            CheckErrors(repo, "ExceptionForGetPotUserTest");
        }

        [Test]
        public void GetPotUser_ShouldReturnPotUser()
        {
            var mock = CreateMock();
            var potUser = ModelTestHelper.CreatePotUser(1, 1);
            mock.Setup(s => s.GetEntity(It.IsAny<PotUserKey>())).Returns(potUser);
            var repo = CreateRepository(mock.Object);
            var dbPotUser = repo.GetPotUser(1, 1);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbPotUser);
            Assert.AreEqual(1, dbPotUser.UserId);
            Assert.AreEqual(1, dbPotUser.PotId);
        }

        [Test]
        public void GetPotUsers_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetPotUsers(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetPotUsersTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetPotUsers(1);
            CheckErrors(repo, "ExceptionForGetPotUsersTest");
        }

        [Test]
        public void GetPotUsers_ShouldReturnValidList()
        {
            var list = new List<PotUser> 
            {
                ModelTestHelper.CreatePotUser(1, 1),
                ModelTestHelper.CreatePotUser(2, 1)
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetPotUsers(It.IsAny<int>())).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetPotUsers(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(p => p.UserId == 1));
            Assert.IsTrue(dbList.Any(p => p.UserId == 1));
        }

        [Test]
        public void GetUserPots_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserPots(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetUserPots"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserPots(1);
            CheckErrors(repo, "ExceptionForGetUserPots");
        }

        [Test]
        public void GetUserPots_ShouldReturnValidList()
        {
            var list = new List<PotUser> 
            {
                ModelTestHelper.CreatePotUser(1, 1),
                ModelTestHelper.CreatePotUser(1, 2)
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetUserPots(It.IsAny<int>())).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetUserPots(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(p => p.PotId == 1));
            Assert.IsTrue(dbList.Any(p => p.PotId == 1));
        }

        [Test]
        public void GetAllUserPots_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForGetAllUserPots"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllPotUser();
            CheckErrors(repo, "ExceptionForGetAllUserPots");
        }

        [Test]
        public void GetAllPotUser_ShouldReturnValidList()
        {
            var list = new List<PotUser> 
            {
                ModelTestHelper.CreatePotUser(1, 1),
                ModelTestHelper.CreatePotUser(1, 2)
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllPotUser();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(p => p.PotId == 1));
            Assert.IsTrue(dbList.Any(p => p.PotId == 1));
            Assert.IsTrue(dbList.Any(p => p.UserId == 1));
        }

        #endregion

    }
}
