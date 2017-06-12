using HolidayPooling.DataRepositories.ImportExport;
using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Tests;
using NUnit.Framework;
using Sams.Commons.Infrastructure.Environment;
using System;
using System.Transactions;

namespace HolidayPooling.DataRepositories.Tests.Repository
{
    [TestFixture]
    public class UserRepositoryIntegrationTest  : RepositoryTestBase<IUserDbImportExport, IUserRepository>
    {

        #region Fields

        private IUserRepository _repo;
        protected HpEnvironment _env;

        #endregion

        #region SetUp & TearDown

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _env = new HpEnvironment();
            _env.SetupEnvironment(AppEnvironment.TEST);
            _repo = new UserRepository();
        }

        [SetUp]
        public void Setup()
        {
            DeleteTable("TUSR");
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTable("TUSR");
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _env.Dispose();
            _env = null;
        }

        #endregion

        #region Tests

        [Test]
        public void Save_WhenRollBack_ShouldRollbackTransaction()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            using (var scope = new TransactionScope())
            {
                try
                {
                    
                    _repo.SaveUser(user);
                    Assert.IsFalse(_repo.HasErrors);
                    Assert.IsNotNull(_repo.GetUser(user.Id));
                    CallException();
                    scope.Complete();

                }catch
                {
                    // do notihing
                }
            }


            Assert.IsNull(_repo.GetUser(user.Id));
        }

        [Test]
        public void Save_WhenCommit_ShouldSaveInDb()
        {
            var user = ModelTestHelper.CreateUser(1, "toto");
            using (var scope = new TransactionScope())
            {
                try
                {

                    _repo.SaveUser(user);
                    Assert.IsNotNull(_repo.GetUser(user.Id));
                    Assert.IsFalse(_repo.HasErrors);
                    scope.Complete();

                }
                catch
                {
                    Assert.Fail("Exception should not be thrown");
                }
            }


            Assert.IsNotNull(_repo.GetUser(user.Id));
        }


        [Test]
        public void Update_WhenException_ShouldRollbackTransaction()
        {
            var user = ModelTestHelper.CreateUser(1, "UpdateUser");
            using (var saveScope = new TransactionScope())
            {
                try
                {
                    _repo.SaveUser(user);
                    Assert.IsFalse(_repo.HasErrors);
                    saveScope.Complete();

                }catch
                {
                    Assert.Fail("Save should not throw execption");
                }
            }

            Assert.IsNotNull(_repo.GetUser(user.Id));

            var oldNumber = user.PhoneNumber;
            user.PhoneNumber = "New Phone Number";

            using(var updateScope = new TransactionScope())
            {
                try
                {
                    _repo.UpdateUser(user);
                    var dbUser = _repo.GetUser(user.Id);
                    Assert.IsNotNull(dbUser);
                    Assert.AreEqual("New Phone Number", dbUser.PhoneNumber);
                    CallException();
                    updateScope.Complete();
                }
                catch
                {
                    // do nothing
                }
            }

            var endUser = _repo.GetUser(user.Id);
            Assert.IsNotNull(endUser);
            Assert.AreEqual(oldNumber, endUser.PhoneNumber);
        }

        [Test]
        public void Update_WhenValid_ShouldCommitTransaction()
        {
            var user = ModelTestHelper.CreateUser(1, "UpdateUserValid");
            using (var saveScope = new TransactionScope())
            {
                try
                {
                    _repo.SaveUser(user);
                    Assert.IsFalse(_repo.HasErrors);
                    saveScope.Complete();

                }
                catch
                {
                    Assert.Fail("Save should not throw execption");
                }
            }

            Assert.IsNotNull(_repo.GetUser(user.Id));


            user.PhoneNumber = "New Phone Number";

            using (var updateScope = new TransactionScope())
            {
                try
                {
                    _repo.UpdateUser(user);
                    var dbUser = _repo.GetUser(user.Id);
                    Assert.IsNotNull(dbUser);
                    updateScope.Complete();
                }
                catch
                {
                    Assert.Fail("Update should not throw execption");
                }
            }

            var endUser = _repo.GetUser(user.Id);
            Assert.IsNotNull(endUser);
            Assert.AreEqual("New Phone Number", endUser.PhoneNumber);
        }

        [Test]
        public void Delete_WhenValid_ShouldCommitTransaction()
        {
            var user = ModelTestHelper.CreateUser(1, "DeleteUserValid");
            using (var saveScope = new TransactionScope())
            {
                try
                {
                    _repo.SaveUser(user);
                    Assert.IsFalse(_repo.HasErrors);
                    saveScope.Complete();

                }
                catch
                {
                    Assert.Fail("Save should not throw execption");
                }
            }

            Assert.IsNotNull(_repo.GetUser(user.Id));

            using (var deleteScope = new TransactionScope())
            {
                try
                {
                    _repo.DeleteUser(user);
                    var dbUser = _repo.GetUser(user.Id);
                    Assert.IsNull(dbUser);
                    deleteScope.Complete();
                }
                catch
                {
                    Assert.Fail("Delete should not throw execption");
                }
            }

            var endUser = _repo.GetUser(user.Id);
            Assert.IsNull(endUser);
        }

        [Test]
        public void Delete_WhenException_ShouldRollbackTransaction()
        {
            var user = ModelTestHelper.CreateUser(1, "DeleteUser");
            using (var saveScope = new TransactionScope())
            {
                try
                {
                    _repo.SaveUser(user);
                    Assert.IsFalse(_repo.HasErrors);
                    saveScope.Complete();

                }
                catch
                {
                    Assert.Fail("Save should not throw execption");
                }
            }

            Assert.IsNotNull(_repo.GetUser(user.Id));

            using (var deleteScope = new TransactionScope())
            {
                try
                {
                    _repo.DeleteUser(user);
                    var dbUser = _repo.GetUser(user.Id);
                    Assert.IsNull(dbUser);
                    CallException();
                    deleteScope.Complete();
                }
                catch
                {
                    // Do nothing
                }
            }

            var endUser = _repo.GetUser(user.Id);
            Assert.IsNotNull(endUser);
        }

        #endregion

        #region RepositoryTestBase<IUserDbImportExport, IUserRepository>

        protected override IUserRepository CreateRepository(IUserDbImportExport persister)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
