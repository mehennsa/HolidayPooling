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
    public class FriendshipDbImportExport : DbImportExportBase<FriendshipKey, Friendship>, IFriendshipDbImportExport
    {

        #region Fields

        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.DbLogger);

        #endregion

        #region Properties

        #endregion

        #region SQL

        private const string InsertQuery = "INSERT INTO TFRDSHP (USRIDT, FRDPSD, STRDAT, INDRSQUSR, INDWTG, DATEFT)" +
                                            " VALUES (:pUSRIDT, :pFRDPSD, :pSTRDAT, :pINDRSQUSR, :pINDWTG, :pDATEFT)";

        private const string DeleteQuery = "DELETE FROM TFRDSHP WHERE USRIDT = :pUSRIDT AND FRDPSD = :pFRDPSD";

        private const string UpdateQuery = "UPDATE TFRDSHP SET STRDAT = :pSTRDAT, INDRSQUSR = :pINDRSQUSR, INDWTG = :pINDWTG, DATEFT = :pDATEFT" +
                                            " WHERE USRIDT = :pUSRIDT AND FRDPSD = :pFRDPSD";

        private const string SelectQuery = "SELECT USRIDT, FRDPSD, STRDAT, INDRSQUSR, INDWTG, DATEFT FROM TFRDSHP";

        private const string SelectByUserId = SelectQuery + " WHERE USRIDT = :pUSRIDT";

        private const string SelectByKey = SelectByUserId + " AND FRDPSD = :pFRDPSD";

        private const string SelectWaitingFriendship = SelectByUserId + " AND INDWTG = 'Y' AND INDRSQUSR = 'N'";

        private const string SelectRequestedFrienship = SelectByUserId + " AND INDWTG = 'Y' AND INDRSQUSR = 'Y'";

        #endregion

        #region .ctor

        public FriendshipDbImportExport()
            : base()
        {

        }

        internal FriendshipDbImportExport(ITimeProvider timeProvider) : base(timeProvider)
        {
        }

        #endregion

        #region DbImportExportBase<FriendshipKey, Friendship>

        protected override FriendshipKey CreateKeyFromReader(IDatabaseReader reader)
        {
            return new FriendshipKey(reader.GetInt("USRIDT"), reader.GetString("FRDPSD"));
        }

        protected override Friendship CreateValueFromReader(IDatabaseReader reader)
        {
            var friendship = new  Friendship
                (
                    reader.GetInt("USRIDT"),
                    reader.GetString("FRDPSD"),
                    reader.GetDate("STRDAT"),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDRSQUSR")),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDWTG"))
                );
            friendship.ModificationDate = reader.GetDate("DATEFT");
            return friendship;
        }

        protected override FriendshipKey CreateKeyFromValue(Friendship value)
        {
            return new FriendshipKey(value.UserId, value.FriendName);
        }

        protected override string GetSelectQuery()
        {
            return SelectQuery;
        }

        #endregion

        #region IFriendshipDbImportExport

        public IEnumerable<Friendship> GetUserFriendships(int userId)
        {
            return GetListValuesWithIdParameter(SelectByUserId, ":pUSRIDT", userId);
        }

        public IEnumerable<Friendship> GetRequestedFriendships(int userId)
        {
            return GetListValuesWithIdParameter(SelectRequestedFrienship, ":pUSRIDT", userId);
        }

        public IEnumerable<Friendship> GetWaitingFriendships(int userId)
        {
            return GetListValuesWithIdParameter(SelectWaitingFriendship, ":pUSRIDT", userId);
        }

        public bool Save(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship sould be provided");

            bool saved = false;
            entity.ModificationDate = TimeProvider.Now();
            _logger.Info("Start saving Friendship");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = InsertQuery;
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pFRDPSD", entity.FriendName);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddStringParameter(":pINDRSQUSR", ConverterHelper.BoolToYesNoString(entity.IsRequested));
                        cmd.AddStringParameter(":pINDWTG", ConverterHelper.BoolToYesNoString(entity.IsWaiting));
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        saved = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End Saving Friendship. Result : " + (saved ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to save friendship with the following query " + InsertQuery, ex);   
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return saved;
        }

        public bool Delete(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship should be provided");
            var deleted = false;
            _logger.Info("Start deleting Friendship");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = DeleteQuery;
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pFRDPSD", entity.FriendName);
                        deleted = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End deleting Friendship : " + (deleted ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to delete friendship with the following query : " + DeleteQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return deleted;
        }

        public bool Update(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship should be provided");

            var updated = false;
            entity.ModificationDate = TimeProvider.Now();
            _logger.Info("Start updating Friendship");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = UpdateQuery;
                        cmd.AddIntParameter(":pUSRIDT", entity.UserId);
                        cmd.AddStringParameter(":pFRDPSD", entity.FriendName);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddStringParameter(":pINDRSQUSR", ConverterHelper.BoolToYesNoString(entity.IsRequested));
                        cmd.AddStringParameter(":pINDWTG", ConverterHelper.BoolToYesNoString(entity.IsWaiting));
                        cmd.AddDateTimeParameter(":pDATEFT", entity.ModificationDate);
                        updated = cmd.ExecuteNonQuery() > 0;
                        _logger.Info("End updating Friendship : " + (updated ? "Success" : "Failure"));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error while trying to update Friendship with the following query : " + UpdateQuery, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public Friendship GetEntity(FriendshipKey key)
        {
            Check.IsNotNull(key, "Key should be provided");
            Friendship friendship = null;
            _logger.Info("Start Retrieving Friendship");
            try
            {

                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = SelectByKey;
                        cmd.AddIntParameter(":pUSRIDT", key.UserId);
                        cmd.AddStringParameter(":pFRDPSD", key.FriendName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                friendship = CreateValueFromReader(reader);
                            }
                        }
                    }
                    _logger.Info("End Retrieving Friendship " + (friendship != null ? "Success" : "Failure"));
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occured while trying to retrieve Friendship with the following query " + SelectByKey);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return friendship;
        }

        public IEnumerable<Friendship> GetAllEntities()
        {
            return InternalGetAllEntities();
        }

        #endregion



    }
}