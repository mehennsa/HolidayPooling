using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Models.Tests.Core
{
    public abstract class ModelHavingSubModelTestBase<T, U> : ModelTestBase<T>
        where T: class, ICloneable where U : class, ICloneable
    {
        public abstract override T CreateModel();

        public abstract override IList<object> GetValuesForHashCode(T model);

        public abstract override bool EqualsWithModel(T model, T other);

        public abstract override void CompareClone(T model, T clone);

        public abstract T CreateModelWithId(int id);

        public abstract U CreateSubModelWithId(int id, int secondId);

        public abstract void AddMethod(T model, U subModel);

        public abstract void DeleteMethod(T model, U subModel);

        public abstract void UpdateMethod(T model, U subModel);

        public abstract U Get(T model, int id);

        public abstract U GetByIndex(T model, int id);

        public abstract int CountSubModel(T model);

        #region Helper

        public void AddWithWrongId()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(3, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(0, CountSubModel(model));
        }

        public void AddAlreadyExist()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
        }

        public void DeleteWithWrongId()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            var secondSubModel = CreateSubModelWithId(2, 1);
            DeleteMethod(model, secondSubModel);
            Assert.IsNotNull(Get(model, 1));
        }

        public void DeleteNotExist()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 2);
            Assert.AreEqual(0, CountSubModel(model));
            DeleteMethod(model, subModel);
            Assert.AreEqual(0, CountSubModel(model));
        }

        public void DeleteValid()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
            DeleteMethod(model, subModel);
            Assert.AreEqual(0, CountSubModel(model));
        }

        public void UpdateWithWrongId()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            var secondSubModel = CreateSubModelWithId(2, 1);
            UpdateMethod(model, secondSubModel);
            var copy = GetByIndex(model, 0);
            Assert.IsNotNull(copy);
            Assert.IsTrue(subModel.Equals(copy));
        }

        public void UpdateNotExist()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            Assert.AreEqual(0, CountSubModel(model));
            UpdateMethod(model, subModel);
            Assert.AreEqual(0, CountSubModel(model));
        }

        public void UpdateValid(Action<U> updateHandler, Func<U, U, bool> comparer)
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
            var clone = subModel.Clone() as U;
            Assert.IsNotNull(clone);
            updateHandler(subModel);
            UpdateMethod(model, subModel);
            Assert.IsTrue(comparer(clone, subModel));
        }

        public void GetByIndexValid()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
            Assert.IsNotNull(GetByIndex(model, 0));
        }

        public void GetByIndexWhenNegative()
        {
            T model = CreateModel();
            Assert.IsNull(GetByIndex(model, -1));
        }

        public void GetByIndexWithNoSubModel()
        {
            T model = CreateModel();
            Assert.IsNull(GetByIndex(model, 0));
        }

        public void GetByIndexWhenIndexGreaterThanCount()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 1);
            AddMethod(model, subModel);
            Assert.AreEqual(1, CountSubModel(model));
            Assert.IsNull(GetByIndex(model, 5));
        }

        public void GetNotExist()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 2);
            Assert.IsNull(Get(model, 1));
        }

        public void GetValid()
        {
            T model = CreateModelWithId(1);
            U subModel = CreateSubModelWithId(1, 2);
            AddMethod(model, subModel);
            Assert.IsNotNull(Get(model, 2));
        }

        #endregion

    }
}
