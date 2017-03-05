using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Converters;
using HolidayPooling.Models.Core;
using HolidayPooling.Models.Helpers;
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


        #region IPotDbImportExport

        public Pot GetTripsPot(int tripId)
        {
            Pot pot = null;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public bool IsPotNameUsed(string potName)
        {
            var found = false;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return found;
        }

        public Pot GetPotByName(string potName)
        {
            Pot pot = null;
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
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public bool Save(Pot entity)
        {
            Check.IsNotNull(entity, "Pot should be provided");

            var saved = false;

            // id
            var id = GetNewId();

            if (id <= 0)
            {
                return false;
            }

            try
            {

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
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            if (saved)
            {
                entity.Id = id;
            }

            return saved;
        }

        public bool Delete(Pot entity)
        {
            Check.IsNotNull(entity, "Pot must be provided");
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

        public bool Update(Pot entity)
        {
            var updated = false;
            Check.IsNotNull(entity, "Pot must provided for update");
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
                    }
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public Pot GetEntity(int key)
        {
            Pot pot = null;
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

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return pot;
        }

        public IEnumerable<Pot> GetAllEntities()
        {
            var list = new List<Pot>();
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