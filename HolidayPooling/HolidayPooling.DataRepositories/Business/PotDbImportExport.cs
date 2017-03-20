using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Infrastructure.Converters;
using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
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
    public class PotDbImportExport : DbImportExportBase<int, Pot>, IPotDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region SQL

        private const string InsertQuery = "INSERT INTO TPOT(IDT, ORG, POTMOD, CURMNT, TGTMNT, TRPIDT, NAM, STRDAT, ENDDAT, VALDAT, DSC, INDCANCEL, CANCELRSN, CANCELDAT)" +
                                            "VALUES (:pIDT, :pORG, :pPOTMOD, :pCURMNT, :pTGTMNT, :pTRPIDT, :pNAM, :pSTRDAT, :pENDDAT, :pVALDAT, :pDSC, :pINDCANCEL, :pCANCELRSN, :pCANCELDAT)";
        private const string DeleteQuery = "DELETE FROM TPOT WHERE IDT = :pIDT";

        private const string UpdateQuery = "UPDATE TPOT SET ORG = :pORG, POTMOD = :pPOTMOD, CURMNT = :pCURMNT," +
                                            " TGTMNT = :pTGTMNT, NAM = :pNAM, STRDAT = :pSTRDAT, ENDDAT = :pENDDAT, VALDAT = :pVALDAT," +
                                            " DSC = :pDSC, INDCANCEL = :pINDCANCEL, CANCELRSN = :pCANCELRSN, CANCELDAT = :pCANCELDAT" +
                                            " WHERE IDT = :pIDT";
        private const string SelectQuery = "SELECT IDT, ORG, POTMOD, CURMNT, TGTMNT, TRPIDT, NAM, STRDAT, ENDDAT, VALDAT, DSC, INDCANCEL, CANCELRSN, CANCELDAT FROM TPOT";

        private const string SelectById = SelectQuery + " WHERE IDT = :pIDT";

        private const string SelectByTrip = SelectQuery + " WHERE TRPIDT = :pTRPIDT";

        private const string SelectByName = SelectQuery + " WHERE NAM = :pNAM";

        private const string SelectNewId = "Select nextval('SPOTIDT')";

        #endregion

        #region .ctor

        public PotDbImportExport()
            : base()
        {

        }

        #endregion

        #region DbImportExportBase<int, Pot>

        protected override int CreateKeyFromReader(IDatabaseReader reader)
        {
            return reader.GetInt("IDT");
        }

        protected override Pot CreateValueFromReader(IDatabaseReader reader)
        {
            return new Pot
                (
                    reader.GetInt("IDT"),
                    reader.GetInt("TRPIDT"),
                    reader.GetString("ORG"),
                    ModelEnumConverter.PotModeFromString(reader.GetString("POTMOD")),
                    reader.GetDouble("CURMNT"),
                    reader.GetDouble("TGTMNT"),
                    reader.GetString("NAM"),
                    reader.GetDate("STRDAT"),
                    reader.GetDate("ENDDAT"),
                    reader.GetDate("VALDAT"),
                    reader.GetString("DSC"),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDCANCEL")),
                    reader.GetString("CANCELRSN"),
                    reader.GetNullableDate("CANCELDAT")
                );
        }

        protected override int CreateKeyFromValue(Pot value)
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


        #region IPotDbImportExport

        public Pot GetTripsPot(int tripId)
        {
            Pot pot = null;
            _logger.Info("Start retrieving Pot for a trip");
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
                            if (reader.Read())
                            {
                                pot = CreateValueFromReader(reader);
                            }
                        }
                    }
                }
                _logger.Info("End retrieving pot for a trip : " + (pot != null ? "Success" : "Failure"));

            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving trip's pot with query " + SelectByTrip, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public bool IsPotNameUsed(string potName)
        {
            var found = false;
            _logger.Info("Start trying to find if a pot name is used");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByName;
                        cmd.AddStringParameter(":pNAM", potName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            found = reader.Read();
                        }
                    }
                }
                _logger.Info("End trying to find if a pot name is used");
            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to find if a pot name is used with query : " + SelectByName, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public Pot GetPotByName(string potName)
        {
            Pot pot = null;
            _logger.Info("Start retrieving pot by its name");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByName;
                        cmd.AddStringParameter(":pNAM", potName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pot = CreateValueFromReader(reader);
                            }
                        }
                        _logger.Info("End retrieving pot by name : " + (pot != null ? "Success" : "Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to retrieve a pot by its name with query : " + SelectByName, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public bool Save(Pot entity)
        {
            Check.IsNotNull(entity, "Pot should be provided");

            var saved = false;
            _logger.Info("Start saving pot");

            try
            {
                // id
                var id = GetNewId();

                if (id <= 0)
                {
                    _logger.Info("Unable to retrieve new id to save a pot");
                    return false;
                }
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pIDT", id);
                        cmd.AddStringParameter(":pORG", entity.Organizer);
                        cmd.AddStringParameter(":pPOTMOD", ModelEnumConverter.PotModeToString(entity.Mode));
                        cmd.AddDoubleParameter(":pCURMNT", entity.CurrentAmount);
                        cmd.AddDoubleParameter(":pTGTMNT", entity.TargetAmount);
                        cmd.AddIntParameter(":pTRPIDT", entity.TripId);
                        cmd.AddStringParameter(":pNAM", entity.Name);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddDateParameter(":pENDDAT", entity.EndDate);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidityDate);
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddStringParameter(":pINDCANCEL", ConverterHelper.BoolToYesNoString(entity.IsCancelled));
                        cmd.AddStringParameter(":pCANCELRSN", entity.CancellationReason);
                        cmd.AddDateParameter(":pCANCELDAT", entity.CancellationDate);
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End saving pot " + (saved ? "Success" : "Error"));
                        if (saved)
                        {
                            entity.Id = id;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to save pot with query : " + InsertQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(Pot entity)
        {
            Check.IsNotNull(entity, "Pot must be provided");
            bool deleted = false;
            _logger.Info("Start deleting pot");
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
                        _logger.Info("End deleting pot : " + (deleted ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to delete pot with query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(Pot entity)
        {
            var updated = false;
            Check.IsNotNull(entity, "Pot must provided for update");
            _logger.Info("Start updating pot");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pIDT", entity.Id);
                        cmd.AddStringParameter(":pORG", entity.Organizer);
                        cmd.AddStringParameter(":pPOTMOD", ModelEnumConverter.PotModeToString(entity.Mode));
                        cmd.AddDoubleParameter(":pCURMNT", entity.CurrentAmount);
                        cmd.AddDoubleParameter(":pTGTMNT", entity.TargetAmount);
                        cmd.AddStringParameter(":pNAM", entity.Name);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddDateParameter(":pENDDAT", entity.EndDate);
                        cmd.AddDateParameter(":pVALDAT", entity.ValidityDate);
                        cmd.AddStringParameter(":pDSC", entity.Description);
                        cmd.AddStringParameter(":pINDCANCEL", ConverterHelper.BoolToYesNoString(entity.IsCancelled));
                        cmd.AddStringParameter(":pCANCELRSN", entity.CancellationReason);
                        cmd.AddDateParameter(":pCANCELDAT", entity.CancellationDate);
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End updating pot : " + (updated ? "Success" : "Error"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to update pot with query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public Pot GetEntity(int key)
        {
            Pot pot = null;
            _logger.Info("Start Get Pot by id");
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
                                pot = CreateValueFromReader(reader);
                            }
                        }
                    }
                }
                _logger.Info("End Get pot by Id : " + (pot != null ? "Success" : "Failure"));
            }
            catch (Exception ex)
            {
                _logger.Error("Error during Get pot with id, query : " + SelectById, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public IEnumerable<Pot> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion
    }
}