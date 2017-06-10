using HolidayPooling.Services.Core;
using System;
using System.Collections.Generic;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.Repository;
using System.Transactions;

namespace HolidayPooling.Services.Pots
{
    public class PotServices : BaseServices, IPotServices
    {

        #region Repository Dependencies

        private readonly IPotRepository _potRepository;
        private readonly IPotUserRepository _potUserRepository;

        #endregion

        #region .ctor

        public PotServices()
            : base()
        {
            _potRepository = new PotRepository();
            _potUserRepository = new PotUserRepository();
        }

        public PotServices(IPotRepository potRepository, IPotUserRepository potUserRepository)
        {
            _potRepository = potRepository;
            _potUserRepository = potUserRepository;
        }

        #endregion

        #region IPotServices

        // TODO : Implementation
        public void Close(Pot pot)
        {
            throw new NotImplementedException();
        }

        // TODO : Payment System
        public void Credit(Pot pot, int userId, double amount)
        {
            InternalCreditDebitPot(pot, userId, p => p.CurrentAmount += amount, p => p.CurrentAmount -= amount, 
                u => 
                {
                    u.Amount += amount;
                    u.HasPayed = Math.Abs(u.Amount - u.TargetAmount) < double.Epsilon;
                }); 
        }

        // TODO : Payment system
        public void Debit(Pot pot, int userId, double amount)
        {
            InternalCreditDebitPot(pot, userId, p => p.CurrentAmount -= amount, p => p.CurrentAmount += amount, 
                u => 
                {
                    u.Amount -= amount;
                    u.HasPayed = Math.Abs(u.Amount - u.TargetAmount) < double.Epsilon;
                });
        }

        public Pot GetPot(int potId)
        {
            Errors.Clear();

            try
            {

                // 1 - pot
                var pot = _potRepository.GetPot(potId);
                if (pot == null || _potRepository.HasErrors)
                {
                    Errors.Add(string.Format("Unable to find pot with id : {0}", potId));
                    MergeErrors(_potRepository);
                    return null;
                }

                // 2 - Pot members
                var potMembers = _potUserRepository.GetPotUsers(pot.Id);
                if(_potUserRepository.HasErrors)
                {
                    Errors.Add(string.Format("Unable to find users for pot with id : {0}", potId));
                    MergeErrors(_potUserRepository);
                    return null;
                }

                foreach(var user in potMembers)
                {
                    pot.AddParticipant(user);
                }

                return pot;


            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        public IEnumerable<PotUser> GetPotMembers(int potId)
        {
            Errors.Clear();

            try
            {
                var potUsers = _potUserRepository.GetPotUsers(potId);
                if (_potUserRepository.HasErrors)
                {
                    Errors.Add(string.Format("Unable to find users for pot with id : {0}", potId));
                    MergeErrors(_potUserRepository);
                    return new List<PotUser>(0);
                }

                return potUsers;
            }
            catch(Exception ex)
            {
                HandleException(ex);
                return new List<PotUser>(0);
            }
        }

        #endregion

        #region Methods

        private void InternalCreditDebitPot(Pot pot, int userId,
            Action<Pot> potHandler, Action<Pot> potRollbackHandler, Action<PotUser> potUserHandler)
        {
            Errors.Clear();

            using (var scope = new TransactionScope())
            {
                try
                {

                    // 1 - Update pot user
                    var potUser = _potUserRepository.GetPotUser(pot.Id, userId);
                    if (potUser == null || _potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to find user in the pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    potUserHandler(potUser);

                    _potUserRepository.UpdatePotUser(potUser);
                    if (_potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to update the pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    // 2 - Update pot
                    potHandler(pot);
                    _potRepository.UpdatePot(pot);
                    if (_potRepository.HasErrors)
                    {
                        potRollbackHandler(pot);
                        Errors.Add("Unable to debit pot");
                        MergeErrors(_potRepository);
                        return;
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        #endregion
    }
}
