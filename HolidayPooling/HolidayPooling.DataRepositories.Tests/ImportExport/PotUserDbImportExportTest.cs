using HolidayPooling.DataRepositories.ImportExport;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;
using NUnit.Framework;
using System;
using System.Linq;

namespace HolidayPooling.DataRepositories.Tests.ImportExport
{
    [TestFixture]
    public class PotUserDbImportExportTest : DbImportExportTestBase<PotUserDbImportExport, PotUserKey, PotUser>
    {

        #region DbImportExportTestBase<PotUserDbImportExport, PotUserKey, PotUser>

        protected override string TableName
        {
            get { return "TPOTUSR"; }
        }

        protected override PotUser CreateModel()
        {
            return ModelTestHelper.CreatePotUser(1, 2);
        }

        protected override PotUserKey GetKeyFromModel(PotUser entity)
        {
            return new PotUserKey(entity.PotId, entity.UserId);
        }

        protected override void UpdateModel(PotUser model)
        {
            model.TargetAmount += 405.3435;
            model.HasCancelled = !model.HasCancelled;
        }

        public override void CompareWithDbValues(PotUser entity, PotUser dbEntity, DateTime modificationDate)
        {
            Assert.AreEqual(entity.PotId, dbEntity.PotId);
            Assert.AreEqual(entity.UserId, dbEntity.UserId);
            Assert.AreEqual(entity.HasPayed, dbEntity.HasPayed);
            Assert.AreEqual(entity.HasValidated, dbEntity.HasValidated);
            Assert.AreEqual(entity.Amount, dbEntity.Amount);
            Assert.AreEqual(entity.TargetAmount, dbEntity.TargetAmount);
            Assert.AreEqual(entity.HasCancelled, dbEntity.HasCancelled);
            Assert.AreEqual(entity.CancellationReason, dbEntity.CancellationReason);
            Assert.AreEqual(modificationDate, dbEntity.ModificationDate);
        }

        protected override PotUserDbImportExport CreateImportExport()
        {
            return new PotUserDbImportExport(new MockTimeProvider(_insertTime));
        }

        protected override PotUserDbImportExport CreateImportExportForUpdate()
        {
            return new PotUserDbImportExport(new MockTimeProvider(_updateTime));
        }

        #endregion

        #region Tests

        [Test]
        public void GetPotUsers_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetPotUsers(-1));
        }

        [Test]
        public void GetPotUsers_ShouldReturnRightNumberOfRecord()
        {
            var firstPotUser = ModelTestHelper.CreatePotUser(2, 1);
            var secondPotUser = ModelTestHelper.CreatePotUser(3, 1);
            var thirdPotUser = ModelTestHelper.CreatePotUser(3, 2);
            Assert.IsTrue(_importExport.Save(firstPotUser));
            Assert.IsTrue(_importExport.Save(secondPotUser));
            Assert.IsTrue(_importExport.Save(thirdPotUser));

            var list = _importExport.GetPotUsers(1);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(p => p.UserId == 2));
            Assert.IsTrue(list.Any(p => p.UserId == 3));
            Assert.IsFalse(list.Any(p => p.PotId == 2));
        }

        [Test]
        public void GetUserPots_WhenExceptionThrown_ShouldThrowArgumentNullException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.GetUserPots(-1));
        }

        [Test]
        public void GetUserPots_ShouldReturnRightNumberOfRecords()
        {
            var firstPotUser = ModelTestHelper.CreatePotUser(2, 1);
            var secondPotUser = ModelTestHelper.CreatePotUser(3, 1);
            var thirdPotUser = ModelTestHelper.CreatePotUser(3, 2);
            Assert.IsTrue(_importExport.Save(firstPotUser));
            Assert.IsTrue(_importExport.Save(secondPotUser));
            Assert.IsTrue(_importExport.Save(thirdPotUser));

            var list = _importExport.GetUserPots(3);
            Assert.AreEqual(2, list.Count());
            Assert.IsFalse(list.Any(p => p.UserId == 2));
            Assert.IsTrue(list.Any(p => p.PotId == 2));
            Assert.IsTrue(list.Any(p => p.PotId == 1));
        }

        [Test]
        public void GetAllEntities_ShouldReturnRightNumberOfRecords()
        {
            var firstPotUser = ModelTestHelper.CreatePotUser(2, 1);
            var secondPotUser = ModelTestHelper.CreatePotUser(3, 1);
            var thirdPotUser = ModelTestHelper.CreatePotUser(3, 2);
            Assert.IsTrue(_importExport.Save(firstPotUser));
            Assert.IsTrue(_importExport.Save(secondPotUser));
            Assert.IsTrue(_importExport.Save(thirdPotUser));

            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(p => p.UserId == 2));
            Assert.IsTrue(list.Any(p => p.UserId == 3));
            Assert.IsTrue(list.Any(p => p.PotId == 2));
            Assert.IsTrue(list.Any(p => p.PotId == 1));
        }

        #endregion

    }
}
