using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;

namespace HolidayPooling.DataRepositories.Tests.Business
{
    [TestFixture]
    public class TripParticipantDbImportExportTest :
        DbImportExportTestBase<TripParticipantDbImportExport, TripParticipantKey, TripParticipant>
    {

        #region DbImportExportTestBase<TripParticipantDbImportExport, TripParticipantKey, TripParticipant>

        protected override string TableName
        {
            get { return "TTRPPTP"; }
        }

        protected override TripParticipant CreateModel()
        {
            return ModelTestHelper.CreateTripParticipant(1, "APseudo");
        }

        protected override TripParticipantKey GetKeyFromModel(TripParticipant entity)
        {
            return new TripParticipantKey(entity.TripId, entity.UserPseudo);
        }

        protected override void UpdateModel(TripParticipant model)
        {
            model.TripNote += 1.2;
            model.HasParticipated = !model.HasParticipated;
        }

        public override void CompareWithDbValues(TripParticipant entity, TripParticipant dbEntity)
        {
            Assert.AreEqual(entity.TripId, dbEntity.TripId);
            Assert.AreEqual(entity.UserPseudo, dbEntity.UserPseudo);
            Assert.AreEqual(entity.TripNote, dbEntity.TripNote);
            Assert.AreEqual(entity.ValidationDate, dbEntity.ValidationDate);
            Assert.AreEqual(entity.HasParticipated, dbEntity.HasParticipated);
        }

        #endregion

        #region Tests

        [Test]
        public void GetTripParticipants_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetParticipantsForTrip(-1));
        }

        [Test]
        public void GetParticipantsForTrip_ShouldReturnOnlyTripParticipant()
        {
            var firstPp = ModelTestHelper.CreateTripParticipant(1, "pseudo1");
            var secondPp = ModelTestHelper.CreateTripParticipant(2, "pseudo2");
            var thridPp = ModelTestHelper.CreateTripParticipant(1, "pseudo3");
            Assert.IsTrue(_importExport.Save(firstPp));
            Assert.IsTrue(_importExport.Save(secondPp));
            Assert.IsTrue(_importExport.Save(thridPp));
            var list = _importExport.GetParticipantsForTrip(1);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(p => p.UserPseudo == "pseudo1"));
            Assert.IsTrue(list.Any(p => p.UserPseudo == "pseudo3"));
            Assert.IsFalse(list.Any(p => p.UserPseudo == "pseudo2"));
            Assert.IsFalse(list.Any(p => p.TripId == 2));
        }

        [Test]
        public void GetAllEntities_ShouldReturnAllParticipants()
        {
            var firstPp = ModelTestHelper.CreateTripParticipant(1, "pseudo1");
            var secondPp = ModelTestHelper.CreateTripParticipant(2, "pseudo2");
            var thridPp = ModelTestHelper.CreateTripParticipant(1, "pseudo3");
            Assert.IsTrue(_importExport.Save(firstPp));
            Assert.IsTrue(_importExport.Save(secondPp));
            Assert.IsTrue(_importExport.Save(thridPp));
            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(p => p.UserPseudo == "pseudo1"));
            Assert.IsTrue(list.Any(p => p.UserPseudo == "pseudo3"));
            Assert.IsTrue(list.Any(p => p.UserPseudo == "pseudo2"));
            Assert.IsTrue(list.Any(p => p.TripId == 2));
            Assert.IsTrue(list.Any(p => p.TripId == 1));
        }

        #endregion
    }
}
