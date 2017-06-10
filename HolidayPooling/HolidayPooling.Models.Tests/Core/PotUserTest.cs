using HolidayPooling.Models.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class PotUserTest : ModelTestBase<PotUser>
    {

        #region ModelTestBase<PotUser>

        public override PotUser CreateModel()
        {
            return ModelTestHelper.CreatePotUser(2, 3);
        }

        public override IList<object> GetValuesForHashCode(PotUser model)
        {
            return new List<object> { model.UserId, model.PotId };
        }

        public override bool EqualsWithModel(PotUser model, PotUser other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(PotUser model, PotUser clone)
        {
            Assert.AreEqual(model.UserId, clone.UserId);
            Assert.AreEqual(model.PotId, clone.PotId);
            Assert.AreEqual(model.HasPayed, clone.HasPayed);
            Assert.AreEqual(model.Amount, clone.Amount);
            Assert.AreEqual(model.TargetAmount, clone.TargetAmount);
            Assert.AreEqual(model.HasCancelled, clone.HasCancelled);
            Assert.AreEqual(model.CancellationReason, clone.CancellationReason);
            Assert.AreEqual(model.HasValidated, clone.HasValidated);
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

        [TestCase(1, 4, 1, 4, true)]
        [TestCase(1, 2, 1, 3, false)]
        [TestCase(1, 1, 2, 1, false)]
        [TestCase(1, 2, 3, 4, false)]
        public void Equals_ShouldReturnCorrectValue(int firstUserId, int firstPotId, int secondUserId, int secondPotId, bool expected)
        {
            var firstPotUser = ModelTestHelper.CreatePotUser(firstUserId, firstPotId);
            var secondPotUser = ModelTestHelper.CreatePotUser(secondUserId, secondPotId);
            Assert.AreEqual(expected, firstPotUser.Equals(secondPotUser));
        }


        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void Clone_ShoulDuplicateValue()
        {
            var potUser = ModelTestHelper.CreatePotUser(1, 3, true, 300, 1200, true, "because", false);
            TestClone(potUser);
        }

        #endregion

    }
}
