using HolidayPooling.Models.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using HolidayPooling.Tests;

namespace HolidayPooling.Models.Tests.Core
{

    [TestFixture]
    public class UserWithTripTest : ModelHavingSubModelTestBase<User, UserTrip>
    {

        #region ModelHavingSubModelTestBase<User, UserTrip>

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
            throw new NotImplementedException();
        }

        public override User CreateModelWithId(int id)
        {
            return ModelTestHelper.CreateUser(id, "WithId");
        }

        public override UserTrip CreateSubModelWithId(int id, int secondId)
        {
            var tripName = secondId.ToString() + "TripName";
            return ModelTestHelper.CreateUserTrip(id, tripName);
        }

        public override void AddMethod(User model, UserTrip subModel)
        {
            model.AddTrip(subModel);
        }

        public override void DeleteMethod(User model, UserTrip subModel)
        {
            model.DeleteTrip(subModel);
        }

        public override void UpdateMethod(User model, UserTrip subModel)
        {
            model.UpdateTrip(subModel);
        }

        public override UserTrip Get(User model, int id)
        {
            var name = id.ToString() + "TripName";
            return model.GetTrip(name);
        }

        public override UserTrip GetByIndex(User model, int id)
        {
            return model.GetTrip(id);
        }

        public override int CountSubModel(User model)
        {
            return model.Trips.Count();
        }

        #endregion

        #region Tests

        [Test]
        public void AddTrip_WhenExist_ShouldNotAddIt()
        {
            AddAlreadyExist();
        }

        [Test]
        public void AddTrip_WhenWrongId_ShouldNotAddIt()
        {
            AddWithWrongId();
        }

        [Test]
        public void DeleteTrip_WithWrongId_ShouldNotDeleteIt()
        {
            DeleteWithWrongId();
        }

        [Test]
        public void DeleteTrip_WhenNotExist_ShouldNotDeleteIt()
        {
            DeleteNotExist();
        }

        [Test]
        public void DeleteTrip_ShoulDeleteTheTrip()
        {
            DeleteValid();
        }

        [Test]
        public void UpdateTrip_WithWrongId_ShouldNotUpdate()
        {
            UpdateWithWrongId();
        }

        [Test]
        public void Update_WhenNotExist_ShouldNotUpdate()
        {
            UpdateNotExist();
        }

        [Test]
        public void Update_ShouldUpdateTrip()
        {
            UpdateValid((t) => t.TripAmount = 854.12, (t1, t2) => t1.TripAmount != t2.TripAmount);
        }

        [Test]
        public void GetByIndex_ShouldReturnValidTrip()
        {
            GetByIndexValid();
        }

        [Test]
        public void GetByIndex_WhenIndexNegative_ShouldReturnNull()
        {
            GetByIndexWhenNegative();
        }

        [Test]
        public void GetByIndex_WhenNoTrip_ShouldReturnNull()
        {
            GetByIndexWithNoSubModel();
        }

        [Test]
        public void GetByIndex_WhenIndexGreaterThanTripsCount_ShouldReturnNull()
        {
            GetByIndexWhenIndexGreaterThanCount();
        }

        #endregion

    }
}
