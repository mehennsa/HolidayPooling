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
    public class FriendshipDbImportExport : DbImportExportBase<FriendshipKey, Friendship>, IFriendshipDbImportExport
    {

        #region SQL

        private const string InsertQuery = "INSERT INTO TFRDSHP (USRIDT, FRDPSD, STRDAT, INDRSQUSR, INDWTG)" +
                                            " VALUES (:pUSRIDT, :pFRDPSD, :pSTRDAT, :pINDRSQUSR, :pINDWTG)";

        private const string DeleteQuery = "DELETE FROM TFRDSHP WHERE USRIDT = :pUSRIDT AND FRDPSD = :pFRDPSD";

        private const string UpdateQuery = "UPDATE TFRDSHP SET STRDAT = :pSTRDAT, INDRSQUSR = :pINDRSQUSR, INDWTG = :pINDWTG" +
                                            " WHERE USRIDT = :pUSRIDT AND FRDPSD = :pFRDPSD";

        private const string SelectQuery = "SELECT USRIDT, FRDPSD, STRDAT, INDRSQUSR, INDWTG FROM TFRDSHP";

        private const string SelectByUserId = SelectQuery + " WHERE USRIDT = :pUSRIDT";

        private const string SelectByKey = SelectByUserId + " AND FRDPSD = :pFRDPSD";

        #endregion

        #region .ctor

        public FriendshipDbImportExport()
            : base()
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
            return new Friendship
                (
                    reader.GetInt("USRIDT"),
                    reader.GetString("FRDPSD"),
                    reader.GetDate("STRDAT"),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDRSQUSR")),
                    ConverterHelper.YesNoStringToBool(reader.GetString("INDWTG"))
                );
        }

        protected override FriendshipKey CreateKeyFromValue(Friendship value)
        {
            return new FriendshipKey(value.UserId, value.FriendName);
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

        #region IFriendshipDbImportExport

        public IEnumerable<Friendship> GetUserFriendships(int userId)
        {
            var list = new List<Friendship>();
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

        public bool Save(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship sould be provided");

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
                        cmd.AddStringParameter(":pFRDPSD", entity.FriendName);
                        cmd.AddDateParameter(":pSTRDAT", entity.StartDate);
                        cmd.AddStringParameter(":pINDRSQUSR", ConverterHelper.BoolToYesNoString(entity.IsRequested));
                        cmd.AddStringParameter(":pINDWTG", ConverterHelper.BoolToYesNoString(entity.IsWaiting));
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

        public bool Delete(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship should be provided");
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
                        cmd.AddStringParameter(":pFRDPSD", entity.FriendName);
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

        public bool Update(Friendship entity)
        {
            Check.IsNotNull(entity, "Friendship should be provided");

            var updated = false;
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
                        updated = cmd.ExecuteNonQuery() > 0;
                    }
                }

            }// TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return updated;
        }

        public Friendship GetEntity(FriendshipKey key)
        {
            Check.IsNotNull(key, "Key should be provided");
            Friendship friendship = null;

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
                }

            }//TODO : Log
            catch (Exception ex)
            {
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return friendship;
        }

        public IEnumerable<Friendship> GetAllEntities()
        {
            var list = new List<Friendship>();

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