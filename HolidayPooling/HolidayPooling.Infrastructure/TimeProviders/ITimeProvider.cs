using System;

namespace HolidayPooling.Infrastructure.TimeProviders
{
    public interface ITimeProvider
    {

        DateTime Now();

    }
}
