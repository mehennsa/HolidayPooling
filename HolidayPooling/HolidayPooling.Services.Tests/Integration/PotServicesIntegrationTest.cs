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

namespace HolidayPooling.Services.Tests.Integration
{
    [TestFixture]
    public class PotServicesIntegrationTest : BaseIntegrationTest
    {
        #region BaseIntegrationTest

        protected override IEnumerable<string> TableUsed
        {
            get
            {
                yield return "TPOT";
                yield return "TPOTUSR";
            }
        }

        #endregion

        #region Tests

        [Test]
        public void Credit_WhenException_ShouldRollback()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount:200);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount:0, targetAmount:200);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.Setup(s => s.UpdatePot(It.IsAny<Pot>())).Throws(new Exception("Exception"));
            var services = new PotServices(mockPotRepo.Object, new PotUserRepository());
            services.Credit(pot, 1, 200);
            Assert.IsTrue(services.HasErrors);
            services = new PotServices();
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(200, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.First();
            Assert.IsNotNull(member);
            Assert.AreEqual(0, member.Amount);
        }

        [Test]
        public void Credit_WhenErrorDuringUpdate_ShouldRollback()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount: 200);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0, targetAmount: 200);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockPotRepo.SetupGet(s => s.Errors).Returns(new List<string> { "an error" });
            var services = new PotServices(mockPotRepo.Object, new PotUserRepository());
            services.Credit(pot, 1, 200);
            Assert.IsTrue(services.HasErrors);
            services = new PotServices();
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(200, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.First();
            Assert.IsNotNull(member);
            Assert.AreEqual(0, member.Amount);
        }

        [Test]
        public void Credit_WhenValid_ShouldCommit()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount: 200);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0, targetAmount: 200);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var services = new PotServices();
            services.Credit(pot, 1, 200);
            Assert.IsFalse(services.HasErrors);
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(400, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.FirstOrDefault();
            Assert.IsNotNull(member);
            Assert.AreEqual(200, member.Amount);
            Assert.IsTrue(member.HasPayed);
        }

        [Test]
        public void Debit_WhenException_ShouldRollback()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount: 200);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0, targetAmount: 200);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.Setup(s => s.UpdatePot(It.IsAny<Pot>())).Throws(new Exception("Exception"));
            var services = new PotServices(mockPotRepo.Object, new PotUserRepository());
            services.Debit(pot, 1, 200);
            Assert.IsTrue(services.HasErrors);
            services = new PotServices();
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(200, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.First();
            Assert.IsNotNull(member);
            Assert.AreEqual(0, member.Amount);
        }

        [Test]
        public void Debit_WhenErrorDuringUpdate_ShouldRollback()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount: 200);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 0, targetAmount: 200);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var mockPotRepo = new Mock<IPotRepository>();
            mockPotRepo.SetupGet(s => s.HasErrors).Returns(true);
            mockPotRepo.SetupGet(s => s.Errors).Returns(new List<string> { "an error" });
            var services = new PotServices(mockPotRepo.Object, new PotUserRepository());
            services.Credit(pot, 1, 200);
            Assert.IsTrue(services.HasErrors);
            services = new PotServices();
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(200, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.First();
            Assert.IsNotNull(member);
            Assert.AreEqual(0, member.Amount);
        }

        [Test]
        public void Debit_WhenValid_ShouldCommit()
        {
            var pot = ModelTestHelper.CreatePot(-1, 1, amount: 700);
            var potRepo = new PotRepository();
            potRepo.SavePot(pot);
            Assert.IsFalse(potRepo.HasErrors);
            var potUser = ModelTestHelper.CreatePotUser(1, pot.Id, amount: 450, targetAmount: 500);
            var potUserRepo = new PotUserRepository();
            potUserRepo.SavePotUser(potUser);
            Assert.IsFalse(potUserRepo.HasErrors);
            var services = new PotServices();
            services.Debit(pot, 1, 200);
            Assert.IsFalse(services.HasErrors);
            var dbPot = services.GetPot(pot.Id);
            Assert.IsNotNull(dbPot);
            Assert.AreEqual(500, dbPot.CurrentAmount);
            Assert.AreEqual(1, dbPot.Participants.Count());
            var member = dbPot.Participants.FirstOrDefault();
            Assert.IsNotNull(member);
            Assert.AreEqual(250, member.Amount);
            Assert.IsFalse(member.HasPayed);
        }

        #endregion
    }
}
