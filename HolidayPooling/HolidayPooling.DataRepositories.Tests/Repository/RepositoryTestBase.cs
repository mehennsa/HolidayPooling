using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Infrastructure.Configuration;
using Moq;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Database;
using System;
using System.Data;

namespace HolidayPooling.DataRepositories.Tests.Repository
{
    public abstract class RepositoryTestBase<TPersister, TRepository> 
        where TPersister : class
        where TRepository : class
    {

        #region Methods

        protected Mock<TPersister> CreateMock()
        {
            return new Mock<TPersister>() { CallBase = true };
        }

        protected void CheckErrors(IRepository repository, string message)
        {
            Assert.IsTrue(repository.HasErrors);
            Assert.AreEqual(1, repository.Errors.Count);
            Assert.AreEqual(message, repository.Errors[0]);
        }

        protected void CallException()
        {
            throw new Exception("For test");
        }

        protected void DeleteTable(string tableName)
        {
            using (var con = new DatabaseConnection(DatabaseType.PostgreSql, ConnectionManager.GetConnectionString(HolidayPoolingDatabase.HP)))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM " + tableName;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Abstracts

        protected abstract TRepository CreateRepository(TPersister persister);

        #endregion


    }
}
