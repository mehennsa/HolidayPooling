using HolidayPooling.DataRepositories.Business;
using HolidayPooling.Infrastructure.Configuration;
using HolidayPooling.Models.Core;
using log4net;
using System;
using System.Collections.Generic;

namespace HolidayPooling.DataRepositories.Repository
{
    public class TripRepository : RepositoryBase<Trip>, ITripRepository
    {

        #region Constants

        private const string NullTripErrorMessage = "Please provide a valid trip";

        private const string NullTripLogErrorMessage = "Trip is null, operation will not be executed";

        private const string SaveFail = "Internal Error : Trip cannot be save";

        private const string UpdateFail = "Internal Error : Trip cannot be updated";

        private const string DeleteFail = "Internal Error : Trip cannot be deleted";

        #endregion

        #region Properties

        private readonly ITripDbImportExport _persister;
        private static readonly ILog _logger = LoggerManager.GetLogger(LoggerNames.RepositoryLogger);

        #endregion

        #region .ctor

        public TripRepository()
            : base()
        {
            _persister = new TripDbImportExport();
        }

        public TripRepository(ITripDbImportExport persister)
        {
            _persister = persister;
        }

        #endregion

        #region ITripRepository

        public void SaveTrip(Trip trip)
        {
            Errors.Clear();

            if (!CheckModel(trip, NullTripErrorMessage, NullTripLogErrorMessage, _logger))
            {
                return;
            }

            try
            {

                if (_persister.IsTripNameUsed(trip.TripName))
                {
                    Errors.Add(string.Format("Trip name {0} is already used, please use another one", trip.TripName));
                    _logger.Warn(string.Format("Trip name {0} is already used, please use another one", trip.TripName));
                    return;
                }

                _logger.Info("Start saving trip");
                var saved = _persister.Save(trip);
                _logger.Info("End saving trip");
                
                if (!saved)
                {
                    SetErrorAndLog(SaveFail, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void UpdateTrip(Trip trip)
        {
            Errors.Clear();

            if (!CheckModel(trip, NullTripErrorMessage, NullTripLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info(string.Format("Start updating trip {0} ", trip.Id));
                var updated = _persister.Update(trip);
                _logger.Info(string.Format("End updating trip {0} ", trip.Id));
                if (!updated)
                {
                    SetErrorAndLog(UpdateFail, _logger);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public void DeleteTrip(Trip trip)
        {
            Errors.Clear();

            if (!CheckModel(trip, NullTripErrorMessage, NullTripLogErrorMessage, _logger))
            {
                return;
            }

            try
            {
                _logger.Info(string.Format("Start deleting trip {0}", trip.Id));
                var deleted = _persister.Delete(trip);
                _logger.Info(string.Format("End deleting trip {0}", trip.Id));

                if (!deleted)
                {
                    SetErrorAndLog(DeleteFail, _logger);
                }

            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }
        }

        public Trip GetTrip(int tripId)
        {
            Errors.Clear();

            Trip trip = null;

            try
            {
                _logger.Info(string.Format("Start retrieving trip {0}", tripId));
                trip = _persister.GetEntity(tripId);
                _logger.Info(string.Format("End retrieving trip {0}", tripId));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return trip;
        }

        public Trip GetTrip(string tripName)
        {
            Errors.Clear();

            Trip trip = null;

            try
            {
                _logger.Info(string.Format("Start retrieving trip {0}", tripName));
                trip = _persister.GetTripByName(tripName);
                _logger.Info(string.Format("End retrieving trip {0}", tripName));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return trip;
        }

        public IEnumerable<Trip> GetValidTrips(DateTime validityDate)
        {
            Errors.Clear();

            IEnumerable<Trip> list = new List<Trip>();

            try
            {
                _logger.Info(string.Format("Start retrieving valid trip after {0}", validityDate));
                list = _persister.GetValidTrips(validityDate);
                _logger.Info(string.Format("End retrieving valid trip after {0}", validityDate));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public IEnumerable<Trip> GetTripsByDate(DateTime? startDate, DateTime? endDate)
        {
            Errors.Clear();

            IEnumerable<Trip> list = new List<Trip>();

            try
            {
                _logger.Info(string.Format("Start retrieving trips between {0} and {1}", startDate, endDate));
                list = _persister.GetTripBetweenStartDateAndEndDate(startDate, endDate);
                _logger.Info(string.Format("End retrieving trips between {0} and {1}", startDate, endDate));
            }
            catch (Exception ex)
            {
                HandleException(ex, _logger);
            }

            return list;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            Errors.Clear();

            IEnumerable<Trip> list = new List<Trip>();

            try
            {
                _logger.Info("Start retrieving all trips");
                list = _persister.GetAllEntities();
                _logger.Info("End retrieving all trips");
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
