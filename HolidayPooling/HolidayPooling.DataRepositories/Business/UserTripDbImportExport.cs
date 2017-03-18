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
    public class UserTripDbImportExport : DbImportExportBase<UserTripKey, UserTrip>, IUserTripDbImportExport
    {

        #region SQL

        private const string InsertQuery = "INSERT INTO TUSRTRP (USRIDT, TRPNAM, INDUSRPAR, INDUSRORG, USRNOT, TRPMNT)" +
                                            " VALUES (:pUSRIDT, :pTRPNAM, :pINDUSRPAR, :pINDUSRORG, :pUSRNOT, :pTRPMNT)";

        private const string DeleteQuery = "DELETE FROM TUSRTRP WHERE USRIDT = :pUSRIDT and TRPNAM = :pTRPNAM";

        private const string UpdateQuery = "UPDATE TUSRTRP SET INDUSRPAR = :pINDUSRPAR, INDUSRORG = :pINDUSRORG," +
                                    " USRNOT = :pUSRNOT, TRPMNT = :pTRPMNT" +
                                    " WHERE USRIDT = :pUSRIDT AND TRPNAM = :pTRPNAM";

        private const string SelectQuery = "SELECT USRIDT, TRPNAM, INDUSRPAR, INDUSRORG, USRNOT, TRPMNT FROM TUSRTRP";

        private const string SelectByUserId = SelectQuery + " WHERE USRIDT = :pUSRIDT";

        private const string SelectByKey = SelectByUserId + " AND TRPNAM = :pTRPNAM";

        #endregion

        #region .ctor

        public UserTripDbImportExport()
            : base()
        {

        }

        #endregion

        #region DbImportExportBase<UserTripKey, UserTrip>

        protected override UserTripKey CreateKeyFromReader(IDatabaseReader reader)
        {
            return new UserTripKey(reader.GetInt("USRIDT"), reader.GetString("TRPNAM"));
        }

        protected override UserTrip CreateValueFromReader(IDatabaseReader reader)
        {
            return new UserTrip
                (
                    reader.GetInt("USRIDT"),
                    reader.GetString("TRPNAM"),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDUSRPAR")),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDUSRORG")),
                    reader.GetDouble("USRNOT"),
                    reader.GetDouble("TRPMNT")
                );
        }

        protected override UserTripKey CreateKeyFromValue(UserTrip value)
        {
            return new UserTripKey(value.UserId, value.TripName);
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

        #region IUserTripDbImportExpot

        public IEnumerable<UserTrip> GetTripForUser(int userId)
        {
            var list = new List<UserTrip>();

            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByUserId;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        public bool Save(UserTrip entity)
        {
            Check.IsNotNull(entity, "User trip shouldn't be empty");
            bool saved = false;
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pTRPNAM", entity.TripName);
                        cmd.AddStringParameter(":pINDUSRPAR", ConverterHelper.BoolToYesNoString(entity.HasParticipated));
                        cmd.AddStringParameter(":pINDUSRORG", ConverterHelper.BoolToYesNoString(entity.HasOrganized));
                        cmd.AddDoubleParameter(":pUSRNOT", entity.UserNote);
                        cmd.AddDoubleParameter(":pTRPMNT", entity.TripAmount);
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

        public bool Delete(UserTrip entity)
        {
            Check.IsNotNull(entity, "UserTrip shouldn't be null");
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
                        cmd.AddStringParameter(":pTRPNAM", entity.TripName);
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

        public bool Update(UserTrip entity)
        {
            Check.IsNotNull(entity, "UserTrip should not be null");
            bool updated = false;
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddStringParameter(":pINDUSRPAR", ConverterHelper.BoolToYesNoString(entity.HasParticipated));
                        cmd.AddStringParameter(":pINDUSRORG", ConverterHelper.BoolToYesNoString(entity.HasOrganized));
                        cmd.AddDoubleParameter(":pUSRNOT", entity.UserNote);
                        cmd.AddDoubleParameter(":pTRPMNT", entity.TripAmount);
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pTRPNAM", entity.TripName);
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

        public UserTrip GetEntity(UserTripKey key)
        {
            UserTrip userTrip = null;
            Check.IsNotNull(key, "Key should be provided");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByKey;
                        cmd.AddIntParameter(":pUSRIDT", key.UserId);
                        cmd.AddStringParameter(":pTRPNAM", key.TripName);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userTrip = CreateValueFromReader(reader);
                            }
                        }
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return userTrip;
        }

        public IEnumerable<UserTrip> GetAllEntities()
        {
            var list = new List<UserTrip>();
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