using HolidayPooling.Models.Core;
using HolidayPooling.Services.Core;
using System.Collections.Generic;

namespace HolidayPooling.Services.Pots
{
    public interface IPotServices : IServices
    {

        void Credit(Pot pot, int userId, double amount);

        void Debit(Pot pot, int userId, double amount);

        void Close(Pot pot);

        Pot GetPot(int potId);

        IEnumerable<PotUser> GetPotMembers(int potId);
    }
}
