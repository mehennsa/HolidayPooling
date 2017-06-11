using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace HolidayPooling.Models.Tests.Core
{
    public abstract class ModelTestBase<T> where T : class, ICloneable
    {

        #region Methods

        public abstract T CreateModel();

        public abstract IList<object> GetValuesForHashCode(T model);

        public abstract bool EqualsWithModel(T model, T other);

        public abstract void CompareClone(T model, T clone);

        #endregion

        #region Helpers

        public void TestEqualsWithNullObject()
        {
            var model = CreateModel();
            object obj = null;
            Assert.IsFalse(model.Equals(obj));
        }

        public void TestEqualsWithObjectHavingSameReference()
        {
            var model = CreateModel();
            object obj = model;
            Assert.IsTrue(model.Equals(obj));
        }

        public void TestEqualsWithDifferentType()
        {
            var model = CreateModel();
            object other = string.Empty;
            Assert.IsFalse(model.Equals(other));
        }

        public void TestEqualsWithObjectHavingSameType()
        {
            var model = CreateModel();
            object other = CreateModel();
            Assert.IsFalse(ReferenceEquals(model, other));
            Assert.IsTrue(model.Equals(other));
        }

        public void TestEqualsWithNullModel()
        {
            var model = CreateModel();
            T other = null;
            Assert.False(EqualsWithModel(model, other));
        }

        public void TestEqualsWithModelHavingSameReferences()
        {
            var model = CreateModel();
            T other = model;
            Assert.IsTrue(EqualsWithModel(model, other));
        }

        public void TestHashCode()
        {
            unchecked
            {
                var model = CreateModel();
                int hash = (int)HashCodeHelper.HashConstant;
                var list = GetValuesForHashCode(model);
                foreach (var v in list)
                {
                    hash = HashCodeHelper.GetUnitaryHashcode(hash, v);
                }

                Assert.AreEqual(hash, model.GetHashCode());

            }

        }

        public void TestClone(T model)
        {
            var clone = model.Clone() as T;
            Assert.IsNotNull(clone);
            Assert.IsFalse(ReferenceEquals(model, clone));
            CompareClone(model, clone);
        }

        #endregion

    }
}
