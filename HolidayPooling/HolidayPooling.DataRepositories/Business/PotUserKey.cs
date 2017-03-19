﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public class PotUserKey
    {
        #region Fields

        public int PotId { get; private set; }

        public int UserId { get; private set; }

        #endregion

        #region .ctor

        public PotUserKey(int potId, int userId)
        {
            PotId = potId;
            UserId = userId;
        }

        #endregion

    }
}