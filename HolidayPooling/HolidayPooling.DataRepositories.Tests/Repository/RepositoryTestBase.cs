using HolidayPooling.DataRepositories.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

        #region Abstracts

        protected abstract TRepository CreateRepository(TPersister persister);

        #endregion


    }
}
