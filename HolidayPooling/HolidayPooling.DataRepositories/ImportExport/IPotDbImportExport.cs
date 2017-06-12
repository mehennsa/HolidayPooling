using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;

namespace HolidayPooling.DataRepositories.ImportExport
{
    public interface IPotDbImportExport : IDbImportExport<int, Pot>
    {

        Pot GetTripsPot(int tripId);

        bool IsPotNameUsed(string potName);

        Pot GetPotByName(string potName);

    }
}