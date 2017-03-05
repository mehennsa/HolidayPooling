using HolidayPooling.Infrastructure.Configuration;
using Sams.Commons.Infrastructure.Cache;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace HolidayPooling.DataRepositories.Core
{
    public abstract class DbImportExportBase<TKey, TValue> : ICacheable<TKey, TValue>
    {
        #region Fields
        
        protected static bool _isCacheRefreshed = false;
        protected bool _useCache;
 
        #endregion
 
        #region .ctor
 
        
        public DbImportExportBase()
        {
            _useCache = false;
        }
 
        #endregion
 
        #region Methods
 
        protected abstract TKey CreateKeyFromReader(IDatabaseReader reader);
 
        protected abstract TValue CreateValueFromReader(IDatabaseReader reader);
 
        protected abstract TKey CreateKeyFromValue(TValue value);
 
        public abstract int GetNewId();
 
        protected abstract string GetSelectQuery();
 
        public virtual string GetConnectionString()
        {
            return ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP);
        }
 
        #endregion
 
        #region ICacheable
 
        public ICache<TKey, TValue> Cache
        {
            get { return CacheProvider<TKey, TValue>.GetCache(); }
        }
 
        public void FillCache()
        {
            if (_isCacheRefreshed)
            {
                return;
            }
            try
            {
                using (var con = new DatabaseConnection(DatabaseType.PostgreSql, 
                    ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP)))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = GetSelectQuery();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var key = CreateKeyFromReader(reader);
                                if (!Cache.Contains(key))
                                {
                                    Cache.AddElement(key, CreateValueFromReader(reader));
                                }
                            }
                        }
                    }
                }
            }
            catch (DatabaseException)
             {
 
            }
        }
 
        public void Clear()
        {
            Cache.Clear();
             _isCacheRefreshed = false;
        }
 
        #endregion
    }
}
