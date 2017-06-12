using HolidayPooling.DataRepositories.ImportExport;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Tests;
using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;

namespace HolidayPooling.DataRepositories.Tests.Repository
{
    [TestFixture]
    public class UserRepositoryTest : RepositoryTestBase<IUserDbImportExport, IUserRepository>
    {

        #region Properties

        private const string NullUserErrorMessage = "Please provide a valid user";

        #endregion

        #region Tests

        [Test]
        public void Save_WhenUserIsNull_ShouldLogError()
        {
            var mock = CreateMock();
            var repository = CreateRepository(mock.Object);
            repository.SaveUser(null);
            CheckErrors(repository, NullUserErrorMessage);
        }

        [Test]
        public void Save_WhenMailAlreadyExist_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsMailUsed(It.IsAny<string>())).Returns(true);
            var repository = CreateRepository(mock.Object);
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            repository.SaveUser(user);
            CheckErrors(repository, "Mail is already used, please choose another one");
        }

        [Test]
        public void Save_WhenPseudoAlreadyExist_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPseudoUsed(It.IsAny<string>())).Returns(true);
            var repository = CreateRepository(mock.Object);
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            repository.SaveUser(user);
            CheckErrors(repository, "Pseudo is already used, please choose another one");
        }

        [Test]
        public void Save_WhenImportExportExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPseudoUsed(It.IsAny<string>())).Throws(new ImportExportException("ImportExportExceptionTest"));
            var repository = CreateRepository(mock.Object);
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            repository.SaveUser(user);
            CheckErrors(repository, "ImportExportExceptionTest");
        }

        [Test]
        public void Save_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPseudoUsed(It.IsAny<string>())).Throws(new Exception("ExceptionForTest"));
            var repository = CreateRepository(mock.Object);
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            repository.SaveUser(user);
            CheckErrors(repository, "Unexpected error occured ExceptionForTest");
        }

        [Test]
        public void Save_WhenCannotSaveInDb_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.IsPseudoUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.IsMailUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(It.IsAny<User>())).Returns(false);
            var repository = CreateRepository(mock.Object);
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            repository.SaveUser(user);
            CheckErrors(repository, "Internal error occured when saving user : account has not been created");
        }

        [Test]
        public void Save_WhenValid_ShouldNotSetErrors() 
        {
            var user = ModelTestHelper.CreateUser(1, "RepositoryUser");
            var mock = CreateMock();
            mock.Setup(s => s.IsPseudoUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.IsMailUsed(It.IsAny<string>())).Returns(false);
            mock.Setup(s => s.Save(user)).Returns(true);
            var repository = CreateRepository(mock.Object);
            repository.SaveUser(user);
            Assert.IsFalse(repository.HasErrors);
        }

        [Test]
        public void UpdateUser_WhenUserNull_ShouldLogErrors()
        {
            var mock = CreateMock();
            var repository = CreateRepository(mock.Object);
            repository.UpdateUser(null);
            CheckErrors(repository, NullUserErrorMessage);
        }

        [Test]
        public void UpdateUser_WhenExceptionThrown_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(2, "UpdateUser");
            var mock = CreateMock();
            mock.Setup(s => s.Update(user)).Throws(new ImportExportException("ExceptionForTest"));
            var repository = CreateRepository(mock.Object);
            repository.UpdateUser(user);
            CheckErrors(repository, "ExceptionForTest");
        }

        [Test]
        public void Update_WhenDbUpdateNotSucceed_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(2, "UpdateUser");
            var mock = CreateMock();
            mock.Setup(s => s.Update(user)).Returns(false);
            var repository = CreateRepository(mock.Object);
            repository.UpdateUser(user);
            CheckErrors(repository, "Internal Error occured : user cannot be updated");
        }

        [Test]
        public void Update_WhenDbUpdateSuccees_ShouldNotSetError()
        {
            var user = ModelTestHelper.CreateUser(2, "UpdateUser");
            var mock = CreateMock();
            mock.Setup(s => s.Update(user)).Returns(true);
            var repository = CreateRepository(mock.Object);
            repository.UpdateUser(user);
            Assert.IsFalse(repository.HasErrors);
        }

        [Test]
        public void Delete_WhenUserIsNull_ShouldLogError()
        {
            var mock = CreateMock();
            var repository = CreateRepository(mock.Object);
            repository.DeleteUser(null);
            CheckErrors(repository, NullUserErrorMessage);
        }

        [Test]
        public void Delete_WhenExceptionIsThrown_ShouldLogError()
        {
            var user = ModelTestHelper.CreateUser(3, "DeleteUser");
            var mock = CreateMock();
            mock.Setup(s => s.Delete(user)).Throws(new ImportExportException("ExceptionForTest"));
            var repository = CreateRepository(mock.Object);
            repository.DeleteUser(user);
            CheckErrors(repository, "ExceptionForTest");
        }

        [Test]
        public void Delete_WhenErrorInDb_ShoudLogError()
        {
            var user = ModelTestHelper.CreateUser(3, "DeleteUser");
            var mock = CreateMock();
            mock.Setup(s => s.Delete(user)).Returns(false);
            var repository = CreateRepository(mock.Object);
            repository.DeleteUser(user);
            CheckErrors(repository, "Internal occured when deleting user : account has not been deleted");
        }

        [Test]
        public void Delete_WhenDbDeleteSuccessful_ShouldNotLogError()
        {
            var user = ModelTestHelper.CreateUser(3, "DeleteUser");
            var mock = CreateMock();
            mock.Setup(s => s.Delete(user)).Returns(true);
            var repository = CreateRepository(mock.Object);
            repository.DeleteUser(user);
            Assert.IsFalse(repository.HasErrors);
        }

        [Test]
        public void GetUsers_WhenExceptionThrow_ShouldLogErrors()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities()).Throws(new ImportExportException("ExceptionForTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUsers();
            CheckErrors(repo, "ExceptionForTest");
        }

        [Test]
        public void GetUsers_ShouldReturnUserList()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetAllEntities())
                .Returns(new List<User> { ModelTestHelper.CreateUser(1, "First"), ModelTestHelper.CreateUser(2, "Second") });
            var repo = CreateRepository(mock.Object);
            var list = repo.GetUsers();
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
        }

        [Test]
        public void GetUser_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetEntity(It.IsAny<int>())).Throws(new ImportExportException("ExceptionForTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUser(1);
            CheckErrors(repo, "ExceptionForTest");
        }

        [Test]
        public void GetUser_ShouldReturnUser()
        {
            var mock = CreateMock();
            const int id = 1;
            mock.Setup(s => s.GetEntity(id)).Returns(ModelTestHelper.CreateUser(id, "GetUserTest"));
            var repo = CreateRepository(mock.Object);
            var user = repo.GetUser(id);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(user);
            Assert.AreEqual("GetUserTest", user.Pseudo);
        }

        [Test]
        public void GetUserByMail_WhenExceptionThrows_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserByMailAndPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ImportExportException("ExceptionForTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserByMail("toto", "pwd");
            CheckErrors(repo, "ExceptionForTest");
        }

        [Test]
        public void GetUserByMail_ShouldReturnUser()
        {
            const string mail = "MyEmail";
            const string password = "pwd";
            var mock = CreateMock();
            mock.Setup(s => s.GetUserByMailAndPassword(mail, password))
                .Returns(ModelTestHelper.CreateUser(1, "toto", mail:mail, password: password));
            var repo = CreateRepository(mock.Object);
            var user = repo.GetUserByMail(mail, password);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual(mail, user.Mail);
        }

        [Test]
        public void GetUserByPseudo_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserByPseudoAndPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new ImportExportException("ExceptionForTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserByPseudo("toto", "pwd");
            CheckErrors(repo, "ExceptionForTest");
        }

        [Test]
        public void GetUserByPseudo_ShouldReturnUser()
        {
            const string pseudo = "MyEmail";
            const string password = "pwd";
            var mock = CreateMock();
            mock.Setup(s => s.GetUserByPseudoAndPassword(pseudo, password))
                .Returns(ModelTestHelper.CreateUser(1, pseudo, password: password));
            var repo = CreateRepository(mock.Object);
            var user = repo.GetUserByPseudo(pseudo, password);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual(pseudo, user.Pseudo);
        }

        [Test]
        public void GetUserInfo_WhenExceptionIsThrown_ShouldLogError()
        {
            var mock = CreateMock();
            mock.Setup(s => s.GetUserInfo(It.IsAny<string>()))
                .Throws(new ImportExportException("ExceptionForTest"));
            var repo = CreateRepository(mock.Object);
            repo.GetUserInfo("toto");
            CheckErrors(repo, "ExceptionForTest");
        }

        [Test]
        public void GetUserInfo_ShouldReturnUser()
        {
            const string pseudo = "MyEmail";
            const string password = "pwd";
            var mock = CreateMock();
            mock.Setup(s => s.GetUserInfo(pseudo))
                .Returns(ModelTestHelper.CreateUser(1, pseudo, password: password));
            var repo = CreateRepository(mock.Object);
            var user = repo.GetUserInfo(pseudo);
            Assert.IsFalse(repo.HasErrors);
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual(pseudo, user.Pseudo);
        }

        #endregion

        #region Methods

        protected override IUserRepository CreateRepository(IUserDbImportExport persister)
        {
            return new UserRepository(persister);
        }

        #endregion
    }
}
