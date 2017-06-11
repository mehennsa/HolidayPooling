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
    public class TripParticipantRepositoryTest : RepositoryTestBase<ITripParticipantDbImportExport, ITripParticipantRepository>
    {


        #region Constants

        private const string NullTripParticipantErrorMessage = "Please provide a valid trip participant";

        private const string SaveFailed = "Internal Error : Unable to save trip's participant";

        private const string DeleteFailed = "Internal Error : Unable to delete trip's participant";

        private const string UpdateFailed = "Internal Error : Unable to update trip's participant";

        #endregion

        #region RepositoryTestBase<ITripParticipantDbImportExport, ITripParticipantRepository>

        protected override ITripParticipantRepository CreateRepository(ITripParticipantDbImportExport persister)
        {
            return new TripParticipantRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void SaveTripParticipant_WhenTripParticipantNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SaveTripParticipant(null);
            CheckErrors(repo, NullTripParticipantErrorMessage);
        }

        [Test]
        public void SaveTripParticipant_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<TripParticipant>())).Throws(new ImportExportException("ExceptionForSaveTest"));
            var repo = CreateRepository(mock.Object);
            repo.SaveTripParticipant(new TripParticipant());
            CheckErrors(repo, "ExceptionForSaveTest");
        }

        [Test]
        public void SaveTripParticipant_WhenDbInsertFailed_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<TripParticipant>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.SaveTripParticipant(new TripParticipant());
            CheckErrors(repo, SaveFailed);
        }

        [Test]
        public void SaveTripParticipant_WhenValid_ShouldNotSetError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Save(It.IsAny<TripParticipant>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.SaveTripParticipant(new TripParticipant());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void UpdateTripParticipant_WhenTripParticipantIsNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdateTripParticipant(null);
            CheckErrors(repo, NullTripParticipantErrorMessage);
        }

        [Test]
        public void UpdateTripParticipant_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<TripParticipant>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdateTripParticipant(new TripParticipant());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void UpdateTripParticipant_WhenUpdateFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<TripParticipant>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdateTripParticipant(new TripParticipant());
            CheckErrors(repo, UpdateFailed);
        }

        [Test]
        public void UpdateTripParticipant_WhenValid_ShouldNotSetErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<TripParticipant>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.UpdateTripParticipant(new TripParticipant());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void DeleteTripParticipant_WhenTripParticipantIsNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeleteTripParticipant(null);
            CheckErrors(repo, NullTripParticipantErrorMessage);
        }

        [Test]
        public void DeleteTripParticipant_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<TripParticipant>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeleteTripParticipant(new TripParticipant());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void DeleteTripParticipant_WhenDbDeleteFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<TripParticipant>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeleteTripParticipant(new TripParticipant());
            CheckErrors(repo, DeleteFailed);
        }

        [Test]
        public void DeleteTripParticipant_WhenValid_ShouldNotLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<TripParticipant>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.DeleteTripParticipant(new TripParticipant());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void GetTripParticipant_WhenExceptionIsThrown_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<TripParticipantKey>())).Throws(new ImportExportException("ExceptionForGetTest"));
            var repo = CreateRepository(mock.Object);
            var pp = repo.GetTripParticipant(1, "fail");
            Assert.IsNull(pp);
            CheckErrors(repo, "ExceptionForGetTest");
        }

        [Test]
        public void GetTripParticipant_ShouldReturnTripParticipant()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<TripParticipantKey>())).Returns(ModelTestHelper.CreateTripParticipant(1, "AParticipant"));
            var repo = CreateRepository(mock.Object);
            var pp = repo.GetTripParticipant(1, "AParticipant");
            Assert.IsNotNull(pp);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(1, pp.TripId);
            Assert.AreEqual("AParticipant", pp.UserPseudo);
        }

        [Test]
        public void GetTripParticipants_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetParticipantsForTrip(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetTripParticipantTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetTripParticipants(1);
            CheckErrors(repo, "ExceptionForGetTripParticipantTest");
        }

        [Test]
        public void GetTripParticipant_ShouldReturnValidList()
        {
            var list = new List<TripParticipant>
            { 
                ModelTestHelper.CreateTripParticipant(1, "PSD1"), 
                ModelTestHelper.CreateTripParticipant(1, "PSD2") 
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetParticipantsForTrip(It.IsAny<int>()))
                .Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetTripParticipants(1);
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(t => t.UserPseudo == "PSD1"));
            Assert.IsTrue(dbList.Any(t => t.UserPseudo == "PSD2"));
        }

        [Test]
        public void GetAllTripParticipants_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForGetAllTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllTripParticipants();
            CheckErrors(repo, "ExceptionForGetAllTest");
        }

        [Test]
        public void GetAllTripParticipants_ShouldReturnValidList()
        {
            var list = new List<TripParticipant>
            { 
                ModelTestHelper.CreateTripParticipant(1, "PSD1"), 
                ModelTestHelper.CreateTripParticipant(2, "PSD2") 
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities())
                .Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllTripParticipants();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(t => t.TripId == 1));
            Assert.IsTrue(dbList.Any(t => t.TripId == 2));
        }
        

        #endregion
    }
}
