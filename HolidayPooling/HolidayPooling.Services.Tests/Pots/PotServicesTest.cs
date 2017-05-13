using HolidayPooling.DataRepositories.Repository;
using HolidayPooling.Models.Core;
using HolidayPooling.Services.Pots;
using HolidayPooling.Tests;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Tests.Pots
{
    [TestFixture]
    public class PotServicesTest
    {

        #region Fields

        private IPotServices _potServices;
        private Mock<IPotRepository> _mockPotRepo;
        private Mock<IPotUserRepository> _mockPotUserRepo;

        #endregion

        #region Methods

        private void CreateServices()
        {
            _potServices = new PotServices(_mockPotRepo.Object, _mockPotUserRepo.Object);
        }

        #endregion

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mockPotRepo = new Mock<IPotRepository>();
            _mockPotUserRepo = new Mock<IPotUserRepository>();
        }

        #endregion

        #region Tests

        [Test]
        public void Credit_WhenException_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _potServices.Credit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void Credit_WhenPotUserIsNull_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            const string error = "Unable to find user in the pot";
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(() => null);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            CreateServices();
            _potServices.Credit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Credit_WhenErrorWhileSearchingForPotUser_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Error while searching for pot";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _potServices.Credit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Credit_WhenUnableToUpdatePotUser_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to update pot user";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.UpdatePotUser(potUser)).Callback
                (
                
                () =>
                    {
                        _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
                        _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
                    }
                );
            CreateServices();
            _potServices.Credit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Credit_WhenUnableToUpdatePot_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount:400);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to update pot";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _potServices.Credit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.AreEqual(400, pot.CurrentAmount);
        }

        [Test]
        public void Credit_WhenValidAndUserHasTotallyPaid_ShouldNotLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount: 400, targetAmount:600);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount:0, targetAmount:200);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _potServices.Credit(pot, 1, 200);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.AreEqual(600, pot.CurrentAmount);
            Assert.AreEqual(200, potUser.Amount);
            Assert.IsTrue(potUser.HasPayed);
        }

        [Test]
        public void Credit_WhenValidAndUserHasNotTotallyPaid_ShouldNotLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount: 400, targetAmount: 700);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0, targetAmount: 300);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _potServices.Credit(pot, 1, 200);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.AreEqual(600, pot.CurrentAmount);
            Assert.AreEqual(200, potUser.Amount);
            Assert.IsFalse(potUser.HasPayed);
        }

        [Test]
        public void Debit_WhenException_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            _potServices.Debit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains("ExceptionForTest"));
        }

        [Test]
        public void Debit_WhenPotUserIsNull_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            const string error = "Unable to find user in the pot";
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(() => null);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            CreateServices();
            _potServices.Debit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Debit_WhenErrorWhileSearchingForPotUser_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Error while searching for pot";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _potServices.Debit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Debit_WhenUnableToUpdatePotUser_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to update pot user";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.UpdatePotUser(potUser)).Callback
                (

                () =>
                {
                    _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
                    _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
                }
                );
            CreateServices();
            _potServices.Debit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
        }

        [Test]
        public void Debit_WhenUnableToUpdatePot_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount: 400);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id);
            const string error = "Unable to update pot";
            var errors = new List<string> { error };
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            _potServices.Debit(pot, 1, 100);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.AreEqual(400, pot.CurrentAmount);
        }

        [Test]
        public void Debit_WhenValidAndUserHasTotallyPaid_ShouldNotLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount: 400, targetAmount: 600);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 200, targetAmount: 200);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _potServices.Debit(pot, 1, 0);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.AreEqual(400, pot.CurrentAmount);
            Assert.AreEqual(200, potUser.Amount);
            Assert.IsTrue(potUser.HasPayed);
        }

        [Test]
        public void Debit_WhenValidAndUserHasNotTotallyPaid_ShouldNotLogError()
        {
            var pot = ModelTestHelper.CreatePot(2, 1, amount: 600, targetAmount: 700);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 300, targetAmount: 300);
            _mockPotUserRepo.Setup(s => s.GetPotUser(pot.Id, 1)).Returns(potUser);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            _potServices.Debit(pot, 1, 200);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.AreEqual(400, pot.CurrentAmount);
            Assert.AreEqual(100, potUser.Amount);
            Assert.IsFalse(potUser.HasPayed);
        }

        [Test]
        public void GetPot_WhenException_ShouldLogError()
        {
            _mockPotRepo.Setup(s => s.GetPot(1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            var dbPot = _potServices.GetPot(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains("ExceptionForTest"));
            Assert.IsNull(dbPot);
        }

        [Test]
        public void GetPot_WhenLoadedPotIsNull_ShouldLogError()
        {
            _mockPotRepo.Setup(s => s.GetPot(1)).Returns(() => null);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(new List<string>());
            var error = string.Format("Unable to find pot with id : {0}", 1);
            CreateServices();
            var dbPot = _potServices.GetPot(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.IsNull(dbPot);
        }

        [Test]
        public void GetPot_WhenErrorWhileLoadingPot_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(1, 1);
            const string error = "Unable to retrieve pot";
            var errors = new List<string> { error };
            _mockPotRepo.Setup(s => s.GetPot(1)).Returns(pot);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            var dbPot = _potServices.GetPot(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.IsNull(dbPot);
        }

        [Test]
        public void GetPot_WhenUnableToRetrievePotMembers_ShouldLogError()
        {
            var pot = ModelTestHelper.CreatePot(1, 1);
            const string error = "Unable to retrieve pot members";
            var errors = new List<string> { error };
            _mockPotRepo.Setup(s => s.GetPot(1)).Returns(pot);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            var dbPot = _potServices.GetPot(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.IsNull(dbPot);
        }

        [Test]
        public void GetPot_WhenValid_ShouldReturnPot()
        {
            var pot = ModelTestHelper.CreatePot(1, 1);
            var potUser = ModelTestHelper.CreatePotUser(1, 1);
            _mockPotRepo.Setup(s => s.GetPot(1)).Returns(pot);
            _mockPotRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            _mockPotUserRepo.Setup(s => s.GetPotUsers(1)).Returns(new List<PotUser> { potUser });
            CreateServices();
            var dbPot = _potServices.GetPot(1);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(1, dbPot.Participants.Count());
        }

        [Test]
        public void GetPotMembers_WhenException_ShouldLogError()
        {
            _mockPotUserRepo.Setup(s => s.GetPotUsers(1)).Throws(new Exception("ExceptionForTest"));
            CreateServices();
            var list = _potServices.GetPotMembers(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains("ExceptionForTest"));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetPotMembers_WhileLoadingMembers_ShouldLogError()
        {
            const string error = "Error while loading members";
            var errors = new List<string> { error };
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(true);
            _mockPotUserRepo.SetupGet(s => s.Errors).Returns(errors);
            CreateServices();
            var list = _potServices.GetPotMembers(1);
            Assert.IsTrue(_potServices.HasErrors);
            Assert.IsTrue(_potServices.Errors.Contains(error));
            Assert.AreEqual(0, list.Count());
        }

        [Test]
        public void GetPotMembers_WhenValid_ShouldReturnMembersList()
        {
            var firstMember = ModelTestHelper.CreatePotUser(1, 1);
            var secondMember = ModelTestHelper.CreatePotUser(2, 1);
            _mockPotUserRepo.Setup(s => s.GetPotUsers(1)).Returns(new List<PotUser> { firstMember, secondMember });
            _mockPotUserRepo.SetupGet(s => s.HasErrors).Returns(false);
            CreateServices();
            var list = _potServices.GetPotMembers(1);
            Assert.IsFalse(_potServices.HasErrors);
            Assert.AreEqual(2, list.Count());
        }

        //TODO: remove when method is implemeted
        [Test]
        public void Close_ShouldThrowNotImplementedException()
        {
            CreateServices();
            Assert.Throws<NotImplementedException>(() => _potServices.Close(null));
        }

        #endregion

    }
}
