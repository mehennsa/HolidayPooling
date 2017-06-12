using HolidayPooling.DataRepositories.ImportExport;
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
    public class FriendshipRepositoryTest : RepositoryTestBase<IFriendshipDbImportExport, IFriendshipRepository>
    {

        #region Constants

        private const string NullFriendshipErrorMessage = "Please provide valid frienship information";
        private const string SaveFailed = "Internal Error : Unable to save friendship";
        private const string UpdateFailed = "Internal Error : Unable to update friendship";
        private const string DeleteFailed = "Internal Error : Unable to delete friendship";

        #endregion

        #region RepositoryTestBase<IFriendshipDbImportExport, IFriendshipRepository>

        protected override IFriendshipRepository CreateRepository(IFriendshipDbImportExport persister)
        {
            return new FriendshipRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void Save_WhenNullFriendship_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SaveFriendship(null);
            CheckErrors(repo, NullFriendshipErrorMessage);
        }

        [Test]
        public void SaveFriendship_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<Friendship>())).Throws(new ImportExportException("ExceptionForSaveTest"));
            var repo = CreateRepository(mock.Object);
            repo.SaveFriendship(new Friendship());
            CheckErrors(repo, "ExceptionForSaveTest");
        }

        [Test]
        public void SaveFriendship_WhenDbInsertFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<Friendship>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.SaveFriendship(new Friendship());
            CheckErrors(repo, SaveFailed);
        }

        [Test]
        public void SaveFriendship_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<Friendship>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.SaveFriendship(new Friendship());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void UpdateFriendship_WhenNullFriendship_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdateFrendship(null);
            CheckErrors(repo, NullFriendshipErrorMessage);
        }

        [Test]
        public void UpdateFriendship_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Friendship>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdateFrendship(new Friendship());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void UpdateFriendship_WhenDbUpdateFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Friendship>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdateFrendship(new Friendship());
            CheckErrors(repo, UpdateFailed);
        }

        [Test]
        public void UpdateFriendship_WhenValid_ShoulNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Friendship>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.UpdateFrendship(new Friendship());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void DeleteFriendship_WhenNullFriendship_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeleteFriendship(null);
            CheckErrors(repo, NullFriendshipErrorMessage);
        }

        [Test]
        public void DeleteFriendship_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Friendship>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeleteFriendship(new Friendship());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void DeleteFriendship_WhenDbDeleteFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Friendship>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeleteFriendship(new Friendship());
            CheckErrors(repo, DeleteFailed);
        }

        [Test]
        public void DeleteFriendship_WhenValid_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Friendship>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.DeleteFriendship(new Friendship());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void GetFriendship_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<FriendshipKey>())).Throws(new ImportExportException("ExceptionForGetFriendship"));
            var repo = CreateRepository(mock.Object);
            var friendship = repo.GetFriendship(1, "toto");
            CheckErrors(repo, "ExceptionForGetFriendship");
            Assert.IsNull(friendship);
        }

        [Test]
        public void GetFriendship_ShouldReturnFriendship()
        {
            var mock = CreateMock();
            var friendship = ModelTestHelper.CreateFriendship(1, "toto");
            mock.Setup(s => s.GetEntity(It.IsAny<FriendshipKey>())).Returns(friendship);
            var repo = CreateRepository(mock.Object);
            var dbfriendship = repo.GetFriendship(1, "toto");
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(friendship);
        }

        [Test]
        public void GetUserFriendships_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserFriendships(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetUserFriendships"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserFriendships(1);
            CheckErrors(repo, "ExceptionForGetUserFriendships");
        }

        [Test]
        public void GetUserFriendships_ShouldReturnValidList()
        {
            var list = new List<Friendship>
            {
                ModelTestHelper.CreateFriendship(1, "Afriend"),
                ModelTestHelper.CreateFriendship(1, "SecondFriend")
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetUserFriendships(1)).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetUserFriendships(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(f => f.FriendName == "Afriend"));
            Assert.IsTrue(dbList.Any(f => f.FriendName == "SecondFriend"));
        }

        [Test]
        public void GetRequestedFriendship_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetRequestedFriendships(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForRequestedFriendship"));
            var repo = CreateRepository(mock.Object);
            repo.GetRequestedFriendships(1);
            CheckErrors(repo, "ExceptionForRequestedFriendship");
        }
        
        [Test]
        public void GetRequestedFriendships_ShouldReturnValidList()
        {
            var list = new List<Friendship>
            {
                ModelTestHelper.CreateFriendship(2, "Afriend"),
                ModelTestHelper.CreateFriendship(2, "SecondFriend")
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetRequestedFriendships(2)).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetRequestedFriendships(2);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(f => f.FriendName == "Afriend"));
            Assert.IsTrue(dbList.Any(f => f.FriendName == "SecondFriend"));
        }

        [Test]
        public void GetWaitingFriendships_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetWaitingFriendships(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForWaitingFriendship"));
            var repo = CreateRepository(mock.Object);
            repo.GetWaitingFriendships(1);
            CheckErrors(repo, "ExceptionForWaitingFriendship");
        }

        [Test]
        public void GetWaitingFriendships_ShouldReturnValidList()
        {
            var list = new List<Friendship>
            {
                ModelTestHelper.CreateFriendship(3, "Afriend"),
                ModelTestHelper.CreateFriendship(3, "SecondFriend")
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetWaitingFriendships(3)).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetWaitingFriendships(3);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(f => f.FriendName == "Afriend"));
            Assert.IsTrue(dbList.Any(f => f.FriendName == "SecondFriend"));
        }

        [Test]
        public void GetAllFriendships_WhenException_ShouldReturnValidList()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForGetAllFriendships"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllFriendship();
            CheckErrors(repo, "ExceptionForGetAllFriendships");
        }

        [Test]
        public void GetAllFriendships_ShouldReturnValidList()
        {
            var list = new List<Friendship>
            {
                ModelTestHelper.CreateFriendship(2, "Afriend"),
                ModelTestHelper.CreateFriendship(3, "SecondFriend")
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllFriendship();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(f => f.FriendName == "Afriend"));
            Assert.IsTrue(dbList.Any(f => f.FriendName == "SecondFriend"));
            Assert.IsTrue(dbList.Any(f => f.UserId == 2));
            Assert.IsTrue(dbList.Any(f => f.UserId == 3));
        }

        #endregion

    }
}
