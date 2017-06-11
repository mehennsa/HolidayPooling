using System;

namespace HolidayPooling.DataRepositories.Core
{
    public sealed class ImportExportException : Exception
    {

        public ImportExportException(string message)
            : base(message)
        {

        }

        public ImportExportException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }
}