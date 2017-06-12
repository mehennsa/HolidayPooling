using NUnit.Framework;
using System;
using System.Linq;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.DataRepositories.ImportExport;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;

namespace HolidayPooling.DataRepositories.Tests.Business
{
    [TestFixture]
    public class UserDbImportExportTest : DbImportExportTestBase<UserDbImportExport, int, User>
    {

        #region DbImportExportTestBase<UserDbImportExport, int, User>

        protected override string TableName
        {
            get { return "TUSR"; }
        }

        protected override User CreateModel()
        {
            return ModelTestHelper.CreateUser(1, "APseudo");
        }

        protected override int GetKeyFromModel(User entity)
        {
            return entity.Id;
        }

        protected override void UpdateModel(User model)
        {
            model.Note += 3.2;
            model.PhoneNumber = "UpdateNumber";
        }

        public override void CompareWithDbValues(User entity, User dbEntity, DateTime modificationDate)
        {
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(entity.Id, dbEntity.Id);
            Assert.AreEqual(entity.Mail, dbEntity.Mail);
            Assert.AreEqual(entity.PhoneNumber, dbEntity.PhoneNumber);
            Assert.AreEqual(entity.Pseudo, dbEntity.Pseudo);
            Assert.AreEqual(entity.Password, dbEntity.Password);
            Assert.AreEqual(entity.Role, dbEntity.Role);
            Assert.AreEqual(entity.CreationDate, dbEntity.CreationDate);
            Assert.AreEqual(entity.Age, dbEntity.Age);
            Assert.AreEqual(entity.Description, dbEntity.Description);
            Assert.AreEqual(entity.Type, dbEntity.Type);
            Assert.AreEqual(entity.Note, dbEntity.Note);
            Assert.AreEqual(modificationDate, dbEntity.ModificationDate);
        }

        protected override UserDbImportExport CreateImportExport()
        {
            return new UserDbImportExport(new MockTimeProvider(_insertTime));
        }

        protected override UserDbImportExport CreateImportExportForUpdate()
        {
            return new UserDbImportExport(new MockTimeProvider(_updateTime));
        }

        #endregion

        #region Tests

        [Test]
        public void Save_WhenIdCannotBeGenerated_ShouldReturnFalse()
        {
            var model = CreateModel();
            var mock = CreateMock(false);
            mock.Setup(s => s.GetNewId()).Returns(-4);
            Assert.IsFalse(mock.Object.Save(model));
        }

        [Test]
        public void GetUserByMailAndPassword_WhenException_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetUserByMailAndPassword("Mail", "PWD"));
        }

        [Test]
        public void GetUserByMailAndPassword_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetUserByMailAndPassword("notexist", "wrongPwd"));
        }

        [Test]
        public void GetUserByMailAndPassword_WhenPasswordIsWrong_ShouldReturnNull()
        {
            var user = CreateModel();
            user.Mail = "WrongMail";
            Assert.IsTrue(_importExport.Save(user));
            Assert.IsNull(_importExport.GetUserByMailAndPassword("WrongMail", "Wrong"));
        }

        [Test]
        public void GetUserByMailAndPassword_WhenExist_ShouldReturnUser()
        {
            var user = CreateModel();
            Assert.IsTrue(_importExport.Save(user));
            user.Password = "Pwd";
            Assert.IsNotNull(_importExport.GetUserByMailAndPassword(user.Mail, user.Password));
        }

        [Test]
        public void GetUserByPseudoAndPaswword_WhenException_ShouldImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetUserByPseudoAndPassword("PSD", "PWD"));
        }

        [Test]
        public void GetUserByPseudoAndPassword_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetUserByPseudoAndPassword("WrongPsd", "PWD"));
        }

        [Test]
        public void GetUserByPseudoAndPassword_WhenPasswordIsWrong_ShouldReturnNull()
        {
            var user = CreateModel();
            user.Pseudo = "WrongPwd";
            Assert.IsTrue(_importExport.Save(user));
            Assert.IsNull(_importExport.GetUserByPseudoAndPassword("WrongPwd", "Wrong"));
        }

        [Test]
        public void GetUserByPseudoAndPassword_WhenExist_ShouldReturnUser()
        {
            var user = CreateModel();
            user.Pseudo = "NewPseudo";
            Assert.IsTrue(_importExport.Save(user));
            user.Password = "Pwd";
            Assert.IsNotNull(_importExport.GetUserByPseudoAndPassword(user.Pseudo, user.Password));
        }

        [Test]
        public void GetUserInfo_WhenException_ShouldImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetUserInfo("PSD"));
        }

        [Test]
        public void GetUserInfo_WhenNotExist_ShouldReturnNull()
        {
            Assert.IsNull(_importExport.GetUserInfo("WrongPsd"));
        }

        [Test]
        public void GetUserInfo_WhenExist_ShouldReturnUser()
        {
            var user = CreateModel();
            user.Pseudo = "NewPseudo";
            Assert.IsTrue(_importExport.Save(user));
            var dbUser = _importExport.GetUserInfo(user.Pseudo);
            Assert.IsNotNull(dbUser);
            Assert.IsEmpty(dbUser.Password);
        }

        [Test]
        public void GetNewId_WhenException_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetNewId());
        }

        [Test]
        public void IsMailUsed_WhenException_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.IsMailUsed("WrongMel"));
        }

        [Test]
        public void IsMailUsd_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.IsMailUsed("NotExistMail"));
        }

        [Test]
        public void IsMailUsed_WhenExist_ShouldReturnTrue()
        {
            var user = CreateModel();
            user.Mail = "MailUsed";
            Assert.IsTrue(_importExport.Save(user));
            Assert.IsTrue(_importExport.IsMailUsed("MailUsed"));
        }

        [Test]
        public void IsPseudoUsed_WhenException_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.IsPseudoUsed("Wrong"));
        }

        [Test]
        public void IsPseudoUsed_WhenNotExist_ShouldReturnFalse()
        {
            Assert.IsFalse(_importExport.IsPseudoUsed("PseudoNotExist"));
        }

        [Test]
        public void IsPseudoUsed_WhenExist_ShouldReturnTrue()
        {
            var user = CreateModel();
            user.Pseudo = "PseudoUsed";
            Assert.IsTrue(_importExport.Save(user));
            Assert.IsTrue(_importExport.IsPseudoUsed("PseudoUsed"));
        }

        [Test]
        public void GetAllEntities_ShouldReturnAllEntites()
        {
            var firstUser = ModelTestHelper.CreateUser(1, "First", "FirstMail");
            var secondUser = ModelTestHelper.CreateUser(-1, "Second", "SecondMail");
            var thirdUser = ModelTestHelper.CreateUser(-1, "Third", "ThirdMail");
            Assert.IsTrue(_importExport.Save(firstUser));
            Assert.IsTrue(_importExport.Save(secondUser));
            Assert.IsTrue(_importExport.Save(thirdUser));
            var list = _importExport.GetAllEntities().ToList();
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Any(u => u.Pseudo == "First"));
            Assert.IsTrue(list.Any(u => u.Mail == "SecondMail"));
            Assert.IsTrue(list.Any(u => u.Pseudo == "Third"));
        }

        #endregion

    }
}
