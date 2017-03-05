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
    public class TripParticipantDbImportExport : DbImportExportBase<TripParticipantKey, TripParticipant>, ITripParticipantDbImportExport
    {

        #region SQL


        private string InsertQuery = "INSERT INTO TTRPPTP (TRPIDT, USRPSD, INDUSRPTP, TRPNOT, VALDAT)" +
                                    " VALUES (:pTRPIDT, :pUSRPSD, :pINDUSRPTP, :pTRPNOT, :pVALDAT)";

        private string DeleteQuery = "DELETE FROM TTRPPTP WHERE TRPIDT = :pTRPIDT AND USRPSD = :pUSRPSD";

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

        public override int GetNewId()
        {
            throw new NotImplementedException();
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region ITripParticipantDbImportExport

        public IEnumerable<TripParticipant> GetParticipantsForTrip(int tripId)
        {
            var list = new List<TripParticipant>();
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByTrip;
                        cmd.AddIntParameter(":pTRPIDT", tripId);
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

        public bool Save(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var saved = false;
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
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var deleted = false;

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
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(TripParticipant entity)
        {
            Check.IsNotNull(entity, "Participant should be provided");
            var updated = false;

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
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public TripParticipant GetEntity(TripParticipantKey key)
        {
            Check.IsNotNull(key, "Key should have been provided");
            TripParticipant participant = null;
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

            }// TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return participant;
        }

        public IEnumerable<TripParticipant> GetAllEntities()
        {
            var list = new List<TripParticipant>();

            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectQuery;
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