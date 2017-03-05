using HolidayPooling.Models.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class TripTest : ModelHavingSubModelTestBase<Trip, TripParticipant>
    {

        #region ModelHavingSubModelTestBase<Trip, TripParticipant>

        public override Trip CreateModel()
        {
            return ModelTestHelper.CreateTrip(1, "myTrip");
        }

        public override IList<object> GetValuesForHashCode(Trip model)
        {
            return new List<object> { model.Id };
        }

        public override bool EqualsWithModel(Trip model, Trip other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(Trip model, Trip clone)
        {
            Assert.AreEqual(model.Id, clone.Id);
            Assert.AreEqual(model.TripName, clone.TripName);
            Assert.AreEqual(model.Price, clone.Price);
            Assert.AreEqual(model.Description, clone.Description);
            Assert.AreEqual(model.NumberMaxOfPeople, clone.NumberMaxOfPeople);
            Assert.AreEqual(model.Location, clone.Location);
            Assert.AreEqual(model.Organizer, clone.Organizer);
            Assert.AreEqual(model.StartDate, clone.StartDate);
            Assert.AreEqual(model.EndDate, clone.EndDate);
            Assert.AreEqual(model.ValidityDate, clone.ValidityDate);
            Assert.AreEqual(model.Note, clone.Note);
            Assert.IsFalse(ReferenceEquals(model.TripPot, clone.TripPot));
            Assert.IsFalse(ReferenceEquals(model.Participants, clone.Participants));
            Assert.AreEqual(model.Participants.Count(), clone.Participants.Count());
            for (int i = 0; i < model.Participants.Count(); i++)
            {
                var pp = model.GetParticipantByIndex(i);
                var clonePp = clone.GetParticipantByIndex(i);
                Assert.IsFalse(ReferenceEquals(pp, clonePp));
            }

            Assert.AreEqual(model.Description, model.ToString());
        }

        public override Trip CreateModelWithId(int id)
        {
            return ModelTestHelper.CreateTrip(1, "tripWithId");
        }

        public override TripParticipant CreateSubModelWithId(int id, int secondId)
        {
            var pseudo = secondId.ToString() + "UserPseudo";
            return ModelTestHelper.CreateTripParticipant(id, pseudo);
        }

        public override void AddMethod(Trip model, TripParticipant subModel)
        {
            model.AddParticipant(subModel);
        }

        public override void DeleteMethod(Trip model, TripParticipant subModel)
        {
            model.DeleteParticipant(subModel);
        }

        public override void UpdateMethod(Trip model, TripParticipant subModel)
        {
            model.UpdateParticipant(subModel);
        }

        public override TripParticipant Get(Trip model, int id)
        {
            var pseudo = id.ToString() + "UserPseudo";
            return model.GetParticipant(pseudo);
        }

        public override TripParticipant GetByIndex(Trip model, int id)
        {
            return model.GetParticipantByIndex(id);
        }

        public override int CountSubModel(Trip model)
        {
            return model.Participants.Count();
        }

        #endregion

        #region Tests

        [Test]
        public void Equals_WithNullObject_ShouldReturnFalse()
        {
            TestEqualsWithNullObject();
        }

        [Test]
        public void Equals_WhenObjectHaveSameReference_ShouldReturnTrue()
        {
            TestEqualsWithObjectHavingSameReference();
        }

        [Test]
        public void Equals_WhenObjectHaveDifferentType_ShouldReturnFalse()
        {
            TestEqualsWithDifferentType();
        }

        [Test]
        public void Equals_WhenObjectHaveSameValues_ShouldReturnTrue()
        {
            TestEqualsWithObjectHavingSameType();
        }

        [Test]
        public void Equals_WhenModelIsNull_ShouldReturnFalse()
        {
            TestEqualsWithNullModel();
        }

        [Test]
        public void Equals_WhenModelHaveSameReference_ShouldReturnTrue()
        {
            TestEqualsWithModelHavingSameReferences();
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void Equals_ShouldReturnCorrectValue(int firstId, int secondId,
            bool expected)
        {
            var firstTrip = ModelTestHelper.CreateTrip(firstId, "TripName");
            var secondTrip = ModelTestHelper.CreateTrip(secondId, "SecondTrip");
            Assert.AreEqual(expected, firstTrip.Equals(secondTrip));
        }

        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void AddParticipant_WhenExist_ShouldNotAddIt()
        {
            AddAlreadyExist();
        }

        [Test]
        public void AddParticipant_WhenWrongId_ShouldNotAddIt()
        {
            AddWithWrongId();
        }

        [Test]
        public void DeleteParticipant_WithWrongId_ShouldNotDeleteIt()
        {
            DeleteWithWrongId();
        }

        [Test]
        public void DeleteParticipant_WhenNotExist_ShouldNotDeleteIt()
        {
            DeleteNotExist();
        }

        [Test]
        public void DeleteParticipant_ShoulDeleteTheParticipant()
        {
            DeleteValid();
        }

        [Test]
        public void UpdateParticipant_WithWrongId_ShouldNotUpdate()
        {
            UpdateWithWrongId();
        }

        [Test]
        public void Update_WhenNotExist_ShouldNotUpdate()
        {
            UpdateNotExist();
        }

        [Test]
        public void Update_ShouldUpdateParticipant()
        {
            UpdateValid((p) => p.TripNote = 7.3, (p1, p2) => p1.TripNote != p2.TripNote);
        }

        [Test]
        public void GetByIndex_ShouldReturnValidParticipant()
        {
            GetByIndexValid();
        }

        [Test]
        public void GetByIndex_WhenIndexNegative_ShouldReturnNull()
        {
            GetByIndexWhenNegative();
        }

        [Test]
        public void GetByIndex_WhenNoParticipant_ShouldReturnNull()
        {
            GetByIndexWithNoSubModel();
        }

        [Test]
        public void GetByIndex_WhenIndexGreaterThanParticipantsCount_ShouldReturnNull()
        {
            GetByIndexWhenIndexGreaterThanCount();
        }

        [Test]
        public void Clone_ShoulDuplicateValues()
        {
            var pot = ModelTestHelper.CreatePot(1, 1, "cloneOrga", name: "ClonePot");
            pot.AddParticipant(ModelTestHelper.CreatePotUser(1, 1));
            pot.AddParticipant(ModelTestHelper.CreatePotUser(2, 1));
            var trip = ModelTestHelper.CreateTrip(1, "MyTrip", maxNbPeople: 20, note: 6.4);
            trip.TripPot = pot;
            trip.AddParticipant(ModelTestHelper.CreateTripParticipant(1, "toto"));
            trip.AddParticipant(ModelTestHelper.CreateTripParticipant(1, "tutu"));
            TestClone(trip);
        }

        [Test]
        public void GetParticipant_WhenNotExist_ShouldReturnNull()
        {
            GetNotExist();
        }

        [Test]
        public void GetParticipant_WhenExist_ShouldReturnTheParticipant()
        {
            GetValid();
        }

        [Test]
        public void Clone_WhenParticipantsIsNull_ShouldCreateNewList()
        {
            var trip = ModelTestHelper.CreateTrip(1, "testTrip");
            trip.Participants = null;
            trip.TripPot = null;
            Assert.IsNull(trip.Participants);
            Assert.IsNull(trip.TripPot);
            var clone = trip.Clone() as Trip;
            Assert.IsNotNull(clone);
            Assert.IsNotNull(clone.Participants);
            Assert.IsNotNull(clone.TripPot);
        }

        #endregion
    
    }
}
