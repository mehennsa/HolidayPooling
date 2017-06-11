namespace HolidayPooling.Infrastructure.Configuration
{
    public static class HashCodeHelper
    {

        #region Fields

        public const uint HashConstant = 2166136261;
        public const int HashMultiplier = 16777619;

        #endregion

        #region Methods

        public static int GetUnitaryHashcode(int hash, object unitaryObject)
        {
            unchecked
            {
                return (hash * HashMultiplier) ^ (unitaryObject != null ? unitaryObject.GetHashCode() : 0);
            }
        }
        #endregion

    }
}