using HolidayPooling.DataRepositories.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public abstract class RepositoryBase<T> : IRepository where T : class, new()
    {

        #region Properties

        public IList<string> Errors { get; set; }

        public bool HasErrors
        {
            get { return Errors == null || Errors.Count > 0; }
        }

        #endregion

        #region .ctor

        public RepositoryBase()
        {
            Errors = new List<string>();
        }

        #endregion

        #region Methods

        protected void HandleException(Exception ex, ILog logger)
        {
            if (ex is ImportExportException)
            {
                Errors.Add(ex.Message);
                logger.Error(ex.Message, ex);
            }
            else
            {
                Errors.Add("Unexpected error occured " + ex.Message);
                logger.Error("Unexpected error occured " + ex.Message, ex);
            }
        }

        protected void SetErrorAndLog(string message, ILog logger)
        {
            Errors.Add(message);
            logger.Error(message);
        }

        protected bool CheckModel(T model, string msg, string logMsg, ILog logger)
        {
            var result = true;
            if (model == null)
            {
                Errors.Add(msg);
                logger.Warn(logMsg);
                result = false;
            }

            return result;
        }

        #endregion
    }
}
