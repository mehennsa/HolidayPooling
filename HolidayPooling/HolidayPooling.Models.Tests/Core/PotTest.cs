﻿using HolidayPooling.Models.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class PotTest : ModelHavingSubModelTestBase<Pot, PotUser>
    {


        #region ModelHavingSubModelTestBase<Pot, PotUser>

        public override Pot CreateModel()
        {
            return ModelTestHelper.CreatePot(1, 2);
        }

        public override IList<object> GetValuesForHashCode(Pot model)
        {
            return new List<object> { model.Id };
        }

        public override bool EqualsWithModel(Pot model, Pot other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(Pot model, Pot clone)
        {
            Assert.AreEqual(model.Id, clone.Id);
            Assert.AreEqual(model.TripId, clone.TripId);
            Assert.AreEqual(model.Organizer, clone.Organizer);
            Assert.AreEqual(model.Mode, clone.Mode);
            Assert.AreEqual(model.Name, clone.Name);
            Assert.AreEqual(model.CurrentAmount, clone.CurrentAmount);
            Assert.AreEqual(model.TargetAmount, clone.TargetAmount);
            Assert.AreEqual(model.StartDate, clone.StartDate);
            Assert.AreEqual(model.EndDate, clone.EndDate);
            Assert.AreEqual(model.ValidityDate, clone.ValidityDate);
            Assert.AreEqual(model.Description, clone.Description);
            Assert.AreEqual(model.IsCancelled, clone.IsCancelled);
            Assert.AreEqual(model.CancellationReason, clone.CancellationReason);
            Assert.AreEqual(model.CancellationDate, clone.CancellationDate);
            Assert.AreEqual(model.ModificationDate, clone.ModificationDate);
            Assert.IsFalse(ReferenceEquals(model.Participants, clone.Participants));
            Assert.AreEqual(model.Participants.Count(), clone.Participants.Count());
            for(int i = 0; i < model.Participants.Count(); i++)
            {
                var pp = model.GetParticipantByIndex(i);
                var clonePp = clone.GetParticipantByIndex(i);
                Assert.IsFalse(ReferenceEquals(pp, clonePp));
                Assert.IsTrue(pp.Equals(clonePp));
            }
        }

        public override Pot CreateModelWithId(int id)
        {
            return ModelTestHelper.CreatePot(id, id + 1);
        }

        public override PotUser CreateSubModelWithId(int id, int secondId)
        {
            return ModelTestHelper.CreatePotUser(secondId, id);
        }

        public override void AddMethod(Pot model, PotUser subModel)
        {
            model.AddParticipant(subModel);
        }

        public override void DeleteMethod(Pot model, PotUser subModel)
        {
            model.DeleteParticipant(subModel);
        }

        public override void UpdateMethod(Pot model, PotUser subModel)
        {
            model.UpdateParticipant(subModel);
        }

        public override PotUser Get(Pot model, int id)
        {
            return model.GetParticipant(id);
        }

        public override PotUser GetByIndex(Pot model, int id)
        {
            return model.GetParticipantByIndex(id);
        }

        public override int CountSubModel(Pot model)
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
            var firstPot = ModelTestHelper.CreatePot(firstId, 1);
            var secondPot = ModelTestHelper.CreatePot(secondId, 2);
            Assert.AreEqual(expected, firstPot.Equals(secondPot));
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
            UpdateValid((p) => p.TargetAmount = 2000, (p1, p2) => p1.TargetAmount != p2.TargetAmount);
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
            TestClone(pot);
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
            var pot = ModelTestHelper.CreatePot(1, 1, participants: null);
            Assert.IsNull(pot.Participants);
            var clone = pot.Clone() as Pot;
            Assert.IsNotNull(clone);
            Assert.IsNotNull(clone.Participants);
        }

        #endregion

    }
}
