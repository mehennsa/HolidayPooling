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
    public class PotRepositoryTest : RepositoryTestBase<IPotDbImportExport, IPotRepository>
    {

        #region Constants

        private const string NullPotErrorMessage = "Please provide a pot";

        private const string SaveFailed = "Internal Error : Unable to save pot";

        private const string UpdateFailed = "Internal Error : Unable to update pot";

        private const string DeleteFailed = "Internal Error : Unable to delete pot";

        #endregion

        #region RepositoryTestBase<IPotDbImportExport, IPotRepository>

        protected override IPotRepository CreateRepository(IPotDbImportExport persister)
        {
            return new PotRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void SavePot_WhenNullPot_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SavePot(null);
            CheckErrors(repo, NullPotErrorMessage);
        }

        [Test]
        public void SavePot_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPotNameUsed(It.IsAny<string>())).Throws(new ImportExportException("ExceptionForSaveTest"));
            var repo = CreateRepository(mock.Object);
            repo.SavePot(new Pot());
            CheckErrors(repo, "ExceptionForSaveTest");
        }

        [Test]
        public void SavePot_WhenPotNameIsUsed_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPotNameUsed(It.IsAny<string>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var pot = ModelTestHelper.CreatePot(1, 2);
            repo.SavePot(pot);
            CheckErrors(repo, string.Format("Pot name {0} is already use, please choose another one", pot.Name));
        }

        [Test]
        public void SavePot_WhenDbInsertFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPotNameUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(It.IsAny<Pot>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.SavePot(new Pot());
            CheckErrors(repo, SaveFailed);
        }

        [Test]
        public void SavePot_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPotNameUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(It.IsAny<Pot>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.SavePot(new Pot());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void Update_WhenNullPot_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdatePot(null);
            CheckErrors(repo, NullPotErrorMessage);
        }

        [Test]
        public void Update_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Pot>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdatePot(new Pot());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void Update_WhenDbUpdateFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Pot>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdatePot(new Pot());
            CheckErrors(repo, UpdateFailed);
        }

        [Test]
        public void Update_WhenValid_ShouldNotSetError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Pot>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.UpdatePot(new Pot());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void Delete_WhenNullPot_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeletePot(null);
            CheckErrors(repo, NullPotErrorMessage);
        }

        [Test]
        public void Delete_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Pot>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeletePot(new Pot());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void Delete_WhenDbDeleteFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Pot>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeletePot(new Pot());
            CheckErrors(repo, DeleteFailed);
        }

        [Test]
        public void Delete_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Pot>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.DeletePot(new Pot());
            Assert.IsFalse(repo.HasErrors);
        }
        
        [Test]
        public void GetPotById_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetPotByIdTest"));
            var repo = CreateRepository(mock.Object);
            var pot = repo.GetPot(1);
            CheckErrors(repo, "ExceptionForGetPotByIdTest");
            Assert.IsNull(pot);
        }

        [Test]
        public void GetPotById_ShouldReturnPot()
        {
            var mock = CreateMock();
            var pot = ModelTestHelper.CreatePot(1, 2);
            mock.Setup(s => s.GetEntity(It.IsAny<int>())).Returns(pot);
            var repo = CreateRepository(mock.Object);
            var dbPot = repo.GetPot(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(pot.Name, dbPot.Name);
        }

        [Test]
        public void GetTripsPot_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetTripsPot(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetTripsPotTest"));
            var repo = CreateRepository(mock.Object);
            var pot = repo.GetTripPots(1);
            CheckErrors(repo, "ExceptionForGetTripsPotTest");
            Assert.IsNull(pot);
        }

        [Test]
        public void GetTripsPot_ShouldReturnPot()
        {
            var mock = CreateMock();
            var pot = ModelTestHelper.CreatePot(1, 2);
            mock.Setup(s => s.GetTripsPot(It.IsAny<int>())).Returns(pot);
            var repo = CreateRepository(mock.Object);
            var dbPot = repo.GetTripPots(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(pot.Id, dbPot.Id);
        }

        [Test]
        public void GetPotByName_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetPotByName(It.IsAny<string>())).Throws(new ImportExportException("ExceptionForGetPotByName"));
            var repo = CreateRepository(mock.Object);
            var pot = repo.GetPot("toto");
            CheckErrors(repo, "ExceptionForGetPotByName");
            Assert.IsNull(pot);
        }

        [Test]
        public void GetPotByName_ShouldReturnPot()
        {
            var mock = CreateMock();
            var pot = ModelTestHelper.CreatePot(1, 2);
            mock.Setup(s => s.GetPotByName(It.IsAny<string>())).Returns(pot);
            var repo = CreateRepository(mock.Object);
            var dbPot = repo.GetPot(pot.Name);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(pot.Id, dbPot.Id);
        }

        [Test]
        public void GetAllPots_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForGetAllTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllPots();
            CheckErrors(repo, "ExceptionForGetAllTest");
        }

        [Test]
        public void GetAllPots_ShouldReturnValidList()
        {
            var list = new List<Pot> 
            {
                ModelTestHelper.CreatePot(1, 2),
                ModelTestHelper.CreatePot(2, 1),
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllPots();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(p => p.TripId == 2));
            Assert.IsTrue(dbList.Any(p => p.Id == 1));
        }

        #endregion
    }
}
