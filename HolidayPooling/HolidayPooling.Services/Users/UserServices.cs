using HolidayPooling.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.Repository;
using System.Transactions;

namespace HolidayPooling.Services.Users
{
    public class UserServices : BaseServices, IUserServices
    {


        #region Repositories Dependencies

        private readonly IUserRepository _userRepo;

        private readonly IUserTripRepository _userTripRepository;

        private readonly IFriendshipRepository _friendshipRepository;

        #endregion

        #region .ctor

        public UserServices() : base()
        {
            _userRepo = new UserRepository();
            _userTripRepository = new UserTripRepository();
            _friendshipRepository = new FriendshipRepository();
        }

        public UserServices(IUserRepository userRepo, IUserTripRepository userTripRepo, IFriendshipRepository friendshipRepo) : base()
        {
            _userRepo = userRepo;
            _userTripRepository = userTripRepo;
            _friendshipRepository = friendshipRepo;
        }

        #endregion

        #region IUserServices

        public void CreateUser(User user)
        {

            Errors.Clear();

            // Starting transaction
            using(var scope = new TransactionScope())
            {
                try
                {
                    _userRepo.SaveUser(user);

                    if (_userRepo.HasErrors)
                    {
                        MergeErrors(_userRepo);
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

        public void UpdateUser(User user)
        {
            Errors.Clear();

            // Starting transaction
            using (var scope = new TransactionScope())
            {
                try
                {
                    _userRepo.UpdateUser(user);

                    if (_userRepo.HasErrors)
                    {
                        MergeErrors(_userRepo);
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

        public void DeleteUser(User user)
        {

            Errors.Clear();

            // Starting transaction
            using (var scope = new TransactionScope())
            {
                try
                {
                    // deleting friendship
                    foreach(var friend in user.Friends)
                    {
                        var otherUser = _userRepo.GetUserInfo(friend.FriendName);
                        if (otherUser == null || _userRepo.HasErrors)
                        {
                            break;
                        }

                        var otherFriendship = _friendshipRepository.GetFriendship(otherUser.Id, user.Pseudo);

                        if (otherFriendship == null || _friendshipRepository.HasErrors)
                        {
                            break;
                        }

                        _friendshipRepository.DeleteFriendship(friend);
                        
                        if (_friendshipRepository.HasErrors)
                        {
                            break;
                        }

                        _friendshipRepository.DeleteFriendship(otherFriendship);

                        if (_friendshipRepository.HasErrors)
                        {
                            break; 
                        }
                        
                    }

                    if (_friendshipRepository.HasErrors || _userRepo.HasErrors)
                    {
                        Errors.Add("Unable to delete friendship");
                        MergeErrors(_friendshipRepository);
                        MergeErrors(_userRepo);
                        return;
                    }

                    // Trips
                    foreach(var trip in user.Trips)
                    {
                        _userTripRepository.DeleteUserTrip(trip);

                        if(_userTripRepository.HasErrors)
                        {
                            Errors.Add("Unable to delete user's trip");
                            MergeErrors(_userTripRepository);
                            return;
                        }
                    }

                    // User
                    _userRepo.DeleteUser(user);

                    if(_userRepo.HasErrors)
                    {
                        Errors.Add("Unable to delete user");
                        MergeErrors(_userRepo);
                        return;
                    }

                    // end the transaction
                    scope.Complete();

                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                
            }
        }

        public User LoginByMail(string mail, string password)
        {

            Errors.Clear();
            User user = null;
            try
            {
                user = _userRepo.GetUserByMail(mail, password);
                if(user == null || _userRepo.HasErrors)
                {
                    Errors.Add("Unable to find this account");
                    MergeErrors(_userRepo);
                }
                else
                {
                    FillUser(user);
                }

                if (HasErrors)
                {
                    Errors.Add("Unable to find user information");
                    user = null;
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return user;
        }

        public User LoginByPseudo(string pseudo, string password)
        {

            Errors.Clear();
            User user = null;
            try
            {
                user = _userRepo.GetUserByPseudo(pseudo, password);

                if (user == null || _userRepo.HasErrors)
                {
                    Errors.Add("Unable to find this account");
                    MergeErrors(_userRepo);
                    user = null;
                }
                else
                {
                    FillUser(user);
                }

                if(HasErrors)
                {
                    Errors.Add("Unable to find user information");
                    user = null;
                }
            }

            catch (Exception ex)
            {
                HandleException(ex);
            }

            return user;
        }

        public IEnumerable<UserTrip> GetUserTrips(int userId)
        {

            Errors.Clear();

            IEnumerable<UserTrip> userTrips = new List<UserTrip>();

            try
            {
                userTrips = _userTripRepository.GetUserTrips(userId);
                
                if(_userTripRepository.HasErrors)
                {
                    MergeErrors(_userTripRepository);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return userTrips;
        }

        public IEnumerable<Friendship> GetUserFriendships(int userId)
        {

            Errors.Clear();

            IEnumerable<Friendship> friends = new List<Friendship>();

            try
            {
                friends = _friendshipRepository.GetUserFriendships(userId);
                
                if(_friendshipRepository.HasErrors)
                {
                    MergeErrors(_friendshipRepository);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return friends;
        }

        public User GetUserInfo(string pseudo)
        {

            User user = null;

            try
            {

                user = _userRepo.GetUserInfo(pseudo);

                if(user ==  null || _userRepo.HasErrors)
                {
                    MergeErrors(_userRepo);
                    user = null;
                }
            
            }
            catch (Exception ex)
            {
                user = null;
                HandleException(ex);
            }

            return user;
        }

        #endregion

        #region Methods

        private void FillUser(User user)
        {

            foreach (var trip in GetUserTrips(user.Id))
            {
                user.AddTrip(trip);
            }

            if(HasErrors)
            {
                return;
            }

            foreach(var friend in GetUserFriendships(user.Id))
            {
                user.AddFriend(friend);
            }

            if(HasErrors)
            {
                return;
            }
        }

        #endregion
    }
}
