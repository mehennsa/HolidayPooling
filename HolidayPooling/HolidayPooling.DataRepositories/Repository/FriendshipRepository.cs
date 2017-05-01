using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public class FriendshipRepository : RepositoryBase<Friendship>, IFriendshipRepository
    {

        #region Constants

        private const string NullFriendshipErrorMessage = "Please provide valid frienship information";
        private const string NullFriendshipLogErrorMessage = "Friendship is null, operation will not be executed";
        private const string SaveFailed = "Internal Error : Unable to save friendship";
        private const string UpdateFailed = "Internal Error : Unable to update friendship";
        private const string DeleteFailed = "Internal Error : Unable to delete friendship";

        #endregion

        #region Properties

        private readonly IFriendshipDbImportExport _persister;

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public FriendshipRepository()
            : base()
        {
            _persister = new FriendshipDbImportExport();
        }

        public FriendshipRepository(IFriendshipDbImportExport persister)
        {
            _persister = persister;
        }

        #endregion

        #region IFriendshipRepository

        public void SaveFriendship(Friendship friendship)
        {
            Errors.Clear();

            if (!CheckModel(friendship, NullFriendshipErrorMessage, NullFriendshipLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                _logger.Info("Start saving friendship");
                var saved = _persister.Save(friendship);
                _logger.Info("End saving friendship");

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

        public void UpdateFrendship(Friendship friendship)
        {
            Errors.Clear();

            if (!CheckModel(friendship, NullFriendshipErrorMessage, NullFriendshipLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                _logger.Info("Start updating friendship");
                 var updated = _persister.Update(friendship);
                _logger.Info("End updating friendship");

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

        public void DeleteFriendship(Friendship friendship)
        {
            Errors.Clear();

            if (!CheckModel(friendship, NullFriendshipErrorMessage, NullFriendshipLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start deleting friendship");
                var deleted = _persister.Delete(friendship);
                _logger.Info("End deleting friendship");

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

        public Friendship GetFriendship(int userId, string friendName)
        {
            Errors.Clear();
            Friendship friendship = null;

            try
            {
                _logger.Info(string.Format("Start retrieving friendship between user {0} and friend {1}", userId, friendName));
                friendship = _persister.GetEntity(new FriendshipKey(userId, friendName));
                _logger.Info(string.Format("End retrieving friendship between user {0} and friend {1}", userId, friendName));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return friendship;

        }

        public IEnumerable<Friendship> GetUserFriendships(int userId)
        {
            return InternalGetSpecificUserFriendships
                (
                    userId, 
                    (i) => _persister.GetUserFriendships(i), 
                    string.Format("retrieving friendships for user {0}", userId)
                );

        }

        public IEnumerable<Friendship> GetRequestedFriendships(int userId)
        {
            return InternalGetSpecificUserFriendships
                (
                    userId,
                    (i) => _persister.GetRequestedFriendships(i),
                    string.Format("retrieving requested friendships for user {0}", userId)
                );
        }

        public IEnumerable<Friendship> GetWaitingFriendships(int userId)
        {
            return InternalGetSpecificUserFriendships
                (
                    userId,
                    (i) => _persister.GetWaitingFriendships(i),
                    string.Format("retrieving waiting friendships for user {0}", userId)
                );
        }

        public IEnumerable<Friendship> GetAllFriendship()
        {
            Errors.Clear();
            IEnumerable<Friendship> list = new List<Friendship>();
            try
            {
                _logger.Info("Start retrieiving all friendships");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieiving all friendships");
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        #endregion

        #region Methods

        private IEnumerable<Friendship> InternalGetSpecificUserFriendships(int userId, Func<int, IEnumerable<Friendship>> function, string logMessage)
        {
            Errors.Clear();
            IEnumerable<Friendship> list = new List<Friendship>();
            try
            {
                _logger.Info("Start " + logMessage);
                list = function(userId);
                _logger.Info("End " + logMessage);
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
