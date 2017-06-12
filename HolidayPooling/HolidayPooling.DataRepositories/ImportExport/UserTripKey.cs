namespace HolidayPooling.DataRepositories.ImportExport
{
    public class UserTripKey
    {

        #region Properties

        public int UserId { get; private set; }

        public string TripName { get; private set; }

        #endregion

        #region .ctor

        public UserTripKey(int userId, string tripName)
        {
            UserId = userId;
            TripName = tripName;
        }

        #endregion

    }
}