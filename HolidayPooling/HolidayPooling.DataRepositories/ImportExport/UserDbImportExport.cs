using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Infrastructure.TimeProviders;
using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
using log4net;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Crypters;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;

namespace HolidayPooling.DataRepositories.ImportExport
{
    public class UserDbImportExport : DbImportExportBase<int, User>, IUserDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL

        private const string Insert = "INSERT INTO TUSR (IDT, MEL, PWD, USRSLT, AGE, PSD, PHNNBR, TYP, DSC, RLE, CREDAT, USRNOT, DATEFT)"
                                    + " VALUES (:pIDT, :pMEL, :pPWD, :pUSRSLT, :pAGE, :pPSD, :pPHNNBR, :pTYP, :pDSC, :pRLE, :pCREDAT, :pUSRNOT, :pDATEFT)";

        private const string UpdateQuery = "UPDATE TUSR SET"
                                    + " MEL = :pMEL, AGE = :pAGE, PSD = :pPSD, PHNNBR = :pPHNNBR, TYP = :pTYP,"
                                    + " DSC = :pDSC, RLE = :pRLE, CREDAT = :pCREDAT, USRNOT = :pUSRNOT, DATEFT = :pDATEFT"
                                    + " WHERE IDT = :pIDT";

        private const string DeleteQuery = "DELETE FROM TUSR WHERE IDT = :pIDT";

        private const string Select = "SELECT IDT, MEL, PWD, USRSLT, AGE, PSD, PHNNBR, TYP, DSC, RLE, CREDAT, USRNOT, DATEFT FROM TUSR";

        private const string SelectSingle = Select + " WHERE IDT = :pIDT";

        private const string SelectByMail = Select + " WHERE MEL = :pMEL";

        private const string SelectByPseudo = Select + " WHERE PSD = :pPSD";

        private const string SelectNewId = "Select nextval('SUSRIDT')";

        #endregion

        #region .ctor

        public UserDbImportExport()
            : base()
        {

        }

        internal UserDbImportExport(ITimeProvider timeProvider) : base(timeProvider)
        {
        }

        #endregion

        #region DbImportExportBase<int, User>

        protected override int CreateKeyFromReader(IDatabaseReader reader)
        {
            return reader.GetInt("IDT");
        }

        protected override User CreateValueFromReader(IDatabaseReader reader)
        {
            var user =  new User
                (
                    reader.GetInt("IDT"),
                    reader.GetString("MEL"),
                    reader.GetString("PWD"),
                    reader.GetString("PSD"),
                    reader.GetInt("AGE"),
                    reader.GetString("DSC"),
                    ModelEnumConverter.RoleFromString(reader.GetString("RLE")),
                    reader.GetDate("CREDAT"),
                    reader.GetString("PHNNBR"),
                    ModelEnumConverter.UserTypeFromString(reader.GetString("TYP")),
                    reader.GetDouble("USRNOT")
                );
            user.ModificationDate = reader.GetDate("DATEFT");
            return user;
        }

        protected override int CreateKeyFromValue(User value)
        {
            return value.Id;
        }

        protected override string NewIdQuery()
        {
            return SelectNewId;
        }

        protected override string GetSelectQuery()
        {
            return Select;
        }

        #endregion

        #region IUserDbImportExport

        public User GetUserByMailAndPassword(string mail, string password)
        {

            return GetUserByCustomCriteriaWithPassword
                                            (
                                                "Mail",
                                                SelectByMail,
                                                mail,
                                                password,
                                                (cmd, s) => cmd.AddStringParameter(":pMEL", s)
                                            );
        }

        public User GetUserInfo(string pseudo)
        {
            return GetUserByCustomCriteriaWithPassword
                                (
                                    "Pseudo",
                                    SelectByPseudo,
                                    pseudo,
                                    string.Empty,
                                    (cmd, s) => cmd.AddStringParameter(":pPSD", s),
                                    false
                                );
        }

        public User GetUserByPseudoAndPassword(string pseudo, string password)
        {
            return GetUserByCustomCriteriaWithPassword
                                (
                                    "Pseudo",
                                    SelectByPseudo,
                                    pseudo,
                                    password,
                                    (cmd, s) => cmd.AddStringParameter(":pPSD", s)
                                );
        }

        public bool IsPseudoUsed(string pseudo)
        {
            bool found = true;
            _logger.Info("Start checking if pseudo is used");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByPseudo;
                        cmd.AddStringParameter(":pPSD", pseudo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            found = reader.Read();
                        }
                    }
                }

                _logger.Info("End checking if pseudo is used");

            }
            catch (Exception ex)
            {
                _logger.Error("Error while checking if a pseudo is used with query : " + SelectByPseudo, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public bool IsMailUsed(string mail)
        {
            bool found = true;
            _logger.Info("Start checking if a mail is already used");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByMail;
                        cmd.AddStringParameter(":pMEL", mail);
                        using (var reader = cmd.ExecuteReader())
                        {
                            found = reader.Read();
                        }
                    }
                }

                _logger.Info("End checking if a mail is already used");

            }
            catch (Exception ex)
            {
                _logger.Error("Error while checking if a mail is already used with query : " + SelectByMail);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public bool Save(User entity)
        {

            bool saved = false;
            Check.IsNotNull(entity, "user shouldn't be null");
            _logger.Info("Start saving user");
            entity.ModificationDate = TimeProvider.Now();
            try
            {

                // get next id
                int id = GetNewId();
                if (id < 0)
                {
                    _logger.Info("Id cannot be generated to save new user : Failure");
                    return false;
                }

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        // Password handling
                        string salt;
                        var hashPassword = PasswordHasher.HashPassword(entity.Password, out salt);
                        // reset password
                        entity.Password = string.Empty;
                        // query
                        cmd.CommandText = Insert;
                        // id
                        cmd.AddIntParameter(":pIDT", id);
                        cmd.AddStringParameter(":pMEL", entity.Mail);
                        cmd.AddStringParameter(":pPWD", hashPassword);
                        cmd.AddStringParameter(":pUSRSLT", salt);
                        cmd.AddIntParameter(":pAGE", entity.Age);
                        cmd.AddStringParameter(":pPSD", entity.Pseudo);
                        cmd.AddStringParameter(":pPHNNBR", entity.PhoneNumber);
                        cmd.AddStringParameter(":pTYP", ModelEnumConverter.UserTypeToString(entity.Type));
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddStringParameter(":pRLE", ModelEnumConverter.RoleToString(entity.Role));
                        cmd.AddDateParameter(":pCREDAT", entity.CreationDate);
                        cmd.AddDoubleParameter(":pUSRNOT", entity.Note);
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End saving user : " + (saved ? "Success" : "Failure"));
                        if (saved)
                        {
                            entity.Id = id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while saving new user with query : " + Insert, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(User entity)
        {
            Check.IsNotNull(entity, "User shouldn't be null");
            bool deleted = false;
            _logger.Info("Start deleting user");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = DeleteQuery;
                        cmd.AddIntParameter(":pIDT", entity.Id);
                        deleted = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End deleting user : " + (deleted ? "Success" : "Delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while deleting user with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(User entity)
        {
            Check.IsNotNull(entity, "User shouldn't be null");
            bool updated = false;
            _logger.Info("Start update user");
            entity.ModificationDate = TimeProvider.Now();
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pIDT", entity.Id);
                        cmd.AddStringParameter(":pMEL", entity.Mail);
                        cmd.AddIntParameter(":pAGE", entity.Age);
                        cmd.AddStringParameter(":pPSD", entity.Pseudo);
                        cmd.AddStringParameter(":pPHNNBR", entity.PhoneNumber);
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddStringParameter(":pTYP", ModelEnumConverter.UserTypeToString(entity.Type));
                        cmd.AddStringParameter(":pRLE", ModelEnumConverter.RoleToString(entity.Role));
                        cmd.AddDateParameter(":pCREDAT", entity.CreationDate);
                        cmd.AddDoubleParameter(":pUSRNOT", entity.Note);
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End update user : " + (updated ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while updating user with query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public User GetEntity(int key)
        {
            User user = null;
            _logger.Info("Start retrieving user y id");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectSingle;
                        cmd.AddIntParameter(":pIDT", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = CreateValueFromReader(reader);
                                user.Password = string.Empty;
                            }
                        }
                    }
                }

                _logger.Info("End retrieving user by id : " + (user != null ? "Success" : "Failure"));

            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving user by id with query " + SelectSingle, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return user;
        }

        public IEnumerable<User> GetAllEntities()
        {
            return InternalGetAllEntities((u) => u.Password = string.Empty);
        }

        #endregion

        #region Utils

        private User GetUserByCustomCriteriaWithPassword(string criteriaName, string customQuery, string criteria, string password, 
            Action<IDatabaseCommand, string> commandSetup, bool usePassword = true)
        {
            User user = null;
            _logger.Info(string.Format("Start retrieving user by {0}", criteriaName));
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = customQuery;
                        commandSetup(cmd, criteria);
                        using (var reader = cmd.ExecuteReader())
                        {
                            user = reader.Read() ? CreateValueFromReader(reader) : null;

                            if (user != null)
                            {
                                var userPassword = user.Password;
                                user.Password = string.Empty;
                                if (usePassword)
                                {
                                    var salt = reader.GetString("USRSLT");
                                    if (!PasswordHasher.CheckPassword(password + salt, userPassword))
                                    {
                                        user = null;
                                    }
                                }
                            }
                        }
                    }
                }

                _logger.Info(string.Format("End retrieving user by {0} : {1}", criteriaName, (user != null ? "Success" : "Failure")));
            }
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return user;
        }

        #endregion

    }
}