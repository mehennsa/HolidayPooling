using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayPooling.Tests;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.Core;

namespace HolidayPooling.DataRepositories.Tests.Business
{
    [TestFixture]
    public class TripDbImportExportTest : DbImportExportTestBase<TripDbImportExport, int, Trip>
    {

        #region DbImportExportTestBase<TripDbImportExport, int, Trip>

        protected override string TableName
        {
            get { return "TTRP"; }
        }

        protected override Trip CreateModel()
        {
            return ModelTestHelper.CreateTrip(1, "MyTrip");
        }

        protected override int GetKeyFromModel(Trip entity)
        {
            return entity.Id;
        }

        protected override void UpdateModel(Trip model)
        {
            model.Price += 300.562;
        }

        public override void CompareWithDbValues(Trip entity, Trip dbEntity)
        {
            Assert.AreEqual(entity.Id, dbEntity.Id);
            Assert.AreEqual(entity.TripName, dbEntity.TripName);
            Assert.AreEqual(entity.Description, dbEntity.Description);
            Assert.AreEqual(entity.StartDate, dbEntity.StartDate);
            Assert.AreEqual(entity.ValidityDate, dbEntity.ValidityDate);
            Assert.AreEqual(entity.EndDate, dbEntity.EndDate);
            Assert.AreEqual(entity.Price, dbEntity.Price);
            Assert.AreEqual(entity.Organizer, dbEntity.Organizer);
            Assert.AreEqual(entity.NumberMaxOfPeople, dbEntity.NumberMaxOfPeople);
            Assert.AreEqual(entity.Note, dbEntity.Note);
            Assert.AreEqual(entity.Location, dbEntity.Location);
        }

        #endregion

        #region Tests

        [Test]
        public void GetNewId_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetNewId());
        }

        [Test]
        public void Save_WhenIdCannotBeGenerated_ShouldReturnFalse()
        {
            var mock = CreateMock(false);
            mock.Setup(s => s.GetNewId()).Returns(-4);
            Assert.IsFalse(mock.Object.Save(CreateModel()));
        }

        [Test]
        public void IsTripNameUsed_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.IsTripNameUsed("toto"));
        }

        [Test]
        public void IsTripNameUsed_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.IsTripNameUsed("TripNotUsed"));
        }

        public void IsTripNameUsed_WhenExist_ShouldReturnTrue()
        {
            var trip = CreateModel();
            Assert.IsTrue(_importExport.Save(trip));
            Assert.IsTrue(_importExport.IsTripNameUsed(trip.TripName));
        }

        [Test]
        public void GetTripByName_WhenExceptionIsThrown_ShouldThrowArgumentNullException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetTripByName("Wrong"));
        }

        [Test]
        public void GetTripByName_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetTripByName("TripByNameNotExist"));
        }
        

        [Test]
        public void GetTripByName_WhenExist_ShouldReturnTrip()
        {
            var trip = CreateModel();
            trip.TripName = "TripGetTripByName";
            trip.Price += 200.34;
            Assert.IsTrue(_importExport.Save(trip));
            Assert.IsTrue(trip.Id > 0);
            var dbEntity = _importExport.GetTripByName("TripGetTripByName");
            Assert.IsNotNull(dbEntity);
            CompareWithDbValues(trip, dbEntity);
        }

        [Test]
        public void GetValidTrips_WhenExceptionThrown_ShouldThrowImportExportException() 
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetValidTrips(DateTime.Today));
        }

        [Test]
        public void GetValidTrips_ShouldReturnRightNumberOfTrips()
        {
            var validDate = new DateTime(2017, 03, 18);
            var firstTrip = CreateModel();
            firstTrip.TripName = "FirstValidTrip";
            firstTrip.ValidityDate = new DateTime(2018, 02, 01);
            var secondTrip = CreateModel();
            secondTrip.TripName = "SecondValidTrip";
            secondTrip.ValidityDate = new DateTime(2017, 03, 18);
            var thirdTrip = CreateModel();
            thirdTrip.TripName = "TripNotValid";
            thirdTrip.ValidityDate = new DateTime(2017, 03, 17);
            Assert.IsTrue(_importExport.Save(firstTrip));
            Assert.IsTrue(_importExport.Save(secondTrip));
            Assert.IsTrue(_importExport.Save(thirdTrip));

            var validTrips = _importExport.GetValidTrips(validDate).ToList();
            Assert.AreEqual(2, validTrips.Count);
            Assert.IsTrue(validTrips.Any(t => t.Id == firstTrip.Id));
            Assert.IsTrue(validTrips.Any(t => t.Id == secondTrip.Id));
            Assert.IsFalse(validTrips.Any(t => t.Id == thirdTrip.Id));
        }

        [Test]
        public void GetTripBetweenStartDateAndEndDate_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetTripBetweenStartDateAndEndDate(null, DateTime.Today));
        }

        [Test]
        public void GetTripBetweenStartDateAndEndDate_WhenDatesAreNull_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _importExport.GetTripBetweenStartDateAndEndDate(null, null));
        }

        [Test]
        public void GetTripBetweenStartDateAndEndDate_WithOnlyStartDate_ShouldReturnRightNumberOfRecord()
        {
            var startDate = new DateTime(2017, 03, 18);
            var firstTrip = CreateModel();
            firstTrip.TripName = "FirstValidTrip";
            firstTrip.StartDate = new DateTime(2018, 02, 01);
            var secondTrip = CreateModel();
            secondTrip.TripName = "SecondValidTrip";
            secondTrip.StartDate = new DateTime(2017, 03, 18);
            var thirdTrip = CreateModel();
            thirdTrip.TripName = "TripNotValid";
            thirdTrip.StartDate = new DateTime(2017, 03, 17);
            Assert.IsTrue(_importExport.Save(firstTrip));
            Assert.IsTrue(_importExport.Save(secondTrip));
            Assert.IsTrue(_importExport.Save(thirdTrip));

            var validTrips = _importExport.GetTripBetweenStartDateAndEndDate(startDate, null).ToList();
            Assert.AreEqual(2, validTrips.Count);
            Assert.IsTrue(validTrips.Any(t => t.Id == firstTrip.Id));
            Assert.IsTrue(validTrips.Any(t => t.Id == secondTrip.Id));
            Assert.IsFalse(validTrips.Any(t => t.Id == thirdTrip.Id));
        }

        [Test]
        public void GetTripBetweenStartDateAndEndDate_WithOnlyEndDate_ShouldReturnRightNumberOfRecords()
        {
            var endDate = new DateTime(2017, 03, 18);
            var firstTrip = CreateModel();
            firstTrip.TripName = "FirstValidTrip";
            firstTrip.EndDate = new DateTime(2016, 02, 01);
            var secondTrip = CreateModel();
            secondTrip.TripName = "SecondValidTrip";
            secondTrip.EndDate = new DateTime(2017, 03, 18);
            var thirdTrip = CreateModel();
            thirdTrip.TripName = "TripNotValid";
            thirdTrip.EndDate = new DateTime(2018, 03, 17);
            Assert.IsTrue(_importExport.Save(firstTrip));
            Assert.IsTrue(_importExport.Save(secondTrip));
            Assert.IsTrue(_importExport.Save(thirdTrip));

            var validTrips = _importExport.GetTripBetweenStartDateAndEndDate(null, endDate).ToList();
            Assert.AreEqual(2, validTrips.Count);
            Assert.IsTrue(validTrips.Any(t => t.Id == firstTrip.Id));
            Assert.IsTrue(validTrips.Any(t => t.Id == secondTrip.Id));
            Assert.IsFalse(validTrips.Any(t => t.Id == thirdTrip.Id));
        }

        [Test]
        public void GetTripBetweenStartDateAndEndDate_WithStartDateAndEndDate_ShouldReturnRightNumberOfRecord()
        {
            var startDate = new DateTime(2017, 03, 18);
            var endDate = new DateTime(2018, 03, 18);
            var firstTrip = CreateModel();
            firstTrip.TripName = "FirstNotValidTrip";
            firstTrip.StartDate = new DateTime(2016, 02, 01);
            firstTrip.EndDate = new DateTime(2018, 02, 01);
            var secondTrip = CreateModel();
            secondTrip.TripName = "SecondValidTrip";
            secondTrip.StartDate = new DateTime(2017, 03, 18);
            secondTrip.EndDate = new DateTime(2017, 09, 01);
            var thirdTrip = CreateModel();
            thirdTrip.TripName = "TripNotValid";
            thirdTrip.StartDate = new DateTime(2017, 09, 01);
            thirdTrip.EndDate = new DateTime(2018, 03, 25);
            Assert.IsTrue(_importExport.Save(firstTrip));
            Assert.IsTrue(_importExport.Save(secondTrip));
            Assert.IsTrue(_importExport.Save(thirdTrip));

            var validTrips = _importExport.GetTripBetweenStartDateAndEndDate(startDate, endDate).ToList();
            Assert.AreEqual(1, validTrips.Count);
            Assert.IsFalse(validTrips.Any(t => t.Id == firstTrip.Id));
            Assert.IsTrue(validTrips.Any(t => t.Id == secondTrip.Id));
            Assert.IsFalse(validTrips.Any(t => t.Id == thirdTrip.Id));
        }

        [Test]
        public void GetAllEntities_ShouldRightNumberOfRecord()
        {
            var firstTrip = CreateModel();
            firstTrip.TripName = "FirstValidTrip";
            firstTrip.StartDate = new DateTime(2016, 02, 01);
            firstTrip.EndDate = new DateTime(2018, 02, 01);
            var secondTrip = CreateModel();
            secondTrip.TripName = "SecondValidTrip";
            secondTrip.StartDate = new DateTime(2017, 03, 18);
            secondTrip.EndDate = new DateTime(2017, 09, 01);
            var thirdTrip = CreateModel();
            thirdTrip.TripName = "ThirdTripValid";
            thirdTrip.StartDate = new DateTime(2017, 09, 01);
            thirdTrip.EndDate = new DateTime(2018, 03, 25);
            Assert.IsTrue(_importExport.Save(firstTrip));
            Assert.IsTrue(_importExport.Save(secondTrip));
            Assert.IsTrue(_importExport.Save(thirdTrip));

            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(t => t.Id == firstTrip.Id));
            Assert.IsTrue(list.Any(t => t.Id == secondTrip.Id));
            Assert.IsTrue(list.Any(t => t.TripName == "ThirdTripValid"));
        }

        #endregion

    }
}
