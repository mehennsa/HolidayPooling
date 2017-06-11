using HolidayPooling.Models.Core;
using NUnit.Framework;
using System.Collections.Generic;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class UserTripTest : ModelTestBase<UserTrip>
    {

        #region ModelTestBase<UserTrip>

        public override UserTrip CreateModel()
        {
            return ModelTestHelper.CreateUserTrip(1, "TripForTest");
        }

        public override IList<object> GetValuesForHashCode(UserTrip model)
        {
            return new List<object> { model.TripName, model.UserId };
        }

        public override bool EqualsWithModel(UserTrip model, UserTrip other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(UserTrip model, UserTrip clone)
        {
            Assert.AreEqual(model.UserId, clone.UserId);
            Assert.AreEqual(model.TripName, clone.TripName);
            Assert.AreEqual(model.HasOrganized, clone.HasOrganized);
            Assert.AreEqual(model.HasParticipated, clone.HasParticipated);
            Assert.AreEqual(model.UserNote, clone.UserNote);
            Assert.AreEqual(model.TripAmount, clone.TripAmount);
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

        [TestCase(1, "trip1", 1, "trip1", true)]
        [TestCase(1, "trip1", 2, "trip1", false)]
        [TestCase(1, "trip1", 1, "trip2", false)]
        [TestCase(1, "trip1", 2, "trip2", false)]
        public void Equals_ShouldReturnCorrectValue(int firstId, string firstTripName, int secondId, string secondTripName,
            bool expected)
        {
            var firstUserTrip = ModelTestHelper.CreateUserTrip(firstId, firstTripName);
            var secondUserTrip = ModelTestHelper.CreateUserTrip(secondId, secondTripName);
            Assert.AreEqual(expected, firstUserTrip.Equals(secondUserTrip));
        }

        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void Clone_ShouldDuplicateValues()
        {
            var userTrip = ModelTestHelper.CreateUserTrip(1, "ForClone", false, true, 3.2, 600.75);
            TestClone(userTrip);
        }

        #endregion


    }
}
