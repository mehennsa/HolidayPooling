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
    public class TripParticipantDbImportExport : DbImportExportBase<TripParticipantKey, TripParticipant>, ITripParticipantDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL


        private const string InsertQuery = "INSERT INTO TTRPPTP (TRPIDT, USRPSD, INDUSRPTP, TRPNOT, VALDAT)" +
                                    " VALUES (:pTRPIDT, :pUSRPSD, :pINDUSRPTP, :pTRPNOT, :pVALDAT)";

        private const string DeleteQuery = "DELETE FROM TTRPPTP WHERE TRPIDT = :pTRPIDT AND USRPSD = :pUSRPSD";

        private const string UpdateQuery = "UPDATE TTRPPTP SET" +
                                            " INDUSRPTP = :pINDUSRPTP, TRPNOT = :pTRPNOT, VALDAT = :pVALDAT" +
                                            " WHERE TRPIDT = :pTRPIDT AND USRPSD = :pUSRPSD";

        private const string SelectQuery = "SELECT TRPIDT, USRPSD, INDUSRPTP, TRPNOT, VALDAT FROM TTRPPTP";

        private const string SelectByTrip = SelectQuery + " WHERE TRPIDT = :pTRPIDT";

        private const string SelectByKey = SelectByTrip + " AND USRPSD = :pUSRPSD";


        #endregion

        #region .ctor

        public TripParticipantDbImportExport()
            : base()
        {

        }

        #endregion

        #region DbImportExportBase<TripParticipantKey, TripParticipant>

        protected override TripParticipantKey CreateKeyFromReader(IDatabaseReader reader)
        {
            return new TripParticipantKey(reader.GetInt("TRPIDT"), reader.GetString("USRPSD"));
        }

        protected override TripParticipant CreateValueFromReader(IDatabaseReader reader)
        {
            return new TripParticipant
                    (
                        reader.GetInt("TRPIDT"),
                        reader.GetString("USRPSD"),
                        ConverterHelper.YesNoStringToBool(reader.GetString("INDUSRPTP")),
                        reader.GetDouble("TRPNOT"),
                        reader.GetNullableDate("VALDAT")
                    );
        }

        protected override TripParticipantKey CreateKeyFromValue(TripParticipant value)
        {
            return new TripParticipantKey(value.TripId, value.UserPseudo);
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region ITripParticipantDbImportExport

        public IEnumerable<TripParticipant> GetParticipantsForTrip(int tripId)
        {
            return GetListValuesWithIdParameter(SelectByTrip, ":pTRPIDT", tripId);
        }

        public bool Save(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var saved = false;
            _logger.Info("Start save trip participant");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pTRPIDT", entity.TripId);
                        cmd.AddStringParameter(":pUSRPSD", entity.UserPseudo);
                        cmd.AddStringParameter(":pINDUSRPTP", ConverterHelper.BoolToYesNoString(entity.HasParticipated));
                        cmd.AddDoubleParameter(":pTRPNOT", entity.TripNote);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidationDate);
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End save trip participant : " + (saved ? "Success" : "Failure"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to save trip participant with query : " + InsertQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var deleted = false;
            _logger.Info("Start delete trip participant");

            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = DeleteQuery;
                        cmd.AddIntParameter(":pTRPIDT", entity.TripId);
                        cmd.AddStringParameter(":pUSRPSD", entity.UserPseudo);
                        deleted = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End delete trip participant : " + (deleted ? "Success" : "Failure"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while deleting trip participant with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var updated = false;
            _logger.Info("Start updating trip participant");

            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pTRPIDT", entity.TripId);
                        cmd.AddStringParameter(":pUSRPSD", entity.UserPseudo);
                        cmd.AddStringParameter(":pINDUSRPTP", ConverterHelper.BoolToYesNoString(entity.HasParticipated));
                        cmd.AddDoubleParameter(":pTRPNOT", entity.TripNote);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidationDate);
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End update trip participant : " + (updated ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while updating trip participant with query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public TripParticipant GetEntity(TripParticipantKey key)
        {
            Check.IsNotNull(key, "Key should have been provided");
            TripParticipant participant = null;
            _logger.Info("Start retrieving trip participant");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByKey;
                        cmd.AddIntParameter(":pTRPIDT", key.TripId);
                        cmd.AddStringParameter(":pUSRPSD", key.UserPseudo);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                participant = CreateValueFromReader(reader);
                            }
                        }
                    }
                }

                _logger.Info("End retrieving trip participant : " + (participant != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving trip participant with query : " + SelectByKey, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return participant;
        }

        public IEnumerable<TripParticipant> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion
    }
}