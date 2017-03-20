using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using Sams.Commons.Infrastructure.Checks;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class TripDbImportExport : DbImportExportBase<int, Trip>, ITripDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL

        private const string InsertQuery = "INSERT INTO TTRP (IDT, NAM, DSC, PRC, NBRMAXPRS, LOC, ORG, STRDAT, ENDDAT, VALDAT, TRPNOT)" +
                                            " VALUES (:pIDT, :pNAM, :pDSC, :pPRC, :pNBRMAXPRS, :pLOC, :pORG, :pSTRDAT, :pENDDAT, :pVALDAT, :pTRPNOT)";
        private const string DeleteQuery = "DELETE FROM TTRP WHERE IDT = :pIDT";

        private const string UpdateQuery = "UPDATE TTRP SET DSC = :pDSC, PRC = :pPRC, NBRMAXPRS = :pNBRMAXPRS, LOC = :pLOC," +
                                            " ORG = :pORG, STRDAT = :pSTRDAT, ENDDAT = :pENDDAT, VALDAT = :pVALDAT, TRPNOT = :pTRPNOT" +
                                            " WHERE IDT = :pIDT";

        private const string SelectQuery = "Select IDT, NAM, DSC, PRC, NBRMAXPRS, LOC, ORG, STRDAT, ENDDAT, VALDAT, TRPNOT FROM TTRP";

        private const string SelectById = SelectQuery + " WHERE IDT = :pIDT";

        private const string SelectByName = SelectQuery + " WHERE NAM = :pNAM";

        private const string SelectOnValidity = SelectQuery + " WHERE VALDAT >= :pVALDAT";

        private const string SelectNewId = "Select nextval('STRPIDT')";

        #endregion

        #region .ctor

        public TripDbImportExport()
            : base()
        {

        }

        #endregion

        #region DbImportExportBase<int, Trip>

        protected override int CreateKeyFromReader(IDatabaseReader reader)
        {
            return reader.GetInt("IDT");
        }

        protected override Trip CreateValueFromReader(IDatabaseReader reader)
        {
            return new Trip
                (
                    reader.GetInt("IDT"),
                    reader.GetString("NAM"),
                    reader.GetDouble("PRC"),
                    reader.GetString("DSC"),
                    reader.GetInt("NBRMAXPRS"),
                    reader.GetString("LOC"),
                    reader.GetString("ORG"),
                    reader.GetDate("STRDAT"),
                    reader.GetDate("ENDDAT"),
                    reader.GetDate("VALDAT"),
                    reader.GetDouble("TRPNOT")
                );
        }

        protected override int CreateKeyFromValue(Trip value)
        {
            return value.Id;
        }

        protected override string NewIdQuery()
        {
            return SelectNewId;
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region ITripDbImportExport

        public bool IsTripNameUsed(string name)
        {
            var result = false;
            _logger.Info("Start checking if trip name is used");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByName;
                        cmd.AddStringParameter(":pNAM", name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            result = reader.Read();
                        }
                    }
                }
                _logger.Info("End checking if trip name is used");
            }
            catch (Exception ex)
            {
                _logger.Error("Error while checking if trip name is used with query : " + SelectByName, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public Trip GetTripByName(string name)
        {
            Trip trip = null;
            _logger.Info("Start retrieving trip by name");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByName;
                        cmd.AddStringParameter(":pNAM", name);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                trip = CreateValueFromReader(reader);
                            }
                        }
                    }
                }
                _logger.Info("End retrieving trip by name : " + (trip != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving trip by name with query : " + SelectByName, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return trip;
        }

        public IEnumerable<Trip> GetValidTrips(DateTime validityDate)
        {
            var list = new List<Trip>();
            _logger.Info(string.Format("Start retrieving trips by validity date : {0}", validityDate));
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectOnValidity;
                        cmd.AddDateParameter(":pVALDAT", validityDate);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }
                _logger.Info(string.Format("End retrieving trip by validity date {0}", validityDate));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error while retrieving trip by validity date {0} with query {1}", validityDate, SelectOnValidity), ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        public IEnumerable<Trip> GetTripBetweenStartDateAndEndDate(DateTime? startDate, DateTime? endDate)
        {
            if ((!startDate.HasValue) && (!endDate.HasValue))
            {
                throw new ArgumentException("StartDate or EndDate should be provided");
            }

            var list = new List<Trip>();
            _logger.Info(string.Format("Start retrieving trips between start date {0} and end date {1}", startDate, endDate));
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        StringBuilder builder = new StringBuilder();
                        builder.Append(SelectQuery);
                        builder.Append(" WHERE 1=1");
                        if (startDate.HasValue)
                        {
                            builder.Append(" AND STRDAT >= :pSTRDAT");
                            cmd.AddDateParameter(":pSTRDAT", startDate.Value);
                        }

                        if (endDate.HasValue)
                        {
                            builder.Append(" AND ENDDAT <= :pENDDAT");
                            cmd.AddDateParameter(":pENDDAT", endDate.Value);
                        }

                        cmd.CommandText = builder.ToString();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }
                _logger.Info(string.Format("End retrieving trips between start date {0} and end date {1}", startDate, endDate));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error while retrieving trips between start date {0} and end date {1}", startDate, endDate), ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        public bool Save(Trip entity)
        {
            Check.IsNotNull(entity, "Trip should be provided");

            var result = false;
            _logger.Info("Start saving trip");
            try
            {

                // new id
                int id = GetNewId();

                if (id <= 0)
                {
                    _logger.Info("New id cannot be generated to save trip : Failure");
                    return false;
                }

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pIDT", id);
                        cmd.AddStringParameter(":pNAM", entity.TripName);
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddDoubleParameter(":pPRC", entity.Price);
                        cmd.AddIntParameter(":pNBRMAXPRS", entity.NumberMaxOfPeople);
                        cmd.AddStringParameter(":pLOC", entity.Location);
                        cmd.AddStringParameter(":pORG", entity.Organizer);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddDateParameter(":pENDDAT", entity.EndDate);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidityDate);
                        cmd.AddDoubleParameter(":pTRPNOT", entity.Note);
                        result = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End saving trip : " + (result ? "Success" : "Failure"));
                        if (result)
                        {
                            entity.Id = id;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while saving trip with query : " + InsertQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public bool Delete(Trip entity)
        {
            Check.IsNotNull(entity, "A Trip should be provided");

            var result = false;
            _logger.Info("Start Deleting trip");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = DeleteQuery;
                        cmd.AddIntParameter(":pIDT", entity.Id);
                        result = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End Deleting trip " + (result ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while deleting trip with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public bool Update(Trip entity)
        {
            Check.IsNotNull(entity, "Trip should be provided");

            var result = false;
            _logger.Info("Start updating trip");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pIDT", entity.Id);
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddDoubleParameter(":pPRC", entity.Price);
                        cmd.AddIntParameter(":pNBRMAXPRS", entity.NumberMaxOfPeople);
                        cmd.AddStringParameter(":pLOC", entity.Location);
                        cmd.AddStringParameter(":pORG", entity.Organizer);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddDateParameter(":pENDDAT", entity.EndDate);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidityDate);
                        cmd.AddDoubleParameter(":pTRPNOT", entity.Note);
                        result = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End updating trip " + (result ? "Success" : "Failure"));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while updating trip with query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public Trip GetEntity(int key)
        {
            Trip trip = null;
            _logger.Info("Start retriving trip by id");
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectById;
                        cmd.AddIntParameter(":pIDT", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                trip = CreateValueFromReader(reader);
                            }
                        }
                    }
                }
                _logger.Info("End retrieving trip by id " + (trip != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving trip with query : " + SelectById, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return trip;
        }

        public IEnumerable<Trip> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion
    }
}