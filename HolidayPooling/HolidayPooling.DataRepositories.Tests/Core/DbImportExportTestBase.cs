using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Infrastructure.Configuration;
using Moq;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Database;
using Sams.Commons.Infrastructure.Environment;
using System;
using System.Data;

namespace HolidayPooling.DataRepositories.Tests.Core
{
    public abstract class DbImportExportTestBase<T, TKey, TEntity>
        where T : DbImportExportBase<TKey, TEntity>, IDbImportExport<TKey, TEntity>, new()
        where TEntity : class
    {

        #region Properties

        protected T _importExport;
        protected HpEnvironment _env;
        protected static readonly DateTime _insertTime = new DateTime(2017, 06, 04, 01, 24, 30);
        protected static readonly DateTime _updateTime = new DateTime(2017, 06, 05, 01, 24, 30);


        protected abstract string TableName
        {
            get;
        }

        #endregion

        #region Methods

        public abstract void CompareWithDbValues(TEntity entity, TEntity dbEntity, DateTime modificationDate);

        protected abstract T CreateImportExport();

        protected abstract T CreateImportExportForUpdate();

        protected abstract TEntity CreateModel();

        protected abstract TKey GetKeyFromModel(TEntity entity);

        protected abstract void UpdateModel(TEntity model);


        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            _env = new HpEnvironment();
            _env.SetupEnvironment(AppEnvironment.TEST);
            _importExport = CreateImportExport();
            DeleteTable();

        }

        [TearDown]
        public virtual void TearDown()
        {
            DeleteTable();
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            DeleteTable();
            _env.Dispose();
            _importExport = null;
        }

        [SetUp]
        public void Setup()
        {
            _importExport = CreateImportExport();
        }

        #endregion

        #region Helpers

        protected Mock<T> CreateMock(bool withException = true)
        {
            var mock = new Mock<T> { CallBase = true };
            if (withException)
            {
                mock.Setup(s => s.GetConnectionString()).Throws(new Exception("Thrown for test purpose"));
            }

            return mock;
        }

        protected void ArgumentNullExceptionTest(TestDelegate toTest)
        {
            Assert.Throws<ArgumentNullException>(toTest);
        }

        protected void ImportExportExceptionTest(TestDelegate toTest)
        {
            Assert.Throws<ImportExportException>(toTest);
        }

        protected void DeleteTable()
        {
            using (var con = new DatabaseConnection(DatabaseType.PostgreSql, _importExport.GetConnectionString()))
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM " + TableName;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Tests


        [Test]
        public void Save_WhenExceptionThrow_ShouldThrowImportExportException()
        {
            var model = CreateModel();
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.Save(model));
        }

        [Test]
        public void Save_WithNull_ShouldThrowArgumentNullException()
        {
            ArgumentNullExceptionTest(() => _importExport.Save(null));
        }

        [Test]
        public void Save_ShouldSaveRecordInDb()
        {
            var model = CreateModel();
            Assert.IsTrue(_importExport.Save(model));
            var dbEntity = _importExport.GetEntity(GetKeyFromModel(model));
            Assert.IsNotNull(dbEntity);
            CompareWithDbValues(model, dbEntity, _insertTime);
        }

        [Test]
        public void Delete_WithNullEntity_ShouldThrowArgumentNullException()
        {
            ArgumentNullExceptionTest(() => _importExport.Delete(null));
        }

        [Test]
        public void Delete_WhenExceptionIsThrow_ShouldThrowImportExportException()
        {
            var mock = CreateMock();
            ImportExportExceptionTest(() => mock.Object.Delete(CreateModel()));
        }

        [Test]
        public void Delete_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.Delete(CreateModel()));
        }

        [Test]
        public void Delete_ShouldDeleteRecordInDb()
        {
            var model = CreateModel();
            Assert.IsTrue(_importExport.Save(model));
            var key = GetKeyFromModel(model);
            Assert.IsTrue(_importExport.Delete(model));
            Assert.IsNull(_importExport.GetEntity(key));
        }

        [Test]
        public void Update_WithNull_ShouldThrowArgumentNullException()
        {
            ArgumentNullExceptionTest(() => _importExport.Update(null));
        }

        [Test]
        public void Update_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.Update(CreateModel()));
        }

        [Test]
        public void Update_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.Update(CreateModel()));
        }

        [Test]
        public void Update_ShouldUpdateRecordInDb()
        {
            _importExport = CreateImportExportForUpdate();
            var model = CreateModel();
            Assert.IsTrue(_importExport.Save(model));
            UpdateModel(model);
            Assert.IsTrue(_importExport.Update(model));
            var dbEntity = _importExport.GetEntity(GetKeyFromModel(model));
            Assert.IsNotNull(dbEntity);
            CompareWithDbValues(model, dbEntity, _updateTime);
        }

        [Test]
        public void GetEntity_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetEntity(GetKeyFromModel(CreateModel())));
        }

        [Test]
        public void GetAllEntities_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetAllEntities());
        }

        #endregion

    }
}
