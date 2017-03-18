using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
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

        public override int GetNewId()
        {
            throw new NotImplementedException();
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region IPotUserDbImportExport

        public IEnumerable<PotUser> GetPotUsers(int potId)
        {
            List<PotUser> list = new List<PotUser>();

            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByPot;
                        cmd.AddIntParameter(":pPOTIDT", potId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }

                return list;
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }
        }

        public IEnumerable<PotUser> GetUserPots(int userId)
        {
            List<PotUser> list = new List<PotUser>();

            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByUser;
                        cmd.AddIntParameter(":pUSRIDT", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }

                return list;
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }
        }

        public bool Save(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
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
                    }
                }

            }// TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
            var deleted = false;

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
                    }
                }

            }// TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(PotUser entity)
        {
            Check.IsNotNull(entity, "Pot participant should be provided");
            var updated = false;

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
                    }
                }


            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public PotUser GetEntity(PotUserKey key)
        {
            Check.IsNotNull(key, "Key should be provided to find pot participant");
            PotUser potUser;

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
            }
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return potUser;
        }

        public IEnumerable<PotUser> GetAllEntities()
        {
            var list = new List<PotUser>();

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
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }

            }// TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        #endregion

    }
}
