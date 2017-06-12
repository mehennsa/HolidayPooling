using HolidayPooling.DataRepositories.ImportExport;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public class TripParticipantRepository : RepositoryBase<TripParticipant>, ITripParticipantRepository
    {


        #region Constants

        private const string NullTripParticipantErrorMessage = "Please provide a valid trip participant";

        private const string NullTripParticipantLogErrorMessage = "Trip's participant not valid, operation will not be executed";

        private const string SaveFailed = "Internal Error : Unable to save trip's participant";

        private const string DeleteFailed = "Internal Error : Unable to delete trip's participant";

        private const string UpdateFailed = "Internal Error : Unable to update trip's participant";

        #endregion

        #region Import/Export

        private readonly ITripParticipantDbImportExport _persister;
        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public TripParticipantRepository() : base()
        {
            _persister = new TripParticipantDbImportExport();
        }

        public TripParticipantRepository(ITripParticipantDbImportExport persister)
        {
            _persister = persister;
        }

        #endregion

        #region ITripParticipantRepository

        public void SaveTripParticipant(TripParticipant tripParticipant)
        {
            Errors.Clear();

            if (!CheckModel(tripParticipant, NullTripParticipantErrorMessage, NullTripParticipantLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info("Start saving trip's participant");
                var saved = _persister.Save(tripParticipant);
                _logger.Info("End saving trip's participant");
                if (!saved)
                {
                    SetErrorAndLog(SaveFailed, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void UpdateTripParticipant(TripParticipant tripParticipant)
        {
            Errors.Clear();

            if (!CheckModel(tripParticipant, NullTripParticipantErrorMessage, NullTripParticipantLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                _logger.Info("Start updating trip's participant");
                var updated = _persister.Update(tripParticipant);
                _logger.Info("End updating trip's participant");
                
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

        public void DeleteTripParticipant(TripParticipant tripParticipant)
        {
            Errors.Clear();

            if (!CheckModel(tripParticipant, NullTripParticipantErrorMessage, NullTripParticipantLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                _logger.Info("Start deleting trip's participant");
                var deleted = _persister.Delete(tripParticipant);
                _logger.Info("End deleting trip's participant");
                
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

        public TripParticipant GetTripParticipant(int tripId, string userPseudo)
        {
            Errors.Clear();

            TripParticipant tripParticipant = null;

            try
            {
                _logger.Info(string.Format("Start retrieving participant {0} on trip {1}", userPseudo, tripId));
                tripParticipant = _persister.GetEntity(new TripParticipantKey(tripId, userPseudo));
                _logger.Info(string.Format("End retrieving participant {0} on trip {1}", userPseudo, tripId));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return tripParticipant;
        }

        public IEnumerable<TripParticipant> GetTripParticipants(int tripId)
        {
            Errors.Clear();
            IEnumerable<TripParticipant> list = new List<TripParticipant>();

            try
            {
                _logger.Info(string.Format("Start retrieving participants for trip {0}", tripId));
                list = _persister.GetParticipantsForTrip(tripId);
                _logger.Info(string.Format("End retrieving participants for trip {0}", tripId));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public IEnumerable<TripParticipant> GetAllTripParticipants()
        {
            Errors.Clear();
            IEnumerable<TripParticipant> list = new List<TripParticipant>();

            try
            {
                _logger.Info("Start retrieving all trip's participant");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieving all trip's participant");
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
