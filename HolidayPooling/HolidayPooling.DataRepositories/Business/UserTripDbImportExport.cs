using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Infrastructure.TimeProviders;
using HolidayPooling.Models.Core;
using log4net;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Database;
using Sams.Commons.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Data;

namespace HolidayPooling.DataRepositories.Business
{
    public class UserTripDbImportExport : DbImportExportBase<UserTripKey, UserTrip>, IUserTripDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL

        private const string InsertQuery = "INSERT INTO TUSRTRP (USRIDT, TRPNAM, INDUSRPAR, INDUSRORG, USRNOT, TRPMNT, DATEFT)" +
                                            " VALUES (:pUSRIDT, :pTRPNAM, :pINDUSRPAR, :pINDUSRORG, :pUSRNOT, :pTRPMNT, :pDATEFT)";

        private const string DeleteQuery = "DELETE FROM TUSRTRP WHERE USRIDT = :pUSRIDT and TRPNAM = :pTRPNAM";

        private const string UpdateQuery = "UPDATE TUSRTRP SET INDUSRPAR = :pINDUSRPAR, INDUSRORG = :pINDUSRORG," +
                                    " USRNOT = :pUSRNOT, TRPMNT = :pTRPMNT, DATEFT = :pDATEFT" +
                                    " WHERE USRIDT = :pUSRIDT AND TRPNAM = :pTRPNAM";

        private const string SelectQuery = "SELECT USRIDT, TRPNAM, INDUSRPAR, INDUSRORG, USRNOT, TRPMNT, DATEFT FROM TUSRTRP";

        private const string SelectByUserId = SelectQuery + " WHERE USRIDT = :pUSRIDT";

        private const string SelectByKey = SelectByUserId + " AND TRPNAM = :pTRPNAM";

        private const string SelectByTripName = SelectQuery + " WHERE TRPNAM = :pTRPNAM";

        #endregion

        #region .ctor

        public UserTripDbImportExport()
            : base()
        {

        }

        internal UserTripDbImportExport(ITimeProvider timeProvider) : base(timeProvider)
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
            var userTrip = new UserTrip
                (
                    reader.GetInt("USRIDT"),
                    reader.GetString("TRPNAM"),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDUSRPAR")),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDUSRORG")),
                    reader.GetDouble("USRNOT"),
                    reader.GetDouble("TRPMNT")
                );
            userTrip.ModificationDate = reader.GetDate("DATEFT");
            return userTrip;
        }

        protected override UserTripKey CreateKeyFromValue(UserTrip value)
        {
            return new UserTripKey(value.UserId, value.TripName);
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region IUserTripDbImportExpot

        public IEnumerable<UserTrip> GetTripForUser(int userId)
        {
            return GetListValuesWithIdParameter(SelectByUserId, ":pUSRIDT", userId);
        }

        public bool Save(UserTrip entity)
        {
            Check.IsNotNull(entity, "User trip shouldn't be empty");
            bool saved = false;
            entity.ModificationDate = TimeProvider.Now();
            _logger.Info("Start saving users trip");
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
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End saving users trip " + (saved ? "Success" : "Failure"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while saving user's trip with query : " + InsertQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(UserTrip entity)
        {
            Check.IsNotNull(entity, "UserTrip shouldn't be null");
            var deleted = false;
            _logger.Info("Start delete user's trip");

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
                        _logger.Info("End delete user's trip : " + (deleted ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while deleting user's trip with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(UserTrip entity)
        {
            Check.IsNotNull(entity, "UserTrip should not be null");
            bool updated = false;
            entity.ModificationDate = TimeProvider.Now();
            _logger.Info("Start update user's trip");

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
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End update user's trip : " + (updated ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while updating user's trip with query" + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public UserTrip GetEntity(UserTripKey key)
        {
            UserTrip userTrip = null;
            Check.IsNotNull(key, "Key should be provided");
            _logger.Info("Start retrieving user's trip");

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
                _logger.Info("End retrieving user's trip : " + (userTrip != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving user's trip with query : " + SelectByKey, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return userTrip;
        }

        public IEnumerable<UserTrip> GetUserTripsByTrip(string tripName)
        {
            _logger.Info("Start retrieving user trips by trip name");
            var list = new List<UserTrip>();

            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByTripName;
                        cmd.AddStringParameter(":pTRPNAM", tripName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }

                return list;
            }
            catch(Exception ex)
            {
                _logger.Error("Error while retrieving user's trip with query : " + SelectByTripName, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }
        }

        public IEnumerable<UserTrip> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion
    }
}
