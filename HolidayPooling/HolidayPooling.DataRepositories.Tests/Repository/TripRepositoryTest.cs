using HolidayPooling.DataRepositories.ImportExport;
using HolidayPooling.DataRepositories.Core;
using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.DataRepositories.Tests.Repository
{
    [TestFixture]
    public class TripRepositoryTest : RepositoryTestBase<ITripDbImportExport, ITripRepository>
    {

        #region Constants

        private const string NullTripErrorMessage = "Please provide a valid trip";

        private const string SaveFail = "Internal Error : Trip cannot be save";

        private const string UpdateFail = "Internal Error : Trip cannot be updated";

        private const string DeleteFail = "Internal Error : Trip cannot be deleted";

        #endregion

        #region RepositoryTestBase<ITripDbImportExport, ITripRepository>

        protected override ITripRepository CreateRepository(ITripDbImportExport persister)
        {
            return new TripRepository(persister);
        }

        #endregion

        #region Tests

        [Test]
        public void SaveTrip_WhenTripIsNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.SaveTrip(null);
            CheckErrors(repo, NullTripErrorMessage);
        }

        [Test]
        public void SaveTrip_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsTripNameUsed(It.IsAny<string>())).Throws(new ImportExportException("ExceptionSaveForTest"));
            var repo = CreateRepository(mock.Object);
            repo.SaveTrip(new Trip());
            CheckErrors(repo, "ExceptionSaveForTest");
        }

        [Test]
        public void SaveTrip_WhenTripNameIsUsed_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsTripNameUsed(It.IsAny<string>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var trip = ModelTestHelper.CreateTrip(1, "aTrip");
            repo.SaveTrip(trip);
            CheckErrors(repo, string.Format("Trip name {0} is already used, please use another one", trip.TripName));
        }

        [Test]
        public void SaveTrip_WhenDbInsertFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsTripNameUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(It.IsAny<Trip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            var trip = ModelTestHelper.CreateTrip(2, "aTrip");
            repo.SaveTrip(trip);
            CheckErrors(repo, SaveFail);
        }

        [Test]
        public void SaveTrip_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsTripNameUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(It.IsAny<Trip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            var trip = ModelTestHelper.CreateTrip(2, "aTrip");
            repo.SaveTrip(trip);
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void UpdateTrip_WhenTripNull_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.UpdateTrip(null);
            CheckErrors(repo, NullTripErrorMessage);
        }

        [Test]
        public void UpdateTrip_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Trip>())).Throws(new ImportExportException("ExceptionForUpdateTest"));
            var repo = CreateRepository(mock.Object);
            repo.UpdateTrip(new Trip());
            CheckErrors(repo, "ExceptionForUpdateTest");
        }

        [Test]
        public void UpdateTrip_WhenDbUpdateFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Trip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.UpdateTrip(new Trip());
            CheckErrors(repo, UpdateFail);
        }

        [Test]
        public void UpdateTrip_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Update(It.IsAny<Trip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.UpdateTrip(new Trip());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void DeleteTrip_WhenNullTrip_ShouldLogError()
        {
            var repo = CreateRepository(CreateMock().Object);
            repo.DeleteTrip(null);
            CheckErrors(repo, NullTripErrorMessage);
        }

        [Test]
        public void DeleteTrip_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Trip>())).Throws(new ImportExportException("ExceptionForDeleteTest"));
            var repo = CreateRepository(mock.Object);
            repo.DeleteTrip(new Trip());
            CheckErrors(repo, "ExceptionForDeleteTest");
        }

        [Test]
        public void DeleteTrip_WhenDbDeleteFails_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Trip>())).Returns(false);
            var repo = CreateRepository(mock.Object);
            repo.DeleteTrip(new Trip());
            CheckErrors(repo, DeleteFail);
        }

        [Test]
        public void DeleteTrip_WhenValid_ShouldNotLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.Delete(It.IsAny<Trip>())).Returns(true);
            var repo = CreateRepository(mock.Object);
            repo.DeleteTrip(new Trip());
            Assert.IsFalse(repo.HasErrors);
        }

        [Test]
        public void GetTripById_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForGetById"));
            var repo = CreateRepository(mock.Object);
            var trip = repo.GetTrip(1);
            Assert.IsNull(trip);
            CheckErrors(repo, "ExceptionForGetById");
        }

        [Test]
        public void GetTripById_ShouldReturnTrip()
        {
            var trip = ModelTestHelper.CreateTrip(3, "TripById");
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<int>())).Returns(trip);
            var repo = CreateRepository(mock.Object);
            var dbTrip = repo.GetTrip(3);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbTrip);
            Assert.AreEqual(3, dbTrip.Id);
        }

        [Test]
        public void GetTripByName_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetTripByName(It.IsAny<string>())).Throws(new ImportExportException("ExceptionForGetTripByName"));
            var repo = CreateRepository(mock.Object);
            var trip = repo.GetTrip("GetTripByName");
            Assert.IsNull(trip);
            CheckErrors(repo, "ExceptionForGetTripByName");
        }

        [Test]
        public void GetTripByName_ShouldReturnTrip()
        {
            var trip = ModelTestHelper.CreateTrip(4, "TripByName");
            var mock = CreateMock();
            mock.Setup(s => s.GetTripByName(It.IsAny<string>())).Returns(trip);
            var repo = CreateRepository(mock.Object);
            var dbTrip = repo.GetTrip("TripByName");
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(dbTrip);
            Assert.AreEqual(4, dbTrip.Id);
        }

        [Test]
        public void GetValidTrips_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetValidTrips(It.IsAny<DateTime>())).Throws(new ImportExportException("ExceptionForGetValidTrips"));
            var repo = CreateRepository(mock.Object);
            repo.GetValidTrips(new DateTime());
            CheckErrors(repo, "ExceptionForGetValidTrips");
        }

        [Test]
        public void GetValidTrips_ShouldReturnValidList()
        {
            var list = new List<Trip>
            {
                ModelTestHelper.CreateTrip(1, "Trip1"),
                ModelTestHelper.CreateTrip(2, "Trip2"),
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetValidTrips(It.IsAny<DateTime>())).Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetValidTrips(new DateTime());
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(t => t.Id == 1));
            Assert.IsTrue(dbList.Any(t => t.Id == 2));
        }

        [Test]
        public void GetTripByDate_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetTripBetweenStartDateAndEndDate(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Throws(new ImportExportException("ExceptionForGetTripByDates"));
            var repo = CreateRepository(mock.Object);
            repo.GetTripsByDate(new DateTime(), new DateTime());
            CheckErrors(repo, "ExceptionForGetTripByDates");
        }

        [Test]
        public void GetTripByDate_ShouldReturnValidList()
        {
            var list = new List<Trip>
            {
                ModelTestHelper.CreateTrip(1, "Trip1"),
                ModelTestHelper.CreateTrip(2, "Trip2"),
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetTripBetweenStartDateAndEndDate(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetTripsByDate(new DateTime(), new DateTime());
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(t => t.Id == 1));
            Assert.IsTrue(dbList.Any(t => t.Id == 2));
        }

        [Test]
        public void GetAllTrips_WhenException_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities())
                .Throws(new ImportExportException("ExceptionForGetAllTrips"));
            var repo = CreateRepository(mock.Object);
            repo.GetAllTrips();
            CheckErrors(repo, "ExceptionForGetAllTrips");
        }

        [Test]
        public void GetAllTrips_ShouldReturnValidList()
        {
            var list = new List<Trip>
            {
                ModelTestHelper.CreateTrip(1, "Trip1"),
                ModelTestHelper.CreateTrip(2, "Trip2"),
            };
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities())
                .Returns(list);
            var repo = CreateRepository(mock.Object);
            var dbList = repo.GetAllTrips();
            Assert.IsFalse(repo.HasErrors);
            Assert.AreEqual(2, dbList.Count());
            Assert.IsTrue(dbList.Any(t => t.Id == 1));
            Assert.IsTrue(dbList.Any(t => t.Id == 2));
        }

        #endregion

    }
}
