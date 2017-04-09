using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public sealed class UserTripRepository : RepositoryBase<UserTrip>, IUserTripRepository
    {

        #region Constants

        private const string NullUserTripErrorMessage = "Please provide valid user trip information";

        private const string NullUserTripLogMessage = "User's trip not valid, operation will not be executed";

        private const string SaveFailed = "Internal Error : Unable to save user's trip";

        private const string DeleteFailed = "Internal Error : Unable to delete user's trip";

        private const string UpdateFailed = "Internal Error : Unable to update user's trip";

        #endregion

        #region Properties

        private IUserTripDbImportExport _persister;
        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public UserTripRepository()
            :base()
        {
            _persister = new UserTripDbImportExport();
        }

        public UserTripRepository(IUserTripDbImportExport persister)
            : base()
        {
            _persister = persister;
        }

        #endregion

        #region IUserTripRepository

        public void SaveUserTrip(UserTrip userTrip)
        {
            Errors.Clear();
            if (!CheckModel(userTrip, NullUserTripErrorMessage, NullUserTripLogMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start saving user's trip information");
                var saved = _persister.Save(userTrip);
                _logger.Info("End saving user's trip information");
                if (!saved)
                {
                    SetErrorAndLog(SaveFailed, _logger);
                }
                
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void UpdateUserTrip(UserTrip userTrip)
        {

            Errors.Clear();

            if (!CheckModel(userTrip, NullUserTripErrorMessage, NullUserTripLogMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start updating user's trip information");
                var updated = _persister.Update(userTrip);
                _logger.Info("End updating user's trip information");
                if (!updated)
                {
                    SetErrorAndLog(UpdateFailed, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void DeleteUserTrip(UserTrip userTrip)
        {
            Errors.Clear();

            if (!CheckModel(userTrip, NullUserTripErrorMessage, NullUserTripLogMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start deleting user's trip information");
                var deleted = _persister.Delete(userTrip);
                _logger.Info("End deleting user's trip information");
                if (!deleted)
                {
                    SetErrorAndLog(DeleteFailed, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public UserTrip GetUserTrip(int userId, string tripName)
        {

            Errors.Clear();

            UserTrip userTrip = null;

            try
            {
                _logger.Info(string.Format("Start retrieving trip's information for user {0} on trip {1}", userId, tripName));
                userTrip = _persister.GetEntity(new UserTripKey(userId, tripName));
                _logger.Info(string.Format("End retrieving trip's information for user {0} on trip {1}", userId, tripName));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return userTrip;
        }

        public IEnumerable<UserTrip> GetUserTrips(int userId)
        {
            Errors.Clear();

            IEnumerable<UserTrip> list = new List<UserTrip>();
            try
            {
                _logger.Info(string.Format("Start retrieving trip's information for user {0}", userId));
                list = _persister.GetTripForUser(userId);
                _logger.Info(string.Format("End retrieving trip's information for user {0}", userId));

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;

        }

        public IEnumerable<UserTrip> GetAllUserTrip()
        {
            Errors.Clear();
            IEnumerable<UserTrip> list = new List<UserTrip>();
            try
            {
                _logger.Info("Start retrieving trip's information for all users");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieving trip's information for all users");

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        #endregion

    }
}
