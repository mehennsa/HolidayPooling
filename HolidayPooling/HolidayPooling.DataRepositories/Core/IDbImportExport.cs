using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Core
{
    public interface IDbImportExport<TKey, TEntity> where TEntity : class
    {

        bool Save(TEntity entity);

        bool Delete(TEntity entity);

        bool Update(TEntity entity);

        TEntity GetEntity(TKey key);

        IEnumerable<TEntity> GetAllEntities();

    }
}