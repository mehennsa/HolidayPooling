using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.DataRepositories.Repository
{
    public interface IRepository
    {

        IList<string> Errors { get; }

        bool HasErrors { get; }

    }
}
