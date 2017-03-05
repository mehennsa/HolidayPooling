using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IPotDbImportExport : IDbImportExport<int, Pot>
    {

        Pot GetTripsPot(int tripId);

        bool IsPotNameUsed(string potName);

        Pot GetPotByName(string potName);

    }
}