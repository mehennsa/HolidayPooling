using HolidayPooling.DataRepositories.Core;
using HolidayPooling.Models.Core;

namespace HolidayPooling.DataRepositories.ImportExport
{
    public interface IUserDbImportExport : IDbImportExport<int, User>
    {

        User GetUserByMailAndPassword(string mail, string password);

        User GetUserByPseudoAndPassword(string pseudo, string password);

        User GetUserInfo(string pseudo);

        bool IsPseudoUsed(string pseudo);

        bool IsMailUsed(string mail);

    }
}