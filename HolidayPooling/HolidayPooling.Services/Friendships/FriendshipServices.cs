using HolidayPooling.Services.Core;
using System;
using System.Collections.Generic;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.Repository;
using System.Transactions;

namespace HolidayPooling.Services.Friendships
{
    public class FriendshipServices : BaseServices, IFriendshipServices
    {


        #region Repositories Dependencies

        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        #endregion

        #region .ctor

        public FriendshipServices() 
            : base()
        {
            _userRepository = new UserRepository();
            _friendshipRepository = new FriendshipRepository();
        }

        public FriendshipServices(IUserRepository userRepository, IFriendshipRepository friendshipRepository)
            : base()
        {
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        #endregion

        #region IFrendshipServices

        public void AcceptFriendship(Friendship friendship, string userPseudo)
        {
            Errors.Clear();
            using (var scope = new TransactionScope())
            {
                try
                {
                    // update current friendship
                    friendship.IsWaiting = false;
                    _friendshipRepository.UpdateFrendship(friendship);

                    if(_friendshipRepository.HasErrors)
                    {
                        friendship.IsWaiting = true;
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    // retrieving other user
                    var friend = _userRepository.GetUserInfo(friendship.FriendName);

                    if(_userRepository.HasErrors || friend == null)
                    {
                        Errors.Add("Unable to find friend's account");
                        friendship.IsWaiting = true;
                        MergeErrors(_userRepository);
                        return;
                    }

                    // retrieving friend friendship
                    var otherFriendship = _friendshipRepository.GetFriendship(friend.Id, userPseudo);

                    if(_friendshipRepository.HasErrors || otherFriendship == null)
                    {
                        Errors.Add("Unable to retrieve friendship");
                        friendship.IsWaiting = true;
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    otherFriendship.IsWaiting = false;
                    // saving other friendship
                    _friendshipRepository.UpdateFrendship(otherFriendship);

                    if(_friendshipRepository.HasErrors)
                    {
                        Errors.Add("Unable to update other friendship");
                        friendship.IsWaiting = true;
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    friendship.IsWaiting = true;
                    HandleException(ex);
                }
                
            }

        }

        public void DenyFriendship(Friendship friendship, string userPseudo)
        {
            Errors.Clear();

            using(var scope = new TransactionScope())
            {
                try
                {

                    // Remove Current Friendship
                    _friendshipRepository.DeleteFriendship(friendship);

                    if(_friendshipRepository.HasErrors)
                    {
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    // Get other
                    var friend = _userRepository.GetUserInfo(friendship.FriendName);
                    if(_userRepository.HasErrors || friend == null)
                    {
                        Errors.Add("Unable to find friend's account");
                        MergeErrors(_userRepository);
                        return;
                    }

                    // Get friend friendship
                    var otherFriendship = _friendshipRepository.GetFriendship(friend.Id, userPseudo);
                    if(_friendshipRepository.HasErrors || otherFriendship == null)
                    {
                        Errors.Add("Unable to find friend's friendship");
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    //Remove other friendship
                    _friendshipRepository.DeleteFriendship(otherFriendship);

                    if (_friendshipRepository.HasErrors)
                    {
                        MergeErrors(_friendshipRepository);
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

        public IEnumerable<Friendship> GetUserFriendships(int userId)
        {

            return InternalGetFriendshipsUsingRepository
                (
                    userId,
                    i => _friendshipRepository.GetUserFriendships(i)
                );
        }

        public IEnumerable<Friendship> GetRequestedFriendships(int userId)
        {
            return InternalGetFriendshipsUsingRepository
                (
                    userId,
                    i => _friendshipRepository.GetRequestedFriendships(i)
                );
        }

        public IEnumerable<Friendship> GetUserFriendships(string pseudo)
        {
            Errors.Clear();

            try
            {
                // get user account
                var user = _userRepository.GetUserInfo(pseudo);
                
                if(_userRepository.HasErrors || user == null)
                {
                    Errors.Add("Unable to find user's account");
                    MergeErrors(_userRepository);
                    return new List<Friendship>(0);
                }

                var list = _friendshipRepository.GetUserFriendships(user.Id);

                if (_friendshipRepository.HasErrors)
                {
                    MergeErrors(_friendshipRepository);
                    return new List<Friendship>(0);
                }

                return list;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<Friendship>(0);
            }
        }

        public IEnumerable<Friendship> GetWaitingFriendships(int userId)
        {
            return InternalGetFriendshipsUsingRepository
                (
                    userId,
                    i => _friendshipRepository.GetWaitingFriendships(i)
                );
        }

        public void RequestFriendship(Friendship friendship, string userPseudo)
        {
            Errors.Clear();

            using(var scope = new TransactionScope())
            {
                try
                {
                    // Saving friendship
                    _friendshipRepository.SaveFriendship(friendship);

                    if(_friendshipRepository.HasErrors)
                    {
                        MergeErrors(_friendshipRepository);
                        return;
                    }

                    // retrieving friend's account
                    var friend = _userRepository.GetUserInfo(friendship.FriendName);

                    if(_userRepository.HasErrors || friend == null)
                    {
                        Errors.Add("Unable to find friend's account");
                        MergeErrors(_userRepository);
                        return;
                    }

                    // Creating opposite friendship
                    var otherFriendship = friendship.Clone() as Friendship;

                    if (otherFriendship == null)
                    {
                        Errors.Add("Unexpected error occured, please call support");
                        return;
                    }

                    otherFriendship.UserId = friend.Id;
                    otherFriendship.FriendName = userPseudo;
                    otherFriendship.IsRequested = false;
                    otherFriendship.IsWaiting = true;

                    //saving other friendship
                    _friendshipRepository.SaveFriendship(otherFriendship);

                    if (_friendshipRepository.HasErrors)
                    {
                        MergeErrors(_friendshipRepository);
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

        public Friendship GetFriendship(int userId, string friendName)
        {
            Friendship friendship = null;

            try
            {

                friendship = _friendshipRepository.GetFriendship(userId, friendName);

                if (_friendshipRepository.HasErrors || friendship == null)
                {
                    Errors.Add("Unable to find friendship");
                    MergeErrors(_friendshipRepository);
                }

            }
            catch(Exception ex)
            {
                HandleException(ex);
            }


            return friendship;
        }

        #endregion

        #region Methods

        private IEnumerable<Friendship> InternalGetFriendships(int userId, 
            Func<int, IEnumerable<Friendship>> func,
            Func<bool> errorsChecker,
            Action errorsHandler)
        {
            Errors.Clear();
            try
            {
                var list = func(userId);
                if(errorsChecker())
                {
                    errorsHandler();
                    return new List<Friendship>(0);
                }

                return list;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return new List<Friendship>(0);
            }
        }

        private IEnumerable<Friendship> InternalGetFriendshipsUsingRepository(int userId, Func<int, IEnumerable<Friendship>> func)
        {
            return InternalGetFriendships
                (
                    userId,
                    func,
                    () => _friendshipRepository.HasErrors,
                    () => MergeErrors(_friendshipRepository)
                );
        }

        #endregion
    }
}
