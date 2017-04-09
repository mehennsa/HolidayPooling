using HolidayPooling.Models.Core;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IPotRepository : IRepository
    {

        void SavePot(Pot pot);

        void UpdatePot(Pot pot);

        void DeletePot(Pot pot);

        Pot GetPot(int potId);

        Pot GetPot(string potName);

        Pot GetTripPots(int tripId);

        IEnumerable<Pot> GetAllPots();
    }
}
