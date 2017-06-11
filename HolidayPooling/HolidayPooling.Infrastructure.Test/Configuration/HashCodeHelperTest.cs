using HolidayPooling.Infrastructure.Configuration;
using NUnit.Framework;

namespace HolidayPooling.Infrastructure.Test.Configuration
{
    [TestFixture]
    public class HashCodeHelperTest
    {

        #region Tests

        [Test]
        public void GetUnitaryObject_WhenObjectIsNotNull_ShouldReturnRightHashCode()
        {
            unchecked
            {
                var hash = 1;
                var unitaryObject = 12;
                var expectedHashCode = (hash * HashCodeHelper.HashMultiplier) ^ (unitaryObject.GetHashCode());
                Assert.AreEqual(expectedHashCode, HashCodeHelper.GetUnitaryHashcode(hash, unitaryObject));
            }
        }

        [Test]
        public void GetUnitaryObject_WhenObjectIsNull_ShouldReturnRightHashCode()
        {
            unchecked
            {
                var hash = 1;
                var expectedHashCode = (hash * HashCodeHelper.HashMultiplier);
                Assert.AreEqual(expectedHashCode, HashCodeHelper.GetUnitaryHashcode(hash, null));
            }
        }

        #endregion

    }
}
