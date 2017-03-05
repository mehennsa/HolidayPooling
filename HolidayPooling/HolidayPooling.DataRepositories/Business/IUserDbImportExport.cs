using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Business
{
    public interface IUserDbImportExport : IDbImportExport<int, User>
    {

        User GetUserByMailAndPassword(string mail, string password);

        User GetUserByPseudoAndPassword(string pseudo, string password);

        bool IsPseudoUsed(string pseudo);

        bool IsMailUsed(string mail);

    }
}