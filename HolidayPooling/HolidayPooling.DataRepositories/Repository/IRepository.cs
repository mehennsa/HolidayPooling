using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IRepository
    {

        IList<string> Errors { get; }

        bool HasErrors { get; }

    }
}
