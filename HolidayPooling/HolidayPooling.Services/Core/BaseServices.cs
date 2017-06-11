using HolidayPooling.DataRepositories.Repository;
using System;
using System.Collections.Generic;

namespace HolidayPooling.Services.Core
{
    public abstract class BaseServices : IServices
    {

        #region IServices

        public List<string> Errors { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        #endregion

        #region .ctor

        public BaseServices()
        {
            Errors = new List<string>();
        }

        #endregion

        #region Methods

        protected void HandleException(Exception ex)
        {
            Errors.Add(ex.Message);
        }

        protected void MergeErrors(IRepository repository)
        {
            Errors.AddRange(repository.Errors);
        }

        #endregion

    }
}
