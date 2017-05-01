using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HolidayPooling.DataRepositories.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {

        #region Constants

        private const string UserNullErrorMessage = "Please provide a valid user";
        private const string UserNullLogMessage = "User not valid, operation will not be executed";

        #endregion

        #region Import/Export

        private readonly IUserDbImportExport _userPersister;

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public UserRepository()
            : base()
        {
            _userPersister = new UserDbImportExport();
        }

        public UserRepository(IUserDbImportExport userPersister) : base()
        {
            _userPersister = userPersister;
        }

        #endregion

        #region IUserRepository

        public void SaveUser(User user)
        {
            Errors.Clear();

            if (!CheckUserIsValid(user))
            {
                return;
            }

            try
            {
                _logger.Info("Start saving user using repository");
                if (_userPersister.IsMailUsed(user.Mail))
                {
                    Errors.Add("Mail is already used, please choose another one");
                    _logger.Warn("Mail is already used, please choose another one");
                    return;
                }

                if (_userPersister.IsPseudoUsed(user.Pseudo))
                {
                    Errors.Add("Pseudo is already used, please choose another one");
                    _logger.Warn("Pseudo is already used, please choose another one");
                    return;
                }

                var saved = _userPersister.Save(user);
                _logger.Info("Save ended");
                if (!saved)
                {
                    const string msg = "Internal error occured when saving user : account has not been created";
                    SetErrorAndLog(msg, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void UpdateUser(User user)
        {
            Errors.Clear();
            const string msg = "Internal Error occured : user cannot be updated";
            if (!CheckUserIsValid(user))
            {
                return;
            }

            try
            {
                _logger.Info("Start updating user using repository");
                var updated = _userPersister.Update(user);
                _logger.Info("End updating user using repository");
                
                if (!updated)
                {   
                    SetErrorAndLog(msg, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

        }

        public void DeleteUser(User user)
        {
            Errors.Clear();
            const string msg = "Internal occured when deleting user : account has not been deleted";
            
            if (!CheckUserIsValid(user))
            {
                return;
            }

            try
            {
                _logger.Info("Start deleting user using repository");
                var deleted = _userPersister.Delete(user);
                _logger.Info("End deleting user using repository");
                
                if (!deleted)
                {
                    SetErrorAndLog(msg, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public IEnumerable<User> GetUsers()
        {

            Errors.Clear();
            IEnumerable<User> list = new List<User>();
            try
            {
                _logger.Info("Start retrieving all users");
                list = _userPersister.GetAllEntities();
                _logger.Info(string.Format("End retrieving all user. Number retrieved {0}", list.Count()));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public User GetUser(int userId)
        {

            Errors.Clear();

            User user = null;
            
            try
            {
                _logger.Info(string.Format("Start retrieving User {0}", userId));
                user = _userPersister.GetEntity(userId);
                _logger.Info("End retrieving user with id");
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return user;
        }

        public User GetUserByMail(string mail, string password)
        {

            Errors.Clear();
            User user = null;
            try
            {
                _logger.Info(string.Format("Start retrieving User {0}", mail));
                user = _userPersister.GetUserByMailAndPassword(mail, password);
                _logger.Info("End retrieving user with mail");
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return user;
        }

        public User GetUserByPseudo(string pseudo, string password)
        {

            Errors.Clear();
            User user = null;

            try
            {
                _logger.Info(string.Format("Start retrieving User {0}", pseudo));
                user = _userPersister.GetUserByPseudoAndPassword(pseudo, password);
                _logger.Info("End retrieving user with pseudo");
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return user;
        }

        public User GetUserInfo(string pseudo)
        {
            User user = null;

            try
            {
                _logger.Info(string.Format("Start retrieving User info {0}", pseudo));
                user = _userPersister.GetUserInfo(pseudo);
                _logger.Info("End retrieving user info with pseudo");
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return user;
        }

        #endregion

        #region Methods

        private bool CheckUserIsValid(User user)
        {
            return CheckModel(user, UserNullErrorMessage, UserNullLogMessage, _logger);
        }

        #endregion

    }
}
