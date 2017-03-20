using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Database;
using Sams.Commons.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class PotUserDbImportExport : DbImportExportBase<PotUserKey, PotUser>, IPotUserDbImportExport
    {


        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL

        private const string InsertQuery = "INSERT INTO TPOTUSR" +
                                            " (POTIDT, USRIDT, INDPAY, CURMNT, TGTMNT, INDCANCEL, CANCELRSN, INDVAL)" +
                                            " VALUES (:pPOTIDT, :pUSRIDT, :pINDPAY, :pCURMNT, :pTGTMNT, :pINDCANCEL, :pCANCELRSN, :pINDVAL)";
        
        private const string DeleteQuery = "DELETE FROM TPOTUSR WHERE POTIDT = :pPOTIDT and USRIDT = :pUSRIDT";

        private const string UpdateQuery = "UPDATE TPOTUSR SET" +
                                            " INDPAY = :pINDPAY, CURMNT =:pCURMNT, TGTMNT = :pTGTMNT, INDCANCEL = :pINDCANCEL, CANCELRSN = :pCANCELRSN, INDVAL = :pINDVAL" +
                                            " WHERE POTIDT = :pPOTIDT AND USRIDT = :pUSRIDT";

        private const string SelectQuery = "SELECT POTIDT, USRIDT, INDPAY, CURMNT, TGTMNT, INDCANCEL, CANCELRSN, INDVAL FROM TPOTUSR";

        private const string SelectByPot = SelectQuery + " WHERE POTIDT = :pPOTIDT";

        private const string SelectByUser = SelectQuery + " WHERE USRIDT = :pUSRIDT";

        private const string SelectByKey = SelectQuery + " WHERE POTIDT = :pPOTIDT AND USRIDT = :pUSRIDT";

        #endregion

        #region DbImportExportBase<PotUserKey, PotUser>

        protected override PotUserKey CreateKeyFromReader(IDatabaseReader reader)
        {
            return new PotUserKey(reader.GetInt("POTIDT"), reader.GetInt("USRIDT"));
        }

        protected override PotUser CreateValueFromReader(IDatabaseReader reader)
        {
            return new PotUser
                        (
                            reader.GetInt("USRIDT"),
                            reader.GetInt("POTIDT"),
                            ConverterHelper.YesNoStringToBool(reader.GetString("INDPAY")),
                            reader.GetDouble("CURMNT"),
                            reader.GetDouble("TGTMNT"),
                            ConverterHelper.YesNoStringToBool(reader.GetString("INDCANCEL")),
                            reader.GetString("CANCELRSN"),
                            ConverterHelper.YesNoStringToBool(reader.GetString("INDVAL"))
                        );
        }

        protected override PotUserKey CreateKeyFromValue(PotUser value)
        {
            return new PotUserKey(value.PotId, value.UserId);
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region IPotUserDbImportExport

        public IEnumerable<PotUser> GetPotUsers(int potId)
        {
            return GetListValuesWithIdParameter(SelectByPot, ":pPOTIDT", potId);
        }

        public IEnumerable<PotUser> GetUserPots(int userId)
        {
            return GetListValuesWithIdParameter(SelectByUser, ":pUSRIDT", userId);
        }

        public bool Save(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
            _logger.Info("Start saving user pot");
            var saved = false;
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pPOTIDT", entity.PotId);
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pINDPAY", ConverterHelper.BoolToYesNoString(entity.HasPayed));
                        cmd.AddDoubleParameter(":pCURMNT", entity.Amount);
                        cmd.AddDoubleParameter(":pTGTMNT", entity.TargetAmount);
                        cmd.AddStringParameter(":pCANCELRSN", entity.CancellationReason);
                        cmd.AddStringParameter(":pINDCANCEL", ConverterHelper.BoolToYesNoString(entity.HasCancelled));
                        cmd.AddStringParameter(":pINDVAL", ConverterHelper.BoolToYesNoString(entity.HasValidated));
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End saving user pot " + (saved ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to save user pot with query : " + InsertQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
            var deleted = false;
            _logger.Info("Start deleting user pot");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = DeleteQuery;
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddIntParameter(":pPOTIDT", entity.PotId);
                        deleted = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End deleting user pot : " + (deleted ? "Success" : "Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to delete user pot with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
            var updated = false;
            _logger.Info("Start updating user pot");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pPOTIDT", entity.PotId);
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pINDPAY", ConverterHelper.BoolToYesNoString(entity.HasPayed));
                        cmd.AddDoubleParameter(":pCURMNT", entity.Amount);
                        cmd.AddDoubleParameter(":pTGTMNT", entity.TargetAmount);
                        cmd.AddStringParameter(":pCANCELRSN", entity.CancellationReason);
                        cmd.AddStringParameter(":pINDCANCEL", ConverterHelper.BoolToYesNoString(entity.HasCancelled));
                        cmd.AddStringParameter(":pINDVAL", ConverterHelper.BoolToYesNoString(entity.HasValidated));
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End updating user pot : " + (updated ? "Success" : "Failure"));
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to update user pot with query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public PotUser GetEntity(PotUserKey key)
        {
            Check.IsNotNull(key, "Key should be provided to find pot participant");
            PotUser potUser;
            _logger.Info("Start retrieving user pot");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByKey;
                        cmd.AddIntParameter(":pPOTIDT", key.PotId);
                        cmd.AddIntParameter(":pUSRIDT", key.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            potUser = reader.Read() ? CreateValueFromReader(reader) : null;
                        }
                    }
                }
                _logger.Info("End retrieving user pot : " + (potUser != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to find user pot with query : " + SelectByKey, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return potUser;
        }

        public IEnumerable<PotUser> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion

    }
}
