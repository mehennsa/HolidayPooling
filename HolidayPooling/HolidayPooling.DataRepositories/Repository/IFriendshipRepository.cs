﻿using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IFriendshipRepository : IRepository
    {

        void SaveFriendship(Friendship friendship);

        void UpdateFrendship(Friendship friendship);

        void DeleteFriendship(Friendship friendship);

        Friendship GetFriendship(int userId, string friendName);

        IEnumerable<Friendship> GetUserFriendships(int userId);

        IEnumerable<Friendship> GetRequestedFriendships(int userId);

        IEnumerable<Friendship> GetWaitingFriendships(int userId);

        IEnumerable<Friendship> GetAllFriendship();

    }
}
