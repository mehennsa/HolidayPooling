using HolidayPooling.DataRepositories.Business;
using HolidayPooling.DataRepositories.Tests.Core;
using HolidayPooling.Models.Core;
using HolidayPooling.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Tests.Business
{
    // Test import export : Friendship
    [TestFixture]
    public class FriendshipDbImportExportTest :
        DbImportExportTestBase<FriendshipDbImportExport, FriendshipKey, Friendship>
    {

        #region DbImportExportTestBase<FriendshipDbImportExport, FriendshipKey, Friendship>

        protected override string TableName
        {
            get { return "TFRDSHP"; }
        }

        protected override Friendship CreateModel()
        {
            return ModelTestHelper.CreateFriendship(1, "Friend");
        }

        protected override FriendshipKey GetKeyFromModel(Friendship entity)
        {
            return new FriendshipKey(entity.UserId, entity.FriendName);
        }

        protected override void UpdateModel(Friendship model)
        {
            model.IsRequested = !model.IsRequested;
            model.IsWaiting = !model.IsWaiting;
        }

        public override void CompareWithDbValues(Friendship entity, Friendship dbEntity)
        {
            Assert.AreEqual(entity.UserId, dbEntity.UserId);
            Assert.AreEqual(entity.FriendName, dbEntity.FriendName);
            Assert.AreEqual(entity.IsRequested, dbEntity.IsRequested);
            Assert.AreEqual(entity.IsWaiting, dbEntity.IsWaiting);
            Assert.AreEqual(entity.StartDate, dbEntity.StartDate);
        }

        #endregion

        #region Tests

        [Test]
        public void GetUserFriendships_WhenExceptionThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetUserFriendships(-1));
        }

        [Test]
        public void GetUserFriendships_ShouldReturnRecordsForUser()
        {
            var firstFriend = ModelTestHelper.CreateFriendship(1, "toto");
            var secondFriend = ModelTestHelper.CreateFriendship(2, "tutu");
            var thirdFriend = ModelTestHelper.CreateFriendship(1, "Friend");
            Assert.IsTrue(_importExport.Save(firstFriend));
            Assert.IsTrue(_importExport.Save(secondFriend));
            Assert.IsTrue(_importExport.Save(thirdFriend));
            var list = _importExport.GetUserFriendships(1);
            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "toto"));
            Assert.IsTrue(list.Any(f => f.FriendName == "Friend"));
            Assert.IsFalse(list.Any(f => f.UserId == 2));
        }

        [Test]
        public void GetRequestedFriendships_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetRequestedFriendships(-1));
        }

        [Test]
        public void GetRequestedFrienships_ShouldReturnRightRecords()
        {
            var firstFriend = ModelTestHelper.CreateFriendship(1, "toto", isRequested:true, isWaiting:true);
            var secondFriend = ModelTestHelper.CreateFriendship(1, "tutu", isRequested:false, isWaiting:true);
            var thirdFriend = ModelTestHelper.CreateFriendship(1, "Friend", isRequested:true, isWaiting:false);
            Assert.IsTrue(_importExport.Save(firstFriend));
            Assert.IsTrue(_importExport.Save(secondFriend));
            Assert.IsTrue(_importExport.Save(thirdFriend));
            var list = _importExport.GetRequestedFriendships(1);
            Assert.AreEqual(1, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "toto"));
        }

        [Test]
        public void GetWaitingFriendships_WhenExceptionIsThrown_ShouldThrowImportExportException()
        {
            ImportExportExceptionTest(() => CreateMock().Object.GetWaitingFriendships(-1));
        }

        [Test]
        public void GetWaitingFriendships_ShouldReturnRightRecords()
        {
            var firstFriend = ModelTestHelper.CreateFriendship(1, "toto", isRequested: true, isWaiting: true);
            var secondFriend = ModelTestHelper.CreateFriendship(1, "tutu", isRequested: false, isWaiting: true);
            var thirdFriend = ModelTestHelper.CreateFriendship(1, "Friend", isRequested: true, isWaiting: false);
            Assert.IsTrue(_importExport.Save(firstFriend));
            Assert.IsTrue(_importExport.Save(secondFriend));
            Assert.IsTrue(_importExport.Save(thirdFriend));
            var list = _importExport.GetWaitingFriendships(1);
            Assert.AreEqual(1, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "tutu"));
        }

        [Test]
        public void GetAllEntities_ShouldReturnAllRecords()
        {
            var firstFriend = ModelTestHelper.CreateFriendship(1, "toto");
            var secondFriend = ModelTestHelper.CreateFriendship(2, "tutu");
            var thirdFriend = ModelTestHelper.CreateFriendship(1, "Friend");
            Assert.IsTrue(_importExport.Save(firstFriend));
            Assert.IsTrue(_importExport.Save(secondFriend));
            Assert.IsTrue(_importExport.Save(thirdFriend));
            var list = _importExport.GetAllEntities();
            Assert.AreEqual(3, list.Count());
            Assert.IsTrue(list.Any(f => f.FriendName == "toto"));
            Assert.IsTrue(list.Any(f => f.FriendName == "tutu"));
            Assert.IsTrue(list.Any(f => f.FriendName == "Friend"));
            Assert.IsTrue(list.Any(f => f.UserId == 2));
            Assert.IsTrue(list.Any(f => f.UserId == 1));
        }

        #endregion

    }
}
