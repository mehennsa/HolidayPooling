using NUnit.Framework;
using System.Linq;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.Tests;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.ImportExport;
using System;

namespace HolidayPooling.DataRepositories.Tests.ImportExport
{

    [TestFixture]
    public class UserTripDbImportExportTest : DbImportExportTestBase<UserTripDbImportExport, UserTripKey, UserTrip>
    {

        #region DbImportExportTestBase<UserTripDbImportExport, UserTripKey, UserTrip>

        protected override string TableName
        {
            get { return "TUSRTRP"; }
        }

        protected override UserTrip CreateModel()
        {
            return ModelTestHelper.CreateUserTrip(1, "ATrip");
        }

        protected override UserTripKey GetKeyFromModel(UserTrip entity)
        {
            return new UserTripKey(entity.UserId, entity.TripName);
        }

        protected override void UpdateModel(UserTrip model)
        {
            model.HasParticipated = !model.HasParticipated;
            model.HasOrganized = !model.HasOrganized;
            model.TripAmount += 200.50;
        }

        public override void CompareWithDbValues(UserTrip entity, UserTrip dbEntity, DateTime modificationDate)
        {
            Assert.AreEqual(entity.UserId, dbEntity.UserId);
            Assert.AreEqual(entity.TripName, dbEntity.TripName);
            Assert.AreEqual(entity.TripAmount, dbEntity.TripAmount);
            Assert.AreEqual(entity.HasOrganized, dbEntity.HasOrganized);
            Assert.AreEqual(entity.HasParticipated, dbEntity.HasParticipated);
            Assert.AreEqual(modificationDate, dbEntity.ModificationDate);
        }

        protected override UserTripDbImportExport CreateImportExport()
        {
            return new UserTripDbImportExport(new MockTimeProvider(_insertTime));
        }

        protected override UserTripDbImportExport CreateImportExportForUpdate()
        {
            return new UserTripDbImportExport(new MockTimeProvider(_updateTime));
        }

        #endregion

        #region Tests

        [Test]
        public void GetTripsForUser_WhenExceptionIsThrown_ShouldThrowArgumentNullException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetTripForUser(-1));
        }

        [Test]
        public void GetTripForUser_ShouldReturnOnlyUserTrip()
        {
            var firstUserTrip = ModelTestHelper.CreateUserTrip(1, "First");
            var secondUserTrip = ModelTestHelper.CreateUserTrip(2, "Second");
            var thirdUserTrip = ModelTestHelper.CreateUserTrip(1, "Third");
            Assert.IsTrue(_importExport.Save(firstUserTrip));
            Assert.IsTrue(_importExport.Save(secondUserTrip));
            Assert.IsTrue(_importExport.Save(thirdUserTrip));
            var list = _importExport.GetTripForUser(1);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(u => u.TripName == "First"));
            Assert.IsFalse(list.Any(u => u.TripName == "Second"));
            Assert.IsTrue(list.Any((u => u.TripName == "Third")));
            Assert.IsFalse(list.Any(u => u.UserId == 2));
        }

        [Test]
        public void GetUserTripsByTrip_WhenException_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetUserTripsByTrip("aTrip"));
        }

        [Test]
        public void GetUserTripsByTrip_ShouldReturnRightRecord()
        {
            var firstUserTrip = ModelTestHelper.CreateUserTrip(1, "First");
            var secondUserTrip = ModelTestHelper.CreateUserTrip(2, "Second");
            var thirdUserTrip = ModelTestHelper.CreateUserTrip(3, "First");
            Assert.IsTrue(_importExport.Save(firstUserTrip));
            Assert.IsTrue(_importExport.Save(secondUserTrip));
            Assert.IsTrue(_importExport.Save(thirdUserTrip));
            var list = _importExport.GetUserTripsByTrip("First");
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(u => u.UserId == 1));
            Assert.IsFalse(list.Any(u => u.TripName == "Second"));
            Assert.IsTrue(list.Any((u => u.UserId == 3)));
            Assert.IsFalse(list.Any(u => u.UserId == 2));
        }

        [Test]
        public void GetAllEntities_ShouldReturnAllRecords()
        {
            var firstUserTrip = ModelTestHelper.CreateUserTrip(1, "First");
            var secondUserTrip = ModelTestHelper.CreateUserTrip(2, "Second");
            var thirdUserTrip = ModelTestHelper.CreateUserTrip(1, "Third");
            Assert.IsTrue(_importExport.Save(firstUserTrip));
            Assert.IsTrue(_importExport.Save(secondUserTrip));
            Assert.IsTrue(_importExport.Save(thirdUserTrip));
            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(u => u.TripName == "First"));
            Assert.IsTrue(list.Any(u => u.TripName == "Second"));
            Assert.IsTrue(list.Any((u => u.TripName == "Third")));
            Assert.IsTrue(list.Any(u => u.UserId == 1));
            Assert.IsTrue(list.Any(u => u.UserId == 2));
        }

        #endregion

    }
}
