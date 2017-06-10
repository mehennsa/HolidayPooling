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
    public class FriendshipTest : ModelTestBase<Friendship>
    {

        #region ModelTestBase<Friendship>

        public override Friendship CreateModel()
        {
            return ModelTestHelper.CreateFriendship(1, "FriendForTest");
        }

        public override IList<object> GetValuesForHashCode(Friendship model)
        {
            return new List<object> { model.FriendName, model.UserId };
        }

        public override bool EqualsWithModel(Friendship model, Friendship other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(Friendship model, Friendship clone)
        {
            Assert.AreEqual(model.UserId, clone.UserId);
            Assert.AreEqual(model.FriendName, clone.FriendName);
            Assert.AreEqual(model.StartDate, clone.StartDate);
            Assert.AreEqual(model.IsRequested, clone.IsRequested);
            Assert.AreEqual(model.IsWaiting, clone.IsWaiting);
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

        [TestCase(1, "friend1", 1, "friend1", true)]
        [TestCase(1, "friend1", 2, "friend1", false)]
        [TestCase(1, "friend1", 1, "friend2", false)]
        [TestCase(1, "Friend1", 2, "Friend2", false)]
        public void Equals_ShouldReturnCorrectValue(int firstId, string firstFriendName, int secondId, string secondFriendName,
            bool expected)
        {
            var firstFriend = ModelTestHelper.CreateFriendship(firstId, firstFriendName);
            var secondFriend = ModelTestHelper.CreateFriendship(secondId, secondFriendName);
            Assert.AreEqual(expected, firstFriend.Equals(secondFriend));
        }

        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void Clone_ShouldDuplicateEveryValue()
        {
            var friendship = ModelTestHelper.CreateFriendship(3, "forClone", new DateTime(2017, 03, 05),
                true, false);
            TestClone(friendship);
            
        }

        #endregion

    }
}
