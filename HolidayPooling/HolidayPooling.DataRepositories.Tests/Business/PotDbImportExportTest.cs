using HolidayPooling.DataRepositories.Business;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Tests.Business
{
    // Test Import Export : Pot
    [TestFixture]
    public class PotDbImportExportTest : DbImportExportTestBase<PotDbImportExport, int, Pot>
    {


        #region DbImportExportTestBase<PotDbImportExport, int, Pot>

        protected override string TableName
        {
            get { return "TPOT"; }
        }

        protected override Pot CreateModel()
        {
            return ModelTestHelper.CreatePot(1, 2);
        }

        protected override int GetKeyFromModel(Pot entity)
        {
            return entity.Id;
        }

        protected override void UpdateModel(Pot model)
        {
            model.IsCancelled = !model.IsCancelled;
            model.TargetAmount += 300.12;
        }

        public override void CompareWithDbValues(Pot entity, Pot dbEntity)
        {
            Assert.AreEqual(entity.Id, dbEntity.Id);
            Assert.AreEqual(entity.TripId, dbEntity.TripId);
            Assert.AreEqual(entity.CancellationDate, dbEntity.CancellationDate);
            Assert.AreEqual(entity.CancellationReason, dbEntity.CancellationReason);
            Assert.AreEqual(entity.IsCancelled, dbEntity.IsCancelled);
            Assert.AreEqual(entity.CurrentAmount, dbEntity.CurrentAmount);
            Assert.AreEqual(entity.Description, dbEntity.Description);
            Assert.AreEqual(entity.EndDate, dbEntity.EndDate);
            Assert.AreEqual(entity.Mode, dbEntity.Mode);
            Assert.AreEqual(entity.Name, dbEntity.Name);
            Assert.AreEqual(entity.Organizer, dbEntity.Organizer);
            Assert.AreEqual(entity.StartDate, dbEntity.StartDate);
            Assert.AreEqual(entity.TargetAmount, dbEntity.TargetAmount);
            Assert.AreEqual(entity.ValidityDate, dbEntity.ValidityDate);
        }

        #endregion

        #region Tests


        [Test]
        public void GetNewId_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetNewId());
        }

        [Test]
        public void Save_WhenIdCannotBeGenerated_ShouldReturnFalse()
        {
            var mock = CreateMock(false);
            mock.Setup(s => s.GetNewId()).Returns(-3);
            Assert.IsFalse(mock.Object.Save(CreateModel()));
        }

        [Test]
        public void GetAllEntities_ShouldReturnRightNumberOfRecords()
        {
            var firstPot = CreateModel();
            firstPot.Name = "FirstPot";
            var secondPot = CreateModel();
            secondPot.Name = "SecondPot";
            var thirdPot = CreateModel();
            thirdPot.Name = "ThirdPot";
            Assert.IsTrue(_importExport.Save(firstPot));
            Assert.IsTrue(_importExport.Save(secondPot));
            Assert.IsTrue(_importExport.Save(thirdPot));

            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(p => p.Id == firstPot.Id));
            Assert.IsTrue(list.Any(p => p.Id == secondPot.Id));
            Assert.IsTrue(list.Any(p => p.Name == thirdPot.Name));
        }

        [Test]
        public void GetTripsPot_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetTripsPot(-2));
        }

        [Test]
        public void GetTripsPot_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetTripsPot(-2));
        }

        [Test]
        public void GetTripsPot_WhenExist_ShouldReturnPot()
        {
            var pot = CreateModel();
            pot.TripId = 3;
            pot.Name = "PotForGetTripsPot";
            Assert.IsTrue(_importExport.Save(pot));
            var dbPot = _importExport.GetTripsPot(pot.TripId);
            Assert.IsNotNull(dbPot);
            CompareWithDbValues(pot, dbPot);
        }

        [Test]
        public void IsPotNameUsed_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.IsPotNameUsed("WrongName"));
        }

        [Test]
        public void IsPotNameUsed_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.IsPotNameUsed("PotNotExist"));
        }

        [Test]
        public void IsPotNameUsed_WhenExist_ShouldReturnTrue()
        {
            var pot = CreateModel();
            pot.Name = "PotIsPotNameUsed";
            Assert.IsTrue(_importExport.Save(pot));
            Assert.IsTrue(_importExport.IsPotNameUsed(pot.Name));
        }

        [Test]
        public void GetPotByName_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetPotByName("WrongName"));
        }

        [Test]
        public void GetPotByName_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetPotByName("WrongPotByName"));
        }

        [Test]
        public void GetPotByName_WhenExist_ShouldReturnPot()
        {
            var pot = CreateModel();
            pot.Name = "PotGetPotByName";
            Assert.IsTrue(_importExport.Save(pot));
            var dbPot = _importExport.GetPotByName(pot.Name);
            Assert.IsNotNull(dbPot);
            CompareWithDbValues(pot, dbPot);
        }

        #endregion

    }
}
