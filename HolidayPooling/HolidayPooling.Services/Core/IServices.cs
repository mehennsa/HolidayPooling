using System.Collections.Generic;

namespace HolidayPooling.Services.Core
{
    public interface IServices
    {

        List<string> Errors { get; }

        bool HasErrors { get; }

    }
}
