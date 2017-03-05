using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Crypters;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class UserDbImportExport : DbImportExportBase<int, User>, IUserDbImportExport
    {

        #region SQL

        private const string Insert = "INSERT INTO TUSR (IDT, MEL, PWD, USRSLT, AGE, PSD, PHNNBR, TYP, DSC, RLE, CREDAT, USRNOT)"
                                    + " VALUES (:pIDT, :pMEL, :pPWD, :pUSRSLT, :pAGE, :pPSD, :pPHNNBR, :pTYP, :pDSC, :pRLE, :pCREDAT, :pUSRNOT)";

        private const string UpdateQuery = "UPDATE TUSR SET"
                                    + " MEL = :pMEL, AGE = :pAGE, PSD = :pPSD, PHNNBR = :pPHNNBR, TYP = :pTYP,"
                                    + " DSC = :pDSC, RLE = :pRLE, CREDAT = :pCREDAT, USRNOT = :pUSRNOT"
                                    + " WHERE IDT = :pIDT";

        private const string DeleteQuery = "DELETE FROM TUSR WHERE IDT = :pIDT";

        private const string Select = "SELECT IDT, MEL, PWD, USRSLT, AGE, PSD, PHNNBR, TYP, DSC, RLE, CREDAT, USRNOT FROM TUSR";

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

        #endregion

        #region DbImportExportBase<int, User>

        protected override int CreateKeyFromReader(IDatabaseReader reader)
        {
            return reader.GetInt("IDT");
        }

        protected override User CreateValueFromReader(IDatabaseReader reader)
        {
            return new User
                (
                    reader.GetInt("IDT"),
                    reader.GetString("MEL"),
                    reader.GetString("PWD"),
                    reader.GetString("PSD"),
                    reader.GetInt("AGE"),
                    reader.GetString("DSC"),
                    ModelEnumConverter.RoleEnumFromString(reader.GetString("RLE")),
                    reader.GetDate("CREDAT"),
                    reader.GetString("PHNNBR"),
                    ModelEnumConverter.UserTypeFromString(reader.GetString("TYP")),
                    reader.GetDouble("USRNOT")
                );
        }

        protected override int CreateKeyFromValue(User value)
        {
            return value.Id;
        }

        public override int GetNewId()
        {
            int result = -1;
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectNewId;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt(0);
                            }
                        }
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        protected override string GetSelectQuery()
        {
            return Select;
        }

        #endregion

        #region IUserDbImportExport

        public User GetUserByMailAndPassword(string mail, string password)
        {
            User user = null;
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
                            user = reader.Read() ? CreateValueFromReader(reader) : null;

                            if (user != null)
                            {
                                var salt = reader.GetString("USRSLT");
                                if (!PasswordHasher.CheckPassword(password + salt, user.Password))
                                {
                                    user = null;
                                }
                                else
                                {
                                    user.Password = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            //TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return user;
        }

        public User GetUserByPseudoAndPassword(string pseudo, string password)
        {
            User user = null;
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
                            user = reader.Read() ? CreateValueFromReader(reader) : null;
                            if (user != null)
                            {
                                var salt = reader.GetString("USRSLT");
                                if (!PasswordHasher.CheckPassword(password + salt, user.Password))
                                {
                                    user = null;
                                }
                                else
                                {
                                    user.Password = string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            //TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return user;
        }

        public bool IsPseudoUsed(string pseudo)
        {
            bool found = true;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public bool IsMailUsed(string mail)
        {
            bool found = true;
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = Select;
                        cmd.AddStringParameter(":pMEL", mail);
                        using (var reader = cmd.ExecuteReader())
                        {
                            found = reader.Read();
                        }
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public bool Save(User entity)
        {

            bool saved = false;
            Check.IsNotNull(entity, "user shouldn't be null");
            try
            {

                // get next id
                int id = GetNewId();
                if (id < 0)
                {
                    return false;
                }

                // id
                entity.Id = id;
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
                        cmd.AddStringParameter(":pRLE", ModelEnumConverter.RoleEnumToString(entity.Role));
                        cmd.AddDateParameter(":pCREDAT", entity.CreationDate);
                        cmd.AddDoubleParameter(":pUSRNOT", 0);
                        saved = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(User entity)
        {
            Check.IsNotNull(entity, "User shouldn't be null");
            bool deleted = false;
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
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(User entity)
        {
            Check.IsNotNull(entity, "User shouldn't be null");
            bool updated = false;

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
                        cmd.AddStringParameter(":pRLE", ModelEnumConverter.RoleEnumToString(entity.Role));
                        cmd.AddDateParameter(":pCREDAT", entity.CreationDate);
                        cmd.AddDoubleParameter(":pUSRNOT", entity.Note);
                        updated = cmd.ExecuteNonQuery() > 0;
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public User GetEntity(int key)
        {
            User user = null;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return user;
        }

        public IEnumerable<User> GetAllEntities()
        {
            var list = new List<User>();
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = GetSelectQuery();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = CreateValueFromReader(reader);
                                user.Password = string.Empty;
                                list.Add(user);
                            }
                        }
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        #endregion
    }
}