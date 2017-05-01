using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public class PotRepository : RepositoryBase<Pot>, IPotRepository
    {


        #region Constants

        private const string NullPotErrorMessage = "Please provide a pot";

        private const string NullPotLogErrorMessage = "Pot is null, operation will not be executed";

        private const string SaveFailed = "Internal Error : Unable to save pot";

        private const string UpdateFailed = "Internal Error : Unable to update pot";

        private const string DeleteFailed = "Internal Error : Unable to delete pot";

        #endregion

        #region Properties

        private readonly IPotDbImportExport _persister;
        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public PotRepository()
            : base()
        {
            _persister = new PotDbImportExport();
        }


        public PotRepository(IPotDbImportExport persister)
        {
            _persister = persister;
        }


        #endregion

        #region IPotRepository

        public void SavePot(Pot pot)
        {
            Errors.Clear();

            if (!CheckModel(pot, NullPotErrorMessage, NullPotLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                if (_persister.IsPotNameUsed(pot.Name))
                {
                    Errors.Add(string.Format("Pot name {0} is already use, please choose another one", pot.Name));
                    _logger.Warn(string.Format("Pot name {0} is already use, please choose another one", pot.Name));
                    return;
                }

                _logger.Info("Start saving pot");
                var saved = _persister.Save(pot);
                _logger.Info("End saving pot");

                if(!saved) 
                {
                    SetErrorAndLog(SaveFailed, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
            
        }

        public void UpdatePot(Pot pot)
        {

            Errors.Clear();

            if (!CheckModel(pot, NullPotErrorMessage, NullPotLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                _logger.Info("Start updating pot");
                var updated = _persister.Update(pot);
                _logger.Info("End updating pot");

                if (!updated)
                {
                    SetErrorAndLog(UpdateFailed, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void DeletePot(Pot pot)
        {

            Errors.Clear();

            if (!CheckModel(pot, NullPotErrorMessage, NullPotLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start deleting pot");
                var deleted = _persister.Delete(pot);
                _logger.Info("End deleting pot");

                if (!deleted)
                {
                    SetErrorAndLog(DeleteFailed, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

        }

        public Pot GetPot(int potId)
        {
            Errors.Clear();
            Pot pot = null;

            try
            {

                _logger.Info(string.Format("Start retrieving pot {0}", potId));
                pot = _persister.GetEntity(potId);
                _logger.Info(string.Format("End retrieving pot {0}", potId));

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return pot;
        }

        public Pot GetPot(string potName)
        {
            Errors.Clear();
            Pot pot = null;

            try
            {

                _logger.Info(string.Format("Start retrieving pot {0}", potName));
                pot = _persister.GetPotByName(potName);
                _logger.Info(string.Format("End retrieving pot {0}", potName));

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return pot;
        }

        public Pot GetTripPots(int tripId)
        {
            Errors.Clear();
            Pot pot = null;

            try
            {
                _logger.Info(string.Format("Start retrieving pot for trip {0}", tripId));
                pot = _persister.GetTripsPot(tripId);
                _logger.Info(string.Format("End retrieving pot for trip {0}", tripId));

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return pot;
        }

        public IEnumerable<Pot> GetAllPots()
        {
            Errors.Clear();
            IEnumerable<Pot> list = new List<Pot>();

            try
            {

                _logger.Info("Start retrieving all pots");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieving all pots");

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        #endregion

    }
}
