using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
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

        public override int GetNewId()
        {
            var result = -1;
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
            return SelectQuery;
        }

        #endregion

        #region ITripDbImportExport

        public bool IsTripNameUsed(string name)
        {
            var result = false;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public Trip GetTripByName(string name)
        {
            Trip trip = null;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return trip;
        }

        public IEnumerable<Trip> GetValidTrips(DateTime validityDate)
        {
            var list = new List<Trip>();
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
            }//TODO : Log
            catch (Exception ex)
            {
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
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        public bool Save(Trip entity)
        {
            Check.IsNotNull(entity, "Trip should be provided");

            // new id
            int id = GetNewId();

            if (id <= 0)
            {
                return false;
            }

            var result = false;
            try
            {
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
                        cmd.AddDoubleParameter(":pTRPNOT", 0);
                        result = cmd.ExecuteNonQuery() > 0;
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            if (result)
            {
                entity.Id = id;
            }

            return result;
        }

        public bool Delete(Trip entity)
        {
            Check.IsNotNull(entity, "A Trip should be provided");

            var result = false;
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
                    }
                }

            }//TODO : log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public bool Update(Trip entity)
        {
            Check.IsNotNull(entity, "Trip should be provided");

            var result = false;
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
                    }
                }
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }

        public Trip GetEntity(int key)
        {
            Trip trip = null;
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
            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return trip;
        }

        public IEnumerable<Trip> GetAllEntities()
        {
            var list = new List<Trip>();
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