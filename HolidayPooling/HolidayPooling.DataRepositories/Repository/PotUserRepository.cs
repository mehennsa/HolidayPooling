using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public class PotUserRepository : RepositoryBase<PotUser>, IPotUserRepository
    {


        #region Constants

        private const string NullPotUserErrorMessage = "Please provide a valid pot's user";

        private const string NullPotUserLogErrorMessage = "Pot's user is null, operation will not be executed";

        private const string SaveFail = "Internal Error : Unable to save pot's user";

        private const string UpdateFail = "Internal Error : Unable to update pot's user";

        private const string DeleteFail = "Internal Error : Unable to delete pot's user";

        #endregion

        #region Properties

        private readonly IPotUserDbImportExport _persister;
        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public PotUserRepository()
            : base()
        {
            _persister = new PotUserDbImportExport();
        }

        public PotUserRepository(IPotUserDbImportExport persister)
            : base()
        {
            _persister = persister;
        }

        #endregion

        #region IPotUserRepository

        public void SavePotUser(PotUser potUser)
        {
            Errors.Clear();

            if (!CheckModel(potUser, NullPotUserErrorMessage, NullPotUserLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start saving pot's user");
                var saved = _persister.Save(potUser);
                _logger.Info("End saving pot's user");
                
                if (!saved)
                {
                    SetErrorAndLog(SaveFail, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void UpdatePotUser(PotUser potUser)
        {

            Errors.Clear();

            if (!CheckModel(potUser, NullPotUserErrorMessage, NullPotUserLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start updating pot's user");
                var updated = _persister.Update(potUser);
                _logger.Info("End updating pot's user");

                if (!updated)
                {
                    SetErrorAndLog(UpdateFail, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void DeletePotUser(PotUser potUser)
        {

            Errors.Clear();

            if (!CheckModel(potUser, NullPotUserErrorMessage, NullPotUserLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start deleting pot's user");
                var deleted = _persister.Delete(potUser);
                _logger.Info("End deleting pot's user");

                if (!deleted)
                {
                    SetErrorAndLog(DeleteFail, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public PotUser GetPotUser(int potId, int userId)
        {
            Errors.Clear();

            PotUser potUser = null;

            try
            {

                _logger.Info(string.Format("Start retrieving pot {0} with user {1}", potId, userId));
                potUser = _persister.GetEntity(new PotUserKey(potId, userId));
                _logger.Info(string.Format("End retrieving pot {0} with user {1}", potId, userId));

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return potUser;
        }

        public IEnumerable<PotUser> GetPotUsers(int potId)
        {
            Errors.Clear();
            IEnumerable<PotUser> list = new List<PotUser>();

            try
            {
                _logger.Info(string.Format("Start retrieving users on pot {0}", potId));
                list = _persister.GetPotUsers(potId);
                _logger.Info(string.Format("End retrieving users on pot {0}", potId));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public IEnumerable<PotUser> GetUserPots(int userId)
        {
            Errors.Clear();
            IEnumerable<PotUser> list = new List<PotUser>();

            try
            {
                _logger.Info(string.Format("Start retrieving pots of user {0}", userId));
                list = _persister.GetUserPots(userId);
                _logger.Info(string.Format("End retrieving pots of user {0}", userId));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public IEnumerable<PotUser> GetAllPotUser()
        {
            Errors.Clear();
            IEnumerable<PotUser> list = new List<PotUser>();

            try
            {

                _logger.Info("Start retrieving all pot users");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieving all pot users");

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
