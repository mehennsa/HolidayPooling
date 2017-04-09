using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayPooling.Services.Core
{
    public interface IServices
    {

        List<string> Errors { get; }

        bool HasErrors { get; }

    }
}
