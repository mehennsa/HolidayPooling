using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Infrastructure.TimeProviders;
using Sams.Commons.Infrastructure.Cache;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;

namespace HolidayPooling.DataRepositories.Core
{
    public abstract class DbImportExportBase<TKey, TValue> : ICacheable<TKey, TValue>
    {
        #region Fields

        protected bool _useCache;
        protected readonly ITimeProvider _timeProvider;

        #endregion

        #region Properties

        protected ITimeProvider TimeProvider
        {
            get { return _timeProvider ?? new TimeProvider(); }
        }

        #endregion

        #region .ctor


        public DbImportExportBase()
        {
            _useCache = false;
        }

        internal DbImportExportBase(ITimeProvider timeProvider) : this()
        {
            _timeProvider = timeProvider;
        }
 
        #endregion
 
        #region Methods
 
        protected abstract TKey CreateKeyFromReader(IDatabaseReader reader);
 
        protected abstract TValue CreateValueFromReader(IDatabaseReader reader);
 
        protected abstract TKey CreateKeyFromValue(TValue value);

        protected virtual string NewIdQuery()
        {
            return string.Empty;
        }

        public virtual int GetNewId()
        {
            var logger = LoggerManager.GetLogger(LoggerNames.DbLogger);
            int result = -1;
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = NewIdQuery();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error while trying to execute query : " + NewIdQuery(), ex);   
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return result;
        }
 
        protected abstract string GetSelectQuery();
 
        public virtual string GetConnectionString()
        {
            return ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP);
        }

        protected virtual IEnumerable<TValue> GetListValuesWithIdParameter(string query, string parameterName, int id)
        {
            var logger = LoggerManager.GetLogger(LoggerNames.DbLogger);
            var list = new List<TValue>();
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, GetConnectionString()))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query;
                        cmd.AddIntParameter(parameterName, id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(CreateValueFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while trying to execute the following query : " + query, ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }

        protected virtual IEnumerable<TValue> InternalGetAllEntities(Action<TValue> beforeAdd = null)
        {
            var logger = LoggerManager.GetLogger(LoggerNames.DbLogger);
            var list = new List<TValue>();
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
                                var value = CreateValueFromReader(reader);
                                if (beforeAdd != null)
                                {
                                    beforeAdd(value);
                                }
                                list.Add(value);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occured while trying to execute the following query : " + GetSelectQuery(), ex);
                throw new ImportExportException("Error occured during database access " + ex.Message, ex);
            }

            return list;
        }
 
        #endregion
 
        #region ICacheable
 
        public ICache<TKey, TValue> Cache
        {
            get { return CacheProvider<TKey, TValue>.GetCache(); }
        }
 
        public void FillCache()
        {
            throw new NotImplementedException();
        }
 
        public void Clear()
        {
            Cache.Clear();
        }
 
        #endregion
    }
}
