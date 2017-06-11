using HolidayPooling.Models.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class TripParticipantTest : ModelTestBase<TripParticipant>
    {

        #region ModelTestBase<TripParticipant>

        public override TripParticipant CreateModel()
        {
            return ModelTestHelper.CreateTripParticipant(1, "UserPseudoForTest");
        }

        public override IList<object> GetValuesForHashCode(TripParticipant model)
        {
            return new List<object> { model.UserPseudo, model.TripId };
        }

        public override bool EqualsWithModel(TripParticipant model, TripParticipant other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(TripParticipant model, TripParticipant clone)
        {
            Assert.AreEqual(model.TripId, clone.TripId);
            Assert.AreEqual(model.UserPseudo, clone.UserPseudo);
            Assert.AreEqual(model.HasParticipated, clone.HasParticipated);
            Assert.AreEqual(model.TripNote, clone.TripNote);
            Assert.AreEqual(model.ValidationDate, clone.ValidationDate);
            Assert.AreEqual(model.ModificationDate, clone.ModificationDate);
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

        [TestCase(1, "pseudo1", 1, "pseudo1", true)]
        [TestCase(1, "pseudo1", 2, "pseudo1", false)]
        [TestCase(1, "pseudo1", 1, "pseudo2", false)]
        [TestCase(1, "pseudo1", 2, "pseudo2", false)]
        public void Equals_ShouldReturnCorrectValue(int firstId, string firstPseudo, int secondId, string secondPseudo,
            bool expected)
        {
            var firstPtp = ModelTestHelper.CreateTripParticipant(firstId, firstPseudo);
            var secondPtp = ModelTestHelper.CreateTripParticipant(secondId, secondPseudo);
            Assert.AreEqual(expected, firstPtp.Equals(secondPtp));
        }

        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void Clone_ShouldDuplicateValues()
        {
            var ptp = ModelTestHelper.CreateTripParticipant(1, "forClone", true, 4.5,
                new DateTime(2017, 03, 06));
            TestClone(ptp);
        }

        #endregion

    }
}
