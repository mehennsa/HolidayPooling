using HolidayPooling.Models.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{
    [TestFixture]
    public class UserTest : ModelHavingSubModelTestBase<User, Friendship>
    {

        #region ModelHavingSubModelTestBase<User, Friendship>

        public override User CreateModel()
        {
            return ModelTestHelper.CreateUser(1, "myUser");
        }

        public override IList<object> GetValuesForHashCode(User model)
        {
            return new List<object> { model.Id };
        }

        public override bool EqualsWithModel(User model, User other)
        {
            return model.Equals(other);
        }

        public override void CompareClone(User model, User clone)
        {
            Assert.AreEqual(model.Id, clone.Id);
            Assert.AreEqual(model.Mail, clone.Mail);
            Assert.AreEqual(model.Password, clone.Password);
            Assert.AreEqual(model.Pseudo, clone.Pseudo);
            Assert.AreEqual(model.Age, clone.Age);
            Assert.AreEqual(model.Description, clone.Description);
            Assert.AreEqual(model.Role, clone.Role);
            Assert.AreEqual(model.CreationDate, clone.CreationDate);
            Assert.AreEqual(model.PhoneNumber, clone.PhoneNumber);
            Assert.AreEqual(model.Type, clone.Type);
            Assert.AreEqual(model.Note, clone.Note);
            Assert.AreEqual(model.ModificationDate, clone.ModificationDate);
            Assert.IsFalse(ReferenceEquals(model.Friends, clone.Friends));
            Assert.AreEqual(model.Friends.Count(), clone.Friends.Count());
            for (int i = 0; i < model.Friends.Count(); i++)
            {
                var friend = model.GetFriend(i);
                var cloneFriend = clone.GetFriend(i);
                Assert.IsFalse(ReferenceEquals(friend, cloneFriend));
            }
            Assert.IsFalse(ReferenceEquals(model.CenterOfInterests, clone.CenterOfInterests));
            Assert.AreEqual(model.CenterOfInterests.Count, clone.CenterOfInterests.Count);
            Assert.IsFalse(ReferenceEquals(model.Trips, clone.Trips));
            Assert.AreEqual(model.Trips.Count(), clone.Trips.Count());
            for (int i = 0; i < model.Trips.Count(); i++)
            {
                var trip = model.GetTrip(i);
                var clonedTrip = clone.GetTrip(i);
                Assert.IsFalse(ReferenceEquals(trip, clonedTrip));
            }

        }

        public override User CreateModelWithId(int id)
        {
            return ModelTestHelper.CreateUser(id, "WithId");
        }

        public override Friendship CreateSubModelWithId(int id, int secondId)
        {
            var name = secondId.ToString() + "Friend";
            return ModelTestHelper.CreateFriendship(id, name);
        }

        public override void AddMethod(User model, Friendship subModel)
        {
            model.AddFriend(subModel);
        }

        public override void DeleteMethod(User model, Friendship subModel)
        {
            model.DeleteFriend(subModel);
        }

        public override void UpdateMethod(User model, Friendship subModel)
        {
            model.UpdateFriend(subModel);
        }

        public override Friendship Get(User model, int id)
        {
            var name = id.ToString() + "Friend";
            return model.GetFriend(name);
        }

        public override Friendship GetByIndex(User model, int id)
        {
            return model.GetFriend(id);
        }

        public override int CountSubModel(User model)
        {
            return model.Friends.Count();
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
            var firstUser = ModelTestHelper.CreateUser(firstId, "FirstUser");
            var secondUser = ModelTestHelper.CreateUser(secondId, "SecondUser");
            Assert.AreEqual(expected, firstUser.Equals(secondUser));
        }

        [Test]
        public void GetHashCode_ShouldReturnCorrectValue()
        {
            TestHashCode();
        }

        [Test]
        public void AddFriendship_WhenExist_ShouldNotAddIt()
        {
            AddAlreadyExist();
        }

        [Test]
        public void AddFriendship_WhenWrongId_ShouldNotAddIt()
        {
            AddWithWrongId();
        }

        [Test]
        public void DeleteFriendship_WithWrongId_ShouldNotDeleteIt()
        {
            DeleteWithWrongId();
        }

        [Test]
        public void DeleteFriendship_WhenNotExist_ShouldNotDeleteIt()
        {
            DeleteNotExist();
        }

        [Test]
        public void DeleteFriendship_ShoulDeleteTheFriendship()
        {
            DeleteValid();
        }

        [Test]
        public void UpdateFriendship_WithWrongId_ShouldNotUpdate()
        {
            UpdateWithWrongId();
        }

        [Test]
        public void Update_WhenNotExist_ShouldNotUpdate()
        {
            UpdateNotExist();
        }

        [Test]
        public void Update_ShouldUpdateFriendship()
        {
            UpdateValid((f) => f.IsWaiting = !f.IsWaiting, (f1, f2) => f1.IsWaiting != f2.IsWaiting);
        }

        [Test]
        public void GetByIndex_ShouldReturnValidFriendship()
        {
            GetByIndexValid();
        }

        [Test]
        public void GetByIndex_WhenIndexNegative_ShouldReturnNull()
        {
            GetByIndexWhenNegative();
        }

        [Test]
        public void GetByIndex_WhenNoFriendship_ShouldReturnNull()
        {
            GetByIndexWithNoSubModel();
        }

        [Test]
        public void GetByIndex_WhenIndexGreaterThanFriendshipsCount_ShouldReturnNull()
        {
            GetByIndexWhenIndexGreaterThanCount();
        }

        [Test]
        public void Clone_ShoulDuplicateValues()
        {
            var user = ModelTestHelper.CreateUser(1, "Clone", mail: "CloneEmail", role: Role.Common,
                phoneNumber: "ClonePhoneNumber");
            user.CenterOfInterests.Add("ACenterOfInterest");
            user.CenterOfInterests.Add("Tennis");
            user.AddFriend(ModelTestHelper.CreateFriendship(1, "AFriend"));
            user.AddFriend(ModelTestHelper.CreateFriendship(1, "TOTO"));
            user.AddTrip(ModelTestHelper.CreateUserTrip(1, "ATrip"));
            user.AddTrip(ModelTestHelper.CreateUserTrip(1, "SecondTrip"));
            TestClone(user);
        }

        [Test]
        public void GetFriendship_WhenNotExist_ShouldReturnNull()
        {
            GetNotExist();
        }

        [Test]
        public void GetFriendship_WhenExist_ShouldReturnTheFriendship()
        {
            GetValid();
        }

        [Test]
        public void Clone_WhenFriendshipsIsNull_ShouldCreateNewList()
        {
            var user = ModelTestHelper.CreateUser(1, "APseudo", centerOfInterest: null, friends: null, trips: null);
            Assert.IsNull(user.CenterOfInterests);
            Assert.IsNull(user.Friends);
            Assert.IsNull(user.Trips);
            var clone = user.Clone() as User;
            Assert.IsNotNull(clone);
            Assert.IsNotNull(clone.CenterOfInterests);
            Assert.IsNotNull(clone.Friends);
            Assert.IsNotNull(clone.Trips);
        }

        [Test]
        public void SetCenterOfInterest_ShouldWork()
        {
            var user = ModelTestHelper.CreateUser(1, "CenterPseudo");
            Assert.IsNotNull(user.CenterOfInterests);
            Assert.AreEqual(0, user.CenterOfInterests.Count);
            var list = new List<string> { "Foot", "Tennis" };
            user.CenterOfInterests = list;
            Assert.AreEqual(2, user.CenterOfInterests.Count);
        }

        [Test]
        public void CopyConstructor_ShouldCopyValues()
        {
            var user = ModelTestHelper.CreateUser(1, "CopyPseudo");
            var copy = new User(user);
            Assert.IsFalse(ReferenceEquals(user, copy));
            Assert.IsTrue(ReferenceEquals(user.Friends, copy.Friends));
        }

        [Test]
        public void ToString_ShouldUseAgeAndPseudo()
        {
            var user = ModelTestHelper.CreateUser(1, "ToStringPseudo");
            var expected = string.Format("{0}, {1}", user.Pseudo, user.Age);
            Assert.AreEqual(expected, user.ToString());
        }

        #endregion

    }
}
